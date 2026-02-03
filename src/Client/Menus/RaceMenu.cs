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
    /// Race Menu - Custom JSON-based race system
    /// High priority custom override feature
    /// </summary>
    public class RaceMenu
    {
        #region Variables

        public NativeMenu Menu { get; private set; }

        // Race state
        private bool _isInRace = false;
        private bool _isCreatingRace = false;
        private string _currentRaceId = "";
        private List<Vector3> _raceCheckpoints = new List<Vector3>();
        private int _currentCheckpoint = 0;
        private long _raceStartTime = 0;
        private List<int> _checkpointBlips = new List<int>();

        // Race settings
        private float _checkpointRadius = 10.0f;
        private float _checkpointRadiusSquared = 100.0f; // Cached for performance
        private int _maxCheckpoints = 20;
        private int _countdownTime = 5;

        #endregion

        #region Data Structures

        public class RaceData
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Creator { get; set; }
            public List<CheckpointData> Checkpoints { get; set; } = new List<CheckpointData>();
            public DateTime Created { get; set; }
        }

        public class CheckpointData
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
        }

        #endregion

        #region Constructor

        public RaceMenu()
        {
            CreateMenu();
            RegisterEventHandlers();
        }

        #endregion

        #region Menu Creation

        private void CreateMenu()
        {
            Menu = ThemeManager.CreateThemedMenu("comboom.sucht", "Race Menu");

            // Race Creator Header
            var creatorHeader = new NativeItem("~b~=== Race Creator ===", "Create custom races");
            creatorHeader.Enabled = false;
            Menu.Add(creatorHeader);

            // Create New Race
            var createRaceItem = new NativeItem("Create New Race", "Start creating a new race");
            createRaceItem.Activated += async (sender, args) =>
            {
                if (_isInRace)
                {
                    Main.ShowNotification("~r~Leave current race first!");
                    return;
                }

                var raceName = await Main.GetUserInput("Enter race name", "", 32);
                if (!string.IsNullOrEmpty(raceName))
                {
                    StartCreatingRace(raceName);
                }
            };
            Menu.Add(createRaceItem);

            // Add Checkpoint
            var addCheckpointItem = new NativeItem("Add Checkpoint", "Add checkpoint at current position");
            addCheckpointItem.Activated += (sender, args) =>
            {
                if (!_isCreatingRace)
                {
                    Main.ShowNotification("~r~You need to create a race first!");
                    return;
                }

                AddCheckpoint();
            };
            Menu.Add(addCheckpointItem);

            // Remove Last Checkpoint
            var removeCheckpointItem = new NativeItem("Remove Last Checkpoint", "Remove the last added checkpoint");
            removeCheckpointItem.Activated += (sender, args) =>
            {
                if (!_isCreatingRace || _raceCheckpoints.Count == 0)
                {
                    Main.ShowNotification("~r~No checkpoints to remove!");
                    return;
                }

                RemoveLastCheckpoint();
            };
            Menu.Add(removeCheckpointItem);

            // Clear All Checkpoints
            var clearCheckpointsItem = new NativeItem("Clear All Checkpoints", "~r~Remove all checkpoints");
            clearCheckpointsItem.Activated += (sender, args) =>
            {
                ClearAllCheckpoints();
            };
            Menu.Add(clearCheckpointsItem);

            // Save Race
            var saveRaceItem = new NativeItem("Save Race", "Save the current race");
            saveRaceItem.Activated += (sender, args) =>
            {
                if (!_isCreatingRace)
                {
                    Main.ShowNotification("~r~No race to save!");
                    return;
                }

                if (_raceCheckpoints.Count < 2)
                {
                    Main.ShowNotification("~r~Need at least 2 checkpoints!");
                    return;
                }

                SaveRace();
            };
            Menu.Add(saveRaceItem);

            // Cancel Race Creation
            var cancelCreateItem = new NativeItem("Cancel Creation", "~r~Cancel race creation");
            cancelCreateItem.Activated += (sender, args) =>
            {
                CancelRaceCreation();
            };
            Menu.Add(cancelCreateItem);

            // Add separator
            Menu.Add(new NativeSeparatorItem());

            // Race Participation Header
            var participateHeader = new NativeItem("~b~=== Race Participation ===", "Join and manage races");
            participateHeader.Enabled = false;
            Menu.Add(participateHeader);

            // Start Race
            var startRaceItem = new NativeItem("Start Race", "Start the race countdown");
            startRaceItem.Activated += (sender, args) =>
            {
                if (!_isCreatingRace || _raceCheckpoints.Count < 2)
                {
                    Main.ShowNotification("~r~No race ready to start!");
                    return;
                }

                StartRace();
            };
            Menu.Add(startRaceItem);

            // Leave Race
            var leaveRaceItem = new NativeItem("Leave Race", "~r~Leave the current race");
            leaveRaceItem.Activated += (sender, args) =>
            {
                LeaveRace();
            };
            Menu.Add(leaveRaceItem);

            // Add separator
            Menu.Add(new NativeSeparatorItem());

            // Race Settings Header
            var settingsHeader = new NativeItem("~b~=== Race Settings ===", "Configure race options");
            settingsHeader.Enabled = false;
            Menu.Add(settingsHeader);

            // Checkpoint Radius
            var radiusItem = new NativeListItem<float>("Checkpoint Radius", "Size of checkpoint trigger area",
                5.0f, 7.5f, 10.0f, 15.0f, 20.0f);
            radiusItem.SelectedItem = _checkpointRadius;
            radiusItem.ItemChanged += (sender, args) =>
            {
                _checkpointRadius = radiusItem.SelectedItem;
                _checkpointRadiusSquared = _checkpointRadius * _checkpointRadius; // Update cached value
                Main.ShowNotification($"~b~Checkpoint radius set to: {_checkpointRadius}m");
            };
            Menu.Add(radiusItem);

            // Countdown Time
            var countdownItem = new NativeListItem<int>("Countdown Time", "Race start countdown duration",
                3, 5, 10, 15);
            countdownItem.SelectedItem = _countdownTime;
            countdownItem.ItemChanged += (sender, args) =>
            {
                _countdownTime = countdownItem.SelectedItem;
                Main.ShowNotification($"~b~Countdown time set to: {_countdownTime}s");
            };
            Menu.Add(countdownItem);

            // Status info
            var statusItem = new NativeItem("Race Status", "Check current race status");
            statusItem.Activated += (sender, args) =>
            {
                ShowRaceStatus();
            };
            Menu.Add(statusItem);
        }

        #endregion

        #region Event Handlers

        private void RegisterEventHandlers()
        {
            // Register network events for multiplayer races
            BaseScript.TriggerEvent("cbps:registerRaceEvents");
        }

        #endregion

        #region Race Creation

        private void StartCreatingRace(string raceName)
        {
            _isCreatingRace = true;
            _currentRaceId = raceName;
            _raceCheckpoints.Clear();
            ClearCheckpointBlips();

            Main.ShowNotification($"~g~Started creating race: {raceName}");
            Main.ShowNotification("~y~Add checkpoints and save the race");
        }

        private void AddCheckpoint()
        {
            if (_raceCheckpoints.Count >= _maxCheckpoints)
            {
                Main.ShowNotification($"~r~Maximum checkpoints ({_maxCheckpoints}) reached!");
                return;
            }

            var position = Game.PlayerPed.Position;
            _raceCheckpoints.Add(position);

            // Create blip for checkpoint
            int blip = API.AddBlipForCoord(position.X, position.Y, position.Z);
            API.SetBlipSprite(blip, 1);
            API.SetBlipDisplay(blip, 4);
            API.SetBlipScale(blip, 0.8f);
            API.SetBlipColour(blip, 5); // Yellow
            API.SetBlipAsShortRange(blip, true);
            API.BeginTextCommandSetBlipName("STRING");
            API.AddTextComponentString($"Checkpoint {_raceCheckpoints.Count}");
            API.EndTextCommandSetBlipName(blip);
            _checkpointBlips.Add(blip);

            Main.ShowNotification($"~g~Checkpoint {_raceCheckpoints.Count} added!");

            // Trigger server event
            BaseScript.TriggerServerEvent("cbps:addRaceCheckpoint", _currentRaceId, position.X, position.Y, position.Z);
        }

        private void RemoveLastCheckpoint()
        {
            if (_raceCheckpoints.Count > 0)
            {
                _raceCheckpoints.RemoveAt(_raceCheckpoints.Count - 1);

                if (_checkpointBlips.Count > 0)
                {
                    int blipToRemove = _checkpointBlips[_checkpointBlips.Count - 1];
                    API.RemoveBlip(ref blipToRemove);
                    _checkpointBlips.RemoveAt(_checkpointBlips.Count - 1);
                }

                Main.ShowNotification($"~r~Checkpoint removed! Total: {_raceCheckpoints.Count}");
            }
        }

        private void ClearAllCheckpoints()
        {
            _raceCheckpoints.Clear();
            ClearCheckpointBlips();
            Main.ShowNotification("~r~All checkpoints cleared!");

            BaseScript.TriggerServerEvent("cbps:clearRaceCheckpoints", _currentRaceId);
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

        private void SaveRace()
        {
            var raceData = new RaceData
            {
                Id = _currentRaceId,
                Name = _currentRaceId,
                Creator = Game.Player.Name,
                Created = DateTime.Now,
                Checkpoints = new List<CheckpointData>()
            };

            foreach (var checkpoint in _raceCheckpoints)
            {
                raceData.Checkpoints.Add(new CheckpointData
                {
                    X = checkpoint.X,
                    Y = checkpoint.Y,
                    Z = checkpoint.Z
                });
            }

            // Save to JSON
            var json = JsonConvert.SerializeObject(raceData);
            API.SetResourceKvp($"cbps_race_{_currentRaceId}", json);

            Main.ShowNotification($"~g~Race saved: {_currentRaceId}");
            Main.ShowNotification($"~b~Checkpoints: {_raceCheckpoints.Count}");

            // Trigger server event
            BaseScript.TriggerServerEvent("cbps:saveRace", _currentRaceId, json);
        }

        private void CancelRaceCreation()
        {
            _isCreatingRace = false;
            _currentRaceId = "";
            _raceCheckpoints.Clear();
            ClearCheckpointBlips();
            Main.ShowNotification("~r~Race creation cancelled!");
        }

        #endregion

        #region Race Participation

        private void StartRace()
        {
            if (_raceCheckpoints.Count < 2)
            {
                Main.ShowNotification("~r~Need at least 2 checkpoints to start!");
                return;
            }

            _isInRace = true;
            _currentCheckpoint = 0;

            // Update blip colors
            for (int i = 0; i < _checkpointBlips.Count; i++)
            {
                API.SetBlipColour(_checkpointBlips[i], i == 0 ? 2 : 5); // Green for first, yellow for rest
            }

            Main.ShowNotification($"~g~Race starting in {_countdownTime} seconds!");

            // Start countdown and race monitoring
            StartCountdown();
        }

        private async void StartCountdown()
        {
            for (int i = _countdownTime; i > 0; i--)
            {
                Main.ShowNotification($"~y~{i}...");
                await BaseScript.Delay(1000);
            }

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

                // Draw checkpoint marker
                API.DrawMarker(1, checkpoint.X, checkpoint.Y, checkpoint.Z - 1.0f,
                    0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f,
                    _checkpointRadius * 2, _checkpointRadius * 2, 2.0f,
                    0, 255, 0, 100, false, true, 2, false, null, null, false);

                // Check if player reached checkpoint (use cached squared radius for performance)
                float distance = playerPos.DistanceToSquared(checkpoint);
                if (distance < _checkpointRadiusSquared)
                {
                    ReachCheckpoint();
                }

                // Display race info
                DrawRaceInfo();
            }
        }

        private void ReachCheckpoint()
        {
            _currentCheckpoint++;

            if (_currentCheckpoint >= _raceCheckpoints.Count)
            {
                // Race finished
                FinishRace();
            }
            else
            {
                Main.ShowNotification($"~b~Checkpoint {_currentCheckpoint}/{_raceCheckpoints.Count}!");

                // Update blip colors
                if (_currentCheckpoint < _checkpointBlips.Count)
                {
                    API.SetBlipColour(_checkpointBlips[_currentCheckpoint - 1], 2); // Completed = green
                    API.SetBlipColour(_checkpointBlips[_currentCheckpoint], 5); // Next = yellow
                }

                BaseScript.TriggerServerEvent("cbps:reachedCheckpoint", _currentRaceId, _currentCheckpoint);
            }
        }

        private void FinishRace()
        {
            long finishTime = API.GetGameTimer() - _raceStartTime;
            _isInRace = false;
            _isCreatingRace = false;

            string formattedTime = FormatTime(finishTime);
            Main.ShowNotification($"~g~Race finished! Time: {formattedTime}");

            BaseScript.TriggerServerEvent("cbps:finishRace", _currentRaceId, finishTime);

            ClearCheckpointBlips();
        }

        private void LeaveRace()
        {
            _isInRace = false;
            _isCreatingRace = false;
            _currentCheckpoint = 0;
            _raceCheckpoints.Clear();
            ClearCheckpointBlips();

            Main.ShowNotification("~r~Left race!");

            BaseScript.TriggerServerEvent("cbps:leftRace", _currentRaceId);
        }

        #endregion

        #region Helper Methods

        private void DrawRaceInfo()
        {
            if (!_isInRace) return;

            long currentTime = API.GetGameTimer() - _raceStartTime;
            string timeText = $"Time: {FormatTime(currentTime)}";
            string checkpointText = $"Checkpoint: {_currentCheckpoint + 1}/{_raceCheckpoints.Count}";

            // Draw text on screen (top right)
            API.SetTextFont(0);
            API.SetTextProportional(true);
            API.SetTextScale(0.5f, 0.5f);
            API.SetTextColour(255, 255, 255, 255);
            API.SetTextDropshadow(0, 0, 0, 0, 255);
            API.SetTextEdge(1, 0, 0, 0, 255);
            API.SetTextDropShadow();
            API.SetTextOutline();
            API.SetTextEntry("STRING");
            API.AddTextComponentString($"{timeText}\n{checkpointText}");
            API.DrawText(0.85f, 0.1f);
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
                status += $"Creating: {_currentRaceId}\n";
                status += $"Checkpoints: {_raceCheckpoints.Count}\n";
            }
            else if (_isInRace)
            {
                status += $"In race: {_currentRaceId}\n";
                status += $"Checkpoint: {_currentCheckpoint + 1}/{_raceCheckpoints.Count}\n";
            }
            else
            {
                status += "Not in a race";
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
