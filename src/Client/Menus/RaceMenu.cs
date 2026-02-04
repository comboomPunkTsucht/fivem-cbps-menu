using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using LemonUI.Menus;
using Newtonsoft.Json;

using CBPSMenu.Shared;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Race Menu - Multiplayer race system with server sync
    /// Features: Race creation, saved templates, multiplayer racing
    /// Works with server/race.lua for synchronization
    /// </summary>
    public class RaceMenu
    {
        #region Variables

        public NativeMenu Menu { get; private set; }
        public NativeMenu SavedRacesMenu { get; private set; }
        public NativeMenu CreatorMenu { get; private set; }
        public NativeMenu JoinRaceMenu { get; private set; }

        // Race state
        private bool _isInRace = false;
        private bool _isCreatingRace = false;
        private int _currentRaceId = 0;
        private string _currentRaceName = "";
        private List<Vector3> _raceCheckpoints = new List<Vector3>();
        private int _currentCheckpoint = 0;
        private long _raceStartTime = 0;
        private List<int> _checkpointBlips = new List<int>();

        // Saved race templates from server
        private List<RaceTemplate> _savedTemplates = new List<RaceTemplate>();

        // Race settings
        private float _checkpointRadius = 10.0f;
        private float _checkpointRadiusSquared = 100.0f;
        private int _countdownTime = 5;
        private bool _showCheckpointMarkers = true;

        #endregion

        #region Data Structures

        public class RaceTemplate
        {
            public string Name { get; set; }
            public List<Vector3> Checkpoints { get; set; } = new List<Vector3>();
            public string CreatedBy { get; set; }
            public long CreatedAt { get; set; }
        }

        #endregion

        #region Constructor

        public RaceMenu()
        {
            CreateMenu();
        }

        #endregion

        #region Menu Creation

        private void CreateMenu()
        {
            Menu = ThemeManager.CreateThemedMenu("comboom.sucht", "Race Menu");

            // === RACE CREATOR SECTION ===
            var creatorHeader = new NativeItem("~g~=== Race Creator ===", "Create multiplayer races")
            {
                Enabled = false
            };
            Menu.Add(creatorHeader);

            // Start Creating Race
            var createRaceItem = new NativeItem("🆕 Create New Race", "Start creating a new race track");
            createRaceItem.Activated += (sender, args) =>
            {
                if (_isInRace)
                {
                    Main.ShowNotification("~r~Leave current race first!");
                    return;
                }

                // Trigger server event to create race
                BaseScript.TriggerServerEvent("cbps:createRace");
                _isCreatingRace = true;
                Main.ShowNotification("~g~Race creation started!\n~y~Add checkpoints at your position");
            };
            Menu.Add(createRaceItem);

            // Add Checkpoint
            var addCheckpointItem = new NativeItem("📍 Add Checkpoint Here", "Place checkpoint at current position");
            addCheckpointItem.Activated += (sender, args) =>
            {
                if (!_isCreatingRace)
                {
                    Main.ShowNotification("~r~Start creating a race first!");
                    return;
                }

                AddCheckpoint();
            };
            Menu.Add(addCheckpointItem);

            // Clear Checkpoints
            var clearCheckpointsItem = new NativeItem("🗑 Clear All Checkpoints", "~r~Remove all checkpoints");
            clearCheckpointsItem.Activated += (sender, args) =>
            {
                if (!_isCreatingRace)
                {
                    Main.ShowNotification("~r~No race being created!");
                    return;
                }

                ClearAllCheckpoints();
                BaseScript.TriggerServerEvent("cbps:clearRaceCheckpoints");
            };
            Menu.Add(clearCheckpointsItem);

            // Save Race Template
            var saveTemplateItem = new NativeItem("💾 Save Race Template", "~g~Save race for future use");
            saveTemplateItem.Activated += async (sender, args) =>
            {
                if (!_isCreatingRace || _raceCheckpoints.Count < 2)
                {
                    Main.ShowNotification("~r~Need at least 2 checkpoints!");
                    return;
                }

                var raceName = await Main.GetUserInput("Enter Race Name", "", 32);
                if (!string.IsNullOrEmpty(raceName))
                {
                    _currentRaceName = raceName;
                    BaseScript.TriggerServerEvent("cbps:saveRaceTemplate", raceName);
                }
            };
            Menu.Add(saveTemplateItem);

            // Start Race
            var startRaceItem = new NativeItem("🏁 Start Race", "~g~Start the countdown for all participants");
            startRaceItem.Activated += (sender, args) =>
            {
                if (_raceCheckpoints.Count < 2)
                {
                    Main.ShowNotification("~r~Need at least 2 checkpoints!");
                    return;
                }

                BaseScript.TriggerServerEvent("cbps:startRace");
            };
            Menu.Add(startRaceItem);

            Menu.Add(new NativeSeparatorItem());

            // === SAVED RACES SECTION ===
            var savedHeader = new NativeItem("~b~=== Saved Races ===", "Load saved race templates")
            {
                Enabled = false
            };
            Menu.Add(savedHeader);

            // Saved Races Submenu
            SavedRacesMenu = ThemeManager.CreateThemedMenu("comboom.sucht", "Saved Race Templates");
            var savedRacesItem = Menu.AddSubMenu(SavedRacesMenu);
            savedRacesItem.Title = "Browse Saved Races";
            savedRacesItem.Description = "Load a saved race template";
            Main.Pool.Add(SavedRacesMenu);

            // Refresh when menu opens
            SavedRacesMenu.Shown += (sender, args) =>
            {
                BaseScript.TriggerServerEvent("cbps:getSavedRaceTemplates");
            };

            // Refresh Templates
            var refreshTemplatesItem = new NativeItem("🔄 Refresh Templates", "Reload saved templates from server");
            refreshTemplatesItem.Activated += (sender, args) =>
            {
                BaseScript.TriggerServerEvent("cbps:getSavedRaceTemplates");
                Main.ShowNotification("~b~Requesting saved templates...");
            };
            Menu.Add(refreshTemplatesItem);

            Menu.Add(new NativeSeparatorItem());

            // === JOIN RACE SECTION ===
            var joinHeader = new NativeItem("~o~=== Join Race ===", "Join an existing race")
            {
                Enabled = false
            };
            Menu.Add(joinHeader);

            // Join Available Race
            var joinRaceItem = new NativeItem("🏎 Join Available Race", "Join a race created by another player");
            joinRaceItem.Activated += (sender, args) =>
            {
                if (_isInRace)
                {
                    Main.ShowNotification("~r~Already in a race!");
                    return;
                }

                BaseScript.TriggerServerEvent("cbps:joinRace");
            };
            Menu.Add(joinRaceItem);

            // Leave Race
            var leaveRaceItem = new NativeItem("~r~Leave Race", "~r~Exit the current race");
            leaveRaceItem.Activated += (sender, args) =>
            {
                LeaveRace();
                BaseScript.TriggerServerEvent("cbps:leaveRace");
            };
            Menu.Add(leaveRaceItem);

            Menu.Add(new NativeSeparatorItem());

            // === SETTINGS SECTION ===
            var settingsHeader = new NativeItem("~c~=== Race Settings ===", "Configure race options")
            {
                Enabled = false
            };
            Menu.Add(settingsHeader);

            // Checkpoint Radius
            var radiusItem = new NativeListItem<float>("Checkpoint Radius", "Size of checkpoint trigger area",
                5.0f, 7.5f, 10.0f, 15.0f, 20.0f);
            radiusItem.SelectedItem = _checkpointRadius;
            radiusItem.ItemChanged += (sender, args) =>
            {
                _checkpointRadius = radiusItem.SelectedItem;
                _checkpointRadiusSquared = _checkpointRadius * _checkpointRadius;
                Main.ShowNotification($"~b~Checkpoint radius: {_checkpointRadius}m");
            };
            Menu.Add(radiusItem);

            // Show Markers Toggle
            var showMarkersItem = new NativeCheckboxItem("Show 3D Markers", "Display checkpoint markers", _showCheckpointMarkers);
            showMarkersItem.CheckboxChanged += (sender, args) =>
            {
                _showCheckpointMarkers = showMarkersItem.Checked;
            };
            Menu.Add(showMarkersItem);

            // Race Status
            var statusItem = new NativeItem("📊 Race Status", "Check current race status");
            statusItem.Activated += (sender, args) =>
            {
                ShowRaceStatus();
            };
            Menu.Add(statusItem);
        }

        private void PopulateSavedRacesMenu()
        {
            SavedRacesMenu.Clear();

            if (_savedTemplates.Count == 0)
            {
                var noRacesItem = new NativeItem("~c~No saved races found", "Create and save your first race!")
                {
                    Enabled = false
                };
                SavedRacesMenu.Add(noRacesItem);
                return;
            }

            // Header
            var listHeader = new NativeItem($"~g~{_savedTemplates.Count} Saved Templates", "")
            {
                Enabled = false
            };
            SavedRacesMenu.Add(listHeader);

            SavedRacesMenu.Add(new NativeSeparatorItem());

            // List templates
            for (int i = 0; i < _savedTemplates.Count; i++)
            {
                var template = _savedTemplates[i];
                int templateIndex = i + 1; // Lua is 1-indexed

                var templateItem = new NativeItem(
                    template.Name,
                    $"By: {template.CreatedBy} | {template.Checkpoints.Count} checkpoints"
                );

                templateItem.Activated += (sender, args) =>
                {
                    if (_isInRace)
                    {
                        Main.ShowNotification("~r~Leave current race first!");
                        return;
                    }

                    BaseScript.TriggerServerEvent("cbps:loadRaceTemplate", templateIndex);
                };

                SavedRacesMenu.Add(templateItem);
            }

            // Delete section
            SavedRacesMenu.Add(new NativeSeparatorItem());

            var deleteHeader = new NativeItem("~r~=== Delete Race ===", "")
            {
                Enabled = false
            };
            SavedRacesMenu.Add(deleteHeader);

            for (int i = 0; i < _savedTemplates.Count; i++)
            {
                var template = _savedTemplates[i];
                int templateIndex = i + 1;

                var deleteItem = new NativeItem($"~r~Delete: {template.Name}", "~r~Permanently delete");

                deleteItem.Activated += (sender, args) =>
                {
                    BaseScript.TriggerServerEvent("cbps:deleteRaceTemplate", templateIndex);
                    // Refresh after delete
                    BaseScript.TriggerServerEvent("cbps:getSavedRaceTemplates");
                };

                SavedRacesMenu.Add(deleteItem);
            }
        }

        #endregion

        #region Server Event Handlers

        /// <summary>
        /// Called when race is created by server
        /// </summary>
        public void OnRaceCreated(int raceId)
        {
            _currentRaceId = raceId;
            _isCreatingRace = true;
            _raceCheckpoints.Clear();
            ClearCheckpointBlips();

            Main.ShowNotification($"~g~Race #{raceId} created!\n~y~Add checkpoints and start when ready");
        }

        /// <summary>
        /// Handle receiving saved race templates from server
        /// </summary>
        public void OnReceiveSavedTemplates(dynamic templates)
        {
            try
            {
                _savedTemplates.Clear();

                if (templates != null)
                {
                    foreach (var template in templates)
                    {
                        var raceTemplate = new RaceTemplate
                        {
                            Name = (string)template.name,
                            CreatedBy = (string)template.createdBy,
                            CreatedAt = (long)template.createdAt,
                            Checkpoints = new List<Vector3>()
                        };

                        if (template.checkpoints != null)
                        {
                            foreach (var cp in template.checkpoints)
                            {
                                raceTemplate.Checkpoints.Add(new Vector3(
                                    (float)cp.x,
                                    (float)cp.y,
                                    (float)cp.z
                                ));
                            }
                        }

                        _savedTemplates.Add(raceTemplate);
                    }
                }

                PopulateSavedRacesMenu();
                Main.ShowNotification($"~g~Loaded {_savedTemplates.Count} race templates");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[RaceMenu] Error parsing templates: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle race template loaded from server
        /// </summary>
        public void OnRaceTemplateLoaded(dynamic checkpoints)
        {
            try
            {
                _raceCheckpoints.Clear();
                ClearCheckpointBlips();

                if (checkpoints != null)
                {
                    foreach (var cp in checkpoints)
                    {
                        var pos = new Vector3((float)cp.x, (float)cp.y, (float)cp.z);
                        _raceCheckpoints.Add(pos);
                        CreateCheckpointBlip(pos, _raceCheckpoints.Count);
                    }
                }

                _isCreatingRace = true;
                Main.ShowNotification($"~g~Loaded {_raceCheckpoints.Count} checkpoints");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[RaceMenu] Error loading template: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle joining a race
        /// </summary>
        public void OnJoinedRace(int raceId, dynamic checkpoints)
        {
            try
            {
                _currentRaceId = raceId;
                _raceCheckpoints.Clear();
                ClearCheckpointBlips();

                if (checkpoints != null)
                {
                    foreach (var cp in checkpoints)
                    {
                        var pos = new Vector3((float)cp.x, (float)cp.y, (float)cp.z);
                        _raceCheckpoints.Add(pos);
                        CreateCheckpointBlip(pos, _raceCheckpoints.Count);
                    }
                }

                Main.ShowNotification($"~g~Joined race #{raceId}\n~b~{_raceCheckpoints.Count} checkpoints\n~y~Waiting for race to start...");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[RaceMenu] Error joining race: {ex.Message}");
            }
        }

        /// <summary>
        /// Handle race started by server
        /// </summary>
        public void OnRaceStarted(int countdown)
        {
            _isInRace = true;
            _isCreatingRace = false;
            _currentCheckpoint = 0;
            _countdownTime = countdown;

            // Update blip colors
            for (int i = 0; i < _checkpointBlips.Count; i++)
            {
                API.SetBlipColour(_checkpointBlips[i], i == 0 ? 2 : 5);
            }

            Main.ShowNotification($"~g~Race starting in {countdown} seconds!");
            StartCountdown();
        }

        /// <summary>
        /// Handle checkpoint reached
        /// </summary>
        public void OnCheckpointReached(int checkpointNum)
        {
            API.PlaySoundFrontend(-1, "CHECKPOINT_NORMAL", "HUD_MINI_GAME_SOUNDSET", true);
            Main.ShowNotification($"~b~Checkpoint {checkpointNum}/{_raceCheckpoints.Count}!");
        }

        /// <summary>
        /// Handle race finished
        /// </summary>
        public void OnRaceFinished(int position, long time)
        {
            _isInRace = false;
            string formattedTime = FormatTime(time);

            string positionText = position == 1 ? "🥇 1st" : position == 2 ? "🥈 2nd" : position == 3 ? "🥉 3rd" : $"#{position}";

            API.PlaySoundFrontend(-1, "CHECKPOINT_PERFECT", "HUD_MINI_GAME_SOUNDSET", true);
            Main.ShowNotification($"~g~🏁 RACE FINISHED! 🏁~s~\nPosition: {positionText}\nTime: {formattedTime}");

            ClearCheckpointBlips();
        }

        /// <summary>
        /// Handle leaving race
        /// </summary>
        public void OnLeftRace()
        {
            _isInRace = false;
            _isCreatingRace = false;
            _currentRaceId = 0;
            _raceCheckpoints.Clear();
            ClearCheckpointBlips();

            API.FreezeEntityPosition(Game.PlayerPed.Handle, false);
            Main.ShowNotification("~r~Left race!");
        }

        #endregion

        #region Race Creation

        private void AddCheckpoint()
        {
            var position = Game.PlayerPed.Position;
            _raceCheckpoints.Add(position);

            // Send to server
            BaseScript.TriggerServerEvent("cbps:addRaceCheckpoint", new { x = position.X, y = position.Y, z = position.Z });

            // Create local blip
            CreateCheckpointBlip(position, _raceCheckpoints.Count);

            API.PlaySoundFrontend(-1, "WAYPOINT_SET", "HUD_FRONTEND_DEFAULT_SOUNDSET", true);
            Main.ShowNotification($"~g~Checkpoint {_raceCheckpoints.Count} added!");
        }

        private void CreateCheckpointBlip(Vector3 position, int number)
        {
            int blip = API.AddBlipForCoord(position.X, position.Y, position.Z);
            API.SetBlipSprite(blip, 1);
            API.SetBlipDisplay(blip, 4);
            API.SetBlipScale(blip, 0.9f);
            API.SetBlipColour(blip, number == 1 ? 2 : 5); // Green for first, yellow for rest
            API.SetBlipAsShortRange(blip, false);
            API.BeginTextCommandSetBlipName("STRING");
            API.AddTextComponentString($"CP {number}");
            API.EndTextCommandSetBlipName(blip);
            _checkpointBlips.Add(blip);
        }

        private void ClearAllCheckpoints()
        {
            _raceCheckpoints.Clear();
            ClearCheckpointBlips();
            Main.ShowNotification("~r~All checkpoints cleared!");
        }

        private void ClearCheckpointBlips()
        {
            foreach (var blip in _checkpointBlips)
            {
                int b = blip;
                API.RemoveBlip(ref b);
            }
            _checkpointBlips.Clear();
        }

        #endregion

        #region Race Participation

        private async void StartCountdown()
        {
            // Freeze player during countdown
            API.FreezeEntityPosition(Game.PlayerPed.Handle, true);

            for (int i = _countdownTime; i > 0; i--)
            {
                Main.ShowNotification($"~y~{i}...");
                API.PlaySoundFrontend(-1, "TIMER", "HUD_FRONTEND_DEFAULT_SOUNDSET", true);
                await BaseScript.Delay(1000);
            }

            // Unfreeze and start
            API.FreezeEntityPosition(Game.PlayerPed.Handle, false);
            API.PlaySoundFrontend(-1, "GO", "HUD_FRONTEND_DEFAULT_SOUNDSET", true);
            Main.ShowNotification("~g~GO!");

            _raceStartTime = API.GetGameTimer();
            MonitorRaceProgress();
        }

        private async void MonitorRaceProgress()
        {
            while (_isInRace && _currentCheckpoint < _raceCheckpoints.Count)
            {
                await BaseScript.Delay(0);

                var playerPos = Game.PlayerPed.Position;
                var checkpoint = _raceCheckpoints[_currentCheckpoint];

                // Draw checkpoint marker if enabled
                if (_showCheckpointMarkers)
                {
                    int r = _currentCheckpoint == 0 ? 0 : 255;
                    int g = 255;
                    int b = 0;

                    API.DrawMarker(1, checkpoint.X, checkpoint.Y, checkpoint.Z - 1.0f,
                        0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
                        _checkpointRadius * 2, _checkpointRadius * 2, 2.0f,
                        r, g, b, 100, false, true, 2, false, null, null, false);
                }

                // Check if player reached checkpoint
                float distance = playerPos.DistanceToSquared(checkpoint);
                if (distance < _checkpointRadiusSquared)
                {
                    ReachCheckpoint();
                }

                // Display race HUD
                DrawRaceHUD();
            }
        }

        private void ReachCheckpoint()
        {
            _currentCheckpoint++;

            // Notify server
            BaseScript.TriggerServerEvent("cbps:reachedCheckpoint", _currentCheckpoint);

            if (_currentCheckpoint >= _raceCheckpoints.Count)
            {
                // Race finished
                long finishTime = API.GetGameTimer() - _raceStartTime;
                BaseScript.TriggerServerEvent("cbps:finishRace", finishTime);
            }
            else
            {
                // Update blip colors
                if (_currentCheckpoint < _checkpointBlips.Count)
                {
                    API.SetBlipColour(_checkpointBlips[_currentCheckpoint - 1], 2); // Completed = green
                    API.SetBlipColour(_checkpointBlips[_currentCheckpoint], 5); // Next = yellow
                    API.SetBlipRoute(_checkpointBlips[_currentCheckpoint], true);
                    API.SetBlipRouteColour(_checkpointBlips[_currentCheckpoint], 2);
                }
            }
        }

        private void LeaveRace()
        {
            _isInRace = false;
            _isCreatingRace = false;
            _currentCheckpoint = 0;
            _raceCheckpoints.Clear();
            ClearCheckpointBlips();

            API.FreezeEntityPosition(Game.PlayerPed.Handle, false);
        }

        #endregion

        #region HUD & Helpers

        private void DrawRaceHUD()
        {
            if (!_isInRace) return;

            long currentTime = API.GetGameTimer() - _raceStartTime;
            string timeText = FormatTime(currentTime);
            string checkpointText = $"CP: {_currentCheckpoint + 1}/{_raceCheckpoints.Count}";

            // Draw race info (top right)
            API.SetTextFont(4);
            API.SetTextProportional(true);
            API.SetTextScale(0.5f, 0.5f);
            API.SetTextColour(255, 255, 255, 255);
            API.SetTextDropshadow(0, 0, 0, 0, 255);
            API.SetTextOutline();
            API.SetTextEntry("STRING");
            API.AddTextComponentString($"~y~{timeText}~s~ | {checkpointText}");
            API.DrawText(0.85f, 0.05f);
        }

        private string FormatTime(long milliseconds)
        {
            int totalSeconds = (int)(milliseconds / 1000);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;
            int ms = (int)(milliseconds % 1000);

            return $"{minutes:D2}:{seconds:D2}.{ms:D3}";
        }

        private void ShowRaceStatus()
        {
            string status = "~b~Race Status:~s~\n";

            if (_isCreatingRace)
            {
                status += $"~g~Creating race #{_currentRaceId}\n";
                status += $"Checkpoints: {_raceCheckpoints.Count}\n";
            }
            else if (_isInRace)
            {
                status += $"~g~Racing #{_currentRaceId}\n";
                status += $"Checkpoint: {_currentCheckpoint + 1}/{_raceCheckpoints.Count}\n";
                status += $"Time: {FormatTime(API.GetGameTimer() - _raceStartTime)}";
            }
            else
            {
                status += "~c~Not in a race\n";
                status += $"Saved templates: {_savedTemplates.Count}";
            }

            Main.ShowNotification(status);
        }

        #endregion

        #region Public Properties

        public bool IsInRace => _isInRace;
        public bool IsCreatingRace => _isCreatingRace;

        #endregion
    }
}
