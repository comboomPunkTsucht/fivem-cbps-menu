using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using LemonUI.Menus;

using CBPSMenu.Client.Managers;
using CBPSMenu.Shared;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Player Options Menu - Enhanced with vMenu features
    /// Based on vMenu/vMenu/menus/PlayerOptions.cs
    /// </summary>
    public class PlayerMenu
    {
        #region Variables

        public NativeMenu Menu { get; private set; }

        // Checkbox items for state tracking
        private NativeCheckboxItem _godModeItem;
        private NativeCheckboxItem _invisibleItem;
        private NativeCheckboxItem _noclipItem;
        private NativeCheckboxItem _superJumpItem;
        private NativeCheckboxItem _fastRunItem;
        private NativeCheckboxItem _fastSwimItem;
        private NativeCheckboxItem _unlimitedStaminaItem;
        private NativeCheckboxItem _noRagdollItem;
        private NativeCheckboxItem _neverWantedItem;
        private NativeCheckboxItem _everyoneIgnoreItem;
        private NativeCheckboxItem _stayInVehicleItem;
        private NativeCheckboxItem _frozenItem;

        // Ped Scenarios
        private static readonly string[] PlayerScenarios =
        {
            "WORLD_HUMAN_AA_COFFEE",
            "WORLD_HUMAN_AA_SMOKE",
            "WORLD_HUMAN_BINOCULARS",
            "WORLD_HUMAN_BUM_FREEWAY",
            "WORLD_HUMAN_CHEERING",
            "WORLD_HUMAN_CLIPBOARD",
            "WORLD_HUMAN_DRINKING",
            "WORLD_HUMAN_GUARD_STAND",
            "WORLD_HUMAN_HANG_OUT_STREET",
            "WORLD_HUMAN_PARTYING",
            "WORLD_HUMAN_PUSH_UPS",
            "WORLD_HUMAN_SIT_UPS",
            "WORLD_HUMAN_SMOKING",
            "WORLD_HUMAN_STAND_MOBILE",
            "WORLD_HUMAN_YOGA"
        };

        private static readonly string[] ScenarioNames =
        {
            "Drinking Coffee",
            "Smoking",
            "Binoculars",
            "Bum (Freeway)",
            "Cheering",
            "Clipboard",
            "Drinking",
            "Guard Stand",
            "Hang Out",
            "Partying",
            "Push Ups",
            "Sit Ups",
            "Smoking (Alt)",
            "Phone Call",
            "Yoga"
        };

        #endregion

        #region Constructor

        public PlayerMenu()
        {
            CreateMenu();
        }

        #endregion

        #region Menu Creation

        private void CreateMenu()
        {
            Menu = ThemeManager.CreateThemedMenu("comboom.sucht", "Player Options");

            // === HEALTH & ARMOR SECTION ===
            var healthHeader = new NativeItem("~b~=== Health & Armor ===", "Manage player health and armor")
            {
                Enabled = false
            };
            Menu.Add(healthHeader);

            // Heal Player
            var healItem = new NativeItem("Heal Player", "Restore health to maximum");
            healItem.Activated += (sender, args) =>
            {
                Main.PlayerManagerInstance.HealPlayer();
            };
            Menu.Add(healItem);

            // Give Armor
            var armorItem = new NativeItem("Give Armor", "Give full armor (100)");
            armorItem.Activated += (sender, args) =>
            {
                Main.PlayerManagerInstance.GiveArmor();
            };
            Menu.Add(armorItem);

            // Set Armor Type
            var armorLevels = new List<string> { "No Armor", "Light", "Standard", "Heavy", "Super", "Max" };
            var setArmorItem = new NativeListItem<string>("Set Armor Type", "Set the armor level", armorLevels.ToArray());
            setArmorItem.ItemChanged += (sender, args) =>
            {
                Game.PlayerPed.Armor = setArmorItem.SelectedIndex * 20;
                Main.ShowNotification($"~b~Armor set to: {armorLevels[setArmorItem.SelectedIndex]}");
            };
            Menu.Add(setArmorItem);

            // Clear Blood
            var clearBloodItem = new NativeItem("Clear Blood & Damage", "Clear blood and damage decals from player");
            clearBloodItem.Activated += (sender, args) =>
            {
                Game.PlayerPed.ClearBloodDamage();
                API.ClearPedDamageDecalByZone(Game.PlayerPed.Handle, 0, "ALL");
                API.ClearPedDamageDecalByZone(Game.PlayerPed.Handle, 1, "ALL");
                API.ClearPedDamageDecalByZone(Game.PlayerPed.Handle, 2, "ALL");
                API.ClearPedDamageDecalByZone(Game.PlayerPed.Handle, 3, "ALL");
                API.ClearPedDamageDecalByZone(Game.PlayerPed.Handle, 4, "ALL");
                API.ClearPedDamageDecalByZone(Game.PlayerPed.Handle, 5, "ALL");
                Main.ShowNotification("~g~Blood and damage cleared!");
            };
            Menu.Add(clearBloodItem);

            // Clean Player Clothes
            var cleanItem = new NativeItem("Clean Player Clothes", "Remove dirt and grime from clothes");
            cleanItem.Activated += (sender, args) =>
            {
                Game.PlayerPed.ClearBloodDamage();
                API.ClearPedEnvDirt(Game.PlayerPed.Handle);
                API.ClearPedWetness(Game.PlayerPed.Handle);
                Main.ShowNotification("~g~Player cleaned!");
            };
            Menu.Add(cleanItem);

            // Dry/Wet Player
            var dryItem = new NativeItem("Dry Player", "Make player dry");
            dryItem.Activated += (sender, args) =>
            {
                Game.PlayerPed.WetnessHeight = 0f;
                Main.ShowNotification("~g~Player is now dry!");
            };
            Menu.Add(dryItem);

            var wetItem = new NativeItem("Wet Player", "Make player wet");
            wetItem.Activated += (sender, args) =>
            {
                Game.PlayerPed.WetnessHeight = 2f;
                Main.ShowNotification("~b~Player is now wet!");
            };
            Menu.Add(wetItem);

            Menu.Add(new NativeSeparatorItem());

            // === PLAYER TOGGLES SECTION ===
            var togglesHeader = new NativeItem("~b~=== Player Toggles ===", "Toggle various player abilities")
            {
                Enabled = false
            };
            Menu.Add(togglesHeader);

            // God Mode
            _godModeItem = new NativeCheckboxItem("God Mode", "Toggle invincibility", false);
            _godModeItem.CheckboxChanged += (sender, args) =>
            {
                Main.PlayerManagerInstance.GodMode = _godModeItem.Checked;
                Game.PlayerPed.IsInvincible = _godModeItem.Checked;
                Main.ShowNotification(_godModeItem.Checked ? "~g~God Mode: ON" : "~r~God Mode: OFF");
            };
            Menu.Add(_godModeItem);

            // Invisible
            _invisibleItem = new NativeCheckboxItem("Invisible", "Toggle invisibility to yourself and others", false);
            _invisibleItem.CheckboxChanged += (sender, args) =>
            {
                Main.PlayerManagerInstance.Invisible = _invisibleItem.Checked;
                API.SetEntityVisible(Game.PlayerPed.Handle, !_invisibleItem.Checked, false);
                Main.ShowNotification(_invisibleItem.Checked ? "~g~Invisible: ON" : "~r~Invisible: OFF");
            };
            Menu.Add(_invisibleItem);

            // Unlimited Stamina
            _unlimitedStaminaItem = new NativeCheckboxItem("Unlimited Stamina", "Run forever without slowing down", false);
            _unlimitedStaminaItem.CheckboxChanged += (sender, args) =>
            {
                Main.PlayerManagerInstance.UnlimitedStamina = _unlimitedStaminaItem.Checked;
                API.StatSetInt((uint)API.GetHashKey("MP0_STAMINA"), _unlimitedStaminaItem.Checked ? 100 : 0, true);
                Main.ShowNotification(_unlimitedStaminaItem.Checked ? "~g~Unlimited Stamina: ON" : "~r~Unlimited Stamina: OFF");
            };
            Menu.Add(_unlimitedStaminaItem);

            // Fast Run
            _fastRunItem = new NativeCheckboxItem("Fast Run", "Run very fast", false);
            _fastRunItem.CheckboxChanged += (sender, args) =>
            {
                Main.PlayerManagerInstance.FastRun = _fastRunItem.Checked;
                API.SetRunSprintMultiplierForPlayer(Game.Player.Handle, _fastRunItem.Checked ? 1.49f : 1f);
                Main.ShowNotification(_fastRunItem.Checked ? "~g~Fast Run: ON" : "~r~Fast Run: OFF");
            };
            Menu.Add(_fastRunItem);

            // Fast Swim
            _fastSwimItem = new NativeCheckboxItem("Fast Swim", "Swim super fast", false);
            _fastSwimItem.CheckboxChanged += (sender, args) =>
            {
                API.SetSwimMultiplierForPlayer(Game.Player.Handle, _fastSwimItem.Checked ? 1.49f : 1f);
                Main.ShowNotification(_fastSwimItem.Checked ? "~g~Fast Swim: ON" : "~r~Fast Swim: OFF");
            };
            Menu.Add(_fastSwimItem);

            // Super Jump
            _superJumpItem = new NativeCheckboxItem("Super Jump", "Jump like a champion", false);
            _superJumpItem.CheckboxChanged += (sender, args) =>
            {
                Main.PlayerManagerInstance.SuperJump = _superJumpItem.Checked;
                Main.ShowNotification(_superJumpItem.Checked ? "~g~Super Jump: ON" : "~r~Super Jump: OFF");
            };
            Menu.Add(_superJumpItem);

            // No Ragdoll
            _noRagdollItem = new NativeCheckboxItem("No Ragdoll", "Prevent ragdolling, stay on your bike", false);
            _noRagdollItem.CheckboxChanged += (sender, args) =>
            {
                Main.PlayerManagerInstance.NoRagdoll = _noRagdollItem.Checked;
                API.SetPedCanRagdoll(Game.PlayerPed.Handle, !_noRagdollItem.Checked);
                Main.ShowNotification(_noRagdollItem.Checked ? "~g~No Ragdoll: ON" : "~r~No Ragdoll: OFF");
            };
            Menu.Add(_noRagdollItem);

            // Never Wanted
            _neverWantedItem = new NativeCheckboxItem("Never Wanted", "Disable all wanted levels", false);
            _neverWantedItem.CheckboxChanged += (sender, args) =>
            {
                API.SetMaxWantedLevel(_neverWantedItem.Checked ? 0 : 5);
                if (_neverWantedItem.Checked)
                {
                    Game.Player.WantedLevel = 0;
                }
                Main.ShowNotification(_neverWantedItem.Checked ? "~g~Never Wanted: ON" : "~r~Never Wanted: OFF");
            };
            Menu.Add(_neverWantedItem);

            // Everyone Ignore Player
            _everyoneIgnoreItem = new NativeCheckboxItem("Everyone Ignores You", "NPCs and police leave you alone", false);
            _everyoneIgnoreItem.CheckboxChanged += (sender, args) =>
            {
                API.SetEveryoneIgnorePlayer(Game.Player.Handle, _everyoneIgnoreItem.Checked);
                API.SetPoliceIgnorePlayer(Game.Player.Handle, _everyoneIgnoreItem.Checked);
                API.SetPlayerCanBeHassledByGangs(Game.Player.Handle, !_everyoneIgnoreItem.Checked);
                Main.ShowNotification(_everyoneIgnoreItem.Checked ? "~g~Everyone Ignores: ON" : "~r~Everyone Ignores: OFF");
            };
            Menu.Add(_everyoneIgnoreItem);

            // Stay In Vehicle
            _stayInVehicleItem = new NativeCheckboxItem("Stay In Vehicle", "NPCs can't drag you out of vehicles", false);
            _stayInVehicleItem.CheckboxChanged += (sender, args) =>
            {
                API.SetPedCanBeDraggedOut(Game.PlayerPed.Handle, !_stayInVehicleItem.Checked);
                Main.ShowNotification(_stayInVehicleItem.Checked ? "~g~Stay In Vehicle: ON" : "~r~Stay In Vehicle: OFF");
            };
            Menu.Add(_stayInVehicleItem);

            // Freeze Player
            _frozenItem = new NativeCheckboxItem("Freeze Player", "Freeze your current position", false);
            _frozenItem.CheckboxChanged += (sender, args) =>
            {
                API.FreezeEntityPosition(Game.PlayerPed.Handle, _frozenItem.Checked);
                Main.ShowNotification(_frozenItem.Checked ? "~g~Player Frozen: ON" : "~r~Player Frozen: OFF");
            };
            Menu.Add(_frozenItem);

            // Noclip
            _noclipItem = new NativeCheckboxItem("Noclip", "Toggle noclip mode (Use F2 for quick toggle)", false);
            _noclipItem.CheckboxChanged += (sender, args) =>
            {
                Main.PlayerManagerInstance.Noclip = _noclipItem.Checked;
                ApplyNoclipState();
            };
            Menu.Add(_noclipItem);

            Menu.Add(new NativeSeparatorItem());

            // === TELEPORT SECTION ===
            var teleportHeader = new NativeItem("~b~=== Teleport ===", "Teleport options")
            {
                Enabled = false
            };
            Menu.Add(teleportHeader);

            // Teleport to Waypoint
            var teleportItem = new NativeItem("Teleport to Waypoint", "Teleport to your map waypoint");
            teleportItem.Activated += async (sender, args) =>
            {
                await Main.PlayerManagerInstance.TeleportToWaypoint();
            };
            Menu.Add(teleportItem);

            // Teleport to Coords
            var teleportCoordsItem = new NativeItem("Teleport to Coordinates", "Enter coordinates to teleport");
            teleportCoordsItem.Activated += async (sender, args) =>
            {
                var input = await Main.GetUserInput("Enter coordinates (X, Y, Z)", "0, 0, 0", 50);
                if (!string.IsNullOrEmpty(input))
                {
                    try
                    {
                        var parts = input.Split(',');
                        if (parts.Length >= 3)
                        {
                            float x = float.Parse(parts[0].Trim());
                            float y = float.Parse(parts[1].Trim());
                            float z = float.Parse(parts[2].Trim());
                            Game.PlayerPed.Position = new Vector3(x, y, z);
                            Main.ShowNotification($"~g~Teleported to: {x}, {y}, {z}");
                        }
                    }
                    catch
                    {
                        Main.ShowNotification("~r~Invalid coordinates!");
                    }
                }
            };
            Menu.Add(teleportCoordsItem);

            Menu.Add(new NativeSeparatorItem());

            // === WANTED LEVEL SECTION ===
            var wantedHeader = new NativeItem("~b~=== Wanted Level ===", "Manage wanted level")
            {
                Enabled = false
            };
            Menu.Add(wantedHeader);

            // Clear Wanted Level
            var clearWantedItem = new NativeItem("Clear Wanted Level", "Remove all wanted stars");
            clearWantedItem.Activated += (sender, args) =>
            {
                Main.PlayerManagerInstance.ClearWantedLevel();
            };
            Menu.Add(clearWantedItem);

            // Set Wanted Level
            AddWantedLevelOptions();

            Menu.Add(new NativeSeparatorItem());

            // === SCENARIOS SECTION ===
            var scenarioHeader = new NativeItem("~b~=== Player Scenarios ===", "Play various animations/scenarios")
            {
                Enabled = false
            };
            Menu.Add(scenarioHeader);

            // Scenario List
            var scenarioItem = new NativeListItem<string>("Play Scenario", "Select and play a scenario", ScenarioNames);
            scenarioItem.Activated += (sender, args) =>
            {
                int index = scenarioItem.SelectedIndex;
                string scenario = PlayerScenarios[index];

                API.ClearPedTasks(Game.PlayerPed.Handle);
                API.TaskStartScenarioInPlace(Game.PlayerPed.Handle, scenario, 0, true);
                Main.ShowNotification($"~b~Playing: {ScenarioNames[index]}");
            };
            Menu.Add(scenarioItem);

            // Stop Scenario
            var stopScenarioItem = new NativeItem("Stop Scenario", "Force stop current scenario");
            stopScenarioItem.Activated += (sender, args) =>
            {
                API.ClearPedTasksImmediately(Game.PlayerPed.Handle);
                Main.ShowNotification("~r~Scenario stopped!");
            };
            Menu.Add(stopScenarioItem);

            Menu.Add(new NativeSeparatorItem());

            // === OTHER SECTION ===
            var otherHeader = new NativeItem("~b~=== Other ===", "Miscellaneous options")
            {
                Enabled = false
            };
            Menu.Add(otherHeader);

            // Suicide
            var suicideItem = new NativeItem("~r~Suicide", "~r~Kill yourself");
            suicideItem.Activated += (sender, args) =>
            {
                Main.PlayerManagerInstance.Suicide();
            };
            Menu.Add(suicideItem);

            // Reset Player State
            var resetItem = new NativeItem("Reset Player State", "Reset all player options to default");
            resetItem.Activated += (sender, args) =>
            {
                ResetAllOptions();
            };
            Menu.Add(resetItem);
        }

        private void AddWantedLevelOptions()
        {
            // Wanted Level List
            var wantedItem = new NativeListItem<int>("Set Wanted Level", "Set your wanted level", 0, 1, 2, 3, 4, 5);
            wantedItem.ItemChanged += (sender, args) =>
            {
                Game.Player.WantedLevel = wantedItem.SelectedItem;
                Main.ShowNotification($"~b~Wanted level set to: {wantedItem.SelectedItem}");
            };
            Menu.Add(wantedItem);
        }

        #endregion

        #region Helper Methods

        private void ApplyNoclipState()
        {
            var playerPed = Game.PlayerPed;

            if (Main.PlayerManagerInstance.Noclip)
            {
                playerPed.IsInvincible = true;
                playerPed.IsVisible = false;
                API.SetEntityCollision(playerPed.Handle, false, false);
                API.FreezeEntityPosition(playerPed.Handle, true);
                Main.ShowNotification("~g~Noclip: ON");
            }
            else
            {
                API.SetEntityCollision(playerPed.Handle, true, true);
                API.FreezeEntityPosition(playerPed.Handle, false);
                playerPed.IsInvincible = Main.PlayerManagerInstance.GodMode;
                playerPed.IsVisible = !Main.PlayerManagerInstance.Invisible;
                Main.ShowNotification("~r~Noclip: OFF");
            }
        }

        /// <summary>
        /// Reset all player options and update checkboxes
        /// </summary>
        public void ResetAllOptions()
        {
            Main.PlayerManagerInstance.ResetPlayerState();

            // Update all checkboxes
            _godModeItem.Checked = false;
            _invisibleItem.Checked = false;
            _noclipItem.Checked = false;
            _superJumpItem.Checked = false;
            _fastRunItem.Checked = false;
            _fastSwimItem.Checked = false;
            _unlimitedStaminaItem.Checked = false;
            _noRagdollItem.Checked = false;
            _neverWantedItem.Checked = false;
            _everyoneIgnoreItem.Checked = false;
            _stayInVehicleItem.Checked = false;
            _frozenItem.Checked = false;

            // Reset API states
            API.SetRunSprintMultiplierForPlayer(Game.Player.Handle, 1f);
            API.SetSwimMultiplierForPlayer(Game.Player.Handle, 1f);
            API.SetPedCanRagdoll(Game.PlayerPed.Handle, true);
            API.SetMaxWantedLevel(5);
            API.SetEveryoneIgnorePlayer(Game.Player.Handle, false);
            API.SetPoliceIgnorePlayer(Game.Player.Handle, false);
            API.SetPlayerCanBeHassledByGangs(Game.Player.Handle, true);
            API.SetPedCanBeDraggedOut(Game.PlayerPed.Handle, true);
            API.FreezeEntityPosition(Game.PlayerPed.Handle, false);

            Main.ShowNotification("~g~Player state reset!");
        }

        /// <summary>
        /// Sync noclip checkbox with manager state (for external toggles)
        /// </summary>
        public void SyncNoclipState()
        {
            _noclipItem.Checked = Main.PlayerManagerInstance.Noclip;
        }

        #endregion
    }
}
