using System;
using CitizenFX.Core;
using LemonUI.Menus;

using CBPSMenu.Client.Managers;
using CBPSMenu.Shared;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Player Options Menu - Similar to vMenu's PlayerOptions.cs
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
        private NativeCheckboxItem _unlimitedStaminaItem;
        private NativeCheckboxItem _noRagdollItem;

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
            _invisibleItem = new NativeCheckboxItem("Invisible", "Toggle invisibility", false);
            _invisibleItem.CheckboxChanged += (sender, args) =>
            {
                Main.PlayerManagerInstance.Invisible = _invisibleItem.Checked;
                Game.PlayerPed.IsVisible = !_invisibleItem.Checked;
                Main.ShowNotification(_invisibleItem.Checked ? "~g~Invisible: ON" : "~r~Invisible: OFF");
            };
            Menu.Add(_invisibleItem);

            // Noclip
            _noclipItem = new NativeCheckboxItem("Noclip", "Toggle noclip mode (Use F2 for quick toggle)", false);
            _noclipItem.CheckboxChanged += (sender, args) =>
            {
                Main.PlayerManagerInstance.Noclip = _noclipItem.Checked;
                ApplyNoclipState();
            };
            Menu.Add(_noclipItem);

            // Super Jump
            _superJumpItem = new NativeCheckboxItem("Super Jump", "Toggle super jump ability", false);
            _superJumpItem.CheckboxChanged += (sender, args) =>
            {
                Main.PlayerManagerInstance.SuperJump = _superJumpItem.Checked;
                Main.ShowNotification(_superJumpItem.Checked ? "~g~Super Jump: ON" : "~r~Super Jump: OFF");
            };
            Menu.Add(_superJumpItem);

            // Fast Run
            _fastRunItem = new NativeCheckboxItem("Fast Run", "Toggle fast running speed", false);
            _fastRunItem.CheckboxChanged += (sender, args) =>
            {
                Main.PlayerManagerInstance.FastRun = _fastRunItem.Checked;
                if (!_fastRunItem.Checked)
                {
                    CitizenFX.Core.Native.API.SetRunSprintMultiplierForPlayer(Game.Player.Handle, 1.0f);
                }
                Main.ShowNotification(_fastRunItem.Checked ? "~g~Fast Run: ON" : "~r~Fast Run: OFF");
            };
            Menu.Add(_fastRunItem);

            // Unlimited Stamina
            _unlimitedStaminaItem = new NativeCheckboxItem("Unlimited Stamina", "Toggle unlimited stamina", false);
            _unlimitedStaminaItem.CheckboxChanged += (sender, args) =>
            {
                Main.PlayerManagerInstance.UnlimitedStamina = _unlimitedStaminaItem.Checked;
                Main.ShowNotification(_unlimitedStaminaItem.Checked ? "~g~Unlimited Stamina: ON" : "~r~Unlimited Stamina: OFF");
            };
            Menu.Add(_unlimitedStaminaItem);

            // No Ragdoll
            _noRagdollItem = new NativeCheckboxItem("No Ragdoll", "Prevent player from ragdolling", false);
            _noRagdollItem.CheckboxChanged += (sender, args) =>
            {
                Main.PlayerManagerInstance.NoRagdoll = _noRagdollItem.Checked;
                Main.ShowNotification(_noRagdollItem.Checked ? "~g~No Ragdoll: ON" : "~r~No Ragdoll: OFF");
            };
            Menu.Add(_noRagdollItem);

            // Teleport to Waypoint
            var teleportItem = new NativeItem("Teleport to Waypoint", "Teleport to your map waypoint");
            teleportItem.Activated += async (sender, args) =>
            {
                await Main.PlayerManagerInstance.TeleportToWaypoint();
            };
            Menu.Add(teleportItem);

            // Clear Wanted Level
            var clearWantedItem = new NativeItem("Clear Wanted Level", "Remove all wanted stars");
            clearWantedItem.Activated += (sender, args) =>
            {
                Main.PlayerManagerInstance.ClearWantedLevel();
            };
            Menu.Add(clearWantedItem);

            // Set Wanted Level submenu
            AddWantedLevelOptions();

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
                CitizenFX.Core.Native.API.SetEntityCollision(playerPed.Handle, false, false);
                CitizenFX.Core.Native.API.FreezeEntityPosition(playerPed.Handle, true);
                Main.ShowNotification("~g~Noclip: ON");
            }
            else
            {
                CitizenFX.Core.Native.API.SetEntityCollision(playerPed.Handle, true, true);
                CitizenFX.Core.Native.API.FreezeEntityPosition(playerPed.Handle, false);
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
            _unlimitedStaminaItem.Checked = false;
            _noRagdollItem.Checked = false;
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
