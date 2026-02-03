using System;
using System.Drawing;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using LemonUI.Menus;

using CBPSMenu.Shared;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Settings Menu - Menu configuration and keybindings
    /// Similar to vMenu's MiscSettings.cs
    /// </summary>
    public class SettingsMenu
    {
        #region Variables

        public NativeMenu Menu { get; private set; }

        #endregion

        #region Constructor

        public SettingsMenu()
        {
            CreateMenu();
        }

        #endregion

        #region Menu Creation

        private void CreateMenu()
        {
            Menu = ThemeManager.CreateThemedMenu("comboom.sucht", "Settings");

            // Keybindings Header
            var keybindHeader = new NativeItem("~b~=== Keybindings ===", "Current menu keybindings");
            keybindHeader.Enabled = false;
            Menu.Add(keybindHeader);

            // Show Keybindings
            var showKeybindsItem = new NativeItem("View Keybindings", "Show all menu keybindings");
            showKeybindsItem.Activated += (sender, args) =>
            {
                Main.ShowNotification($"~b~Menu: {Main.MenuToggleKey} | Noclip: {Main.NoClipKey} | Reset: {Main.ResetKey}");
            };
            Menu.Add(showKeybindsItem);

            // Add separator
            Menu.Add(new NativeSeparatorItem());

            // Theme Header
            var themeHeader = new NativeItem("~b~=== Theme Settings ===", "Customize menu appearance");
            themeHeader.Enabled = false;
            Menu.Add(themeHeader);

            // Theme Selection (Nord is forced as per requirements)
            var themeItem = new NativeListItem<string>("Menu Theme", "Change menu color theme (Nord forced)",
                "Nord (Default)", "Blue", "Red", "Green", "Purple", "Orange", "Dark");
            themeItem.ItemChanged += (sender, args) =>
            {
                ApplyTheme(themeItem.SelectedItem);
            };
            Menu.Add(themeItem);

            // Reset Theme to Nord
            var resetThemeItem = new NativeItem("Reset to Nord Theme", "Reset theme to default Nord colors");
            resetThemeItem.Activated += (sender, args) =>
            {
                ThemeManager.SetNordTheme();
                RefreshAllMenuThemes();
                Main.ShowNotification("~g~Theme reset to Nord!");
            };
            Menu.Add(resetThemeItem);

            // Add separator
            Menu.Add(new NativeSeparatorItem());

            // Debug Header
            var debugHeader = new NativeItem("~b~=== Debug Options ===", "Developer and debug options");
            debugHeader.Enabled = false;
            Menu.Add(debugHeader);

            // Toggle Debug Mode
            var debugItem = new NativeCheckboxItem("Debug Mode", "Enable debug logging", Main.DebugMode);
            debugItem.CheckboxChanged += (sender, args) =>
            {
                Main.DebugMode = debugItem.Checked;
                Main.ShowNotification(Main.DebugMode ? "~g~Debug Mode: ON" : "~r~Debug Mode: OFF");
            };
            Menu.Add(debugItem);

            // Show Player Coords
            var coordsItem = new NativeItem("Show Coordinates", "Display your current position");
            coordsItem.Activated += (sender, args) =>
            {
                var pos = Game.PlayerPed.Position;
                Main.ShowNotification($"~b~Position: X: {pos.X:F2}, Y: {pos.Y:F2}, Z: {pos.Z:F2}");
                Debug.WriteLine($"[comboom.sucht] Player position: {pos.X:F2}, {pos.Y:F2}, {pos.Z:F2}");
            };
            Menu.Add(coordsItem);

            // Copy Coords to Clipboard
            var copyCoords = new NativeItem("Copy Coordinates", "Copy current position to clipboard (console)");
            copyCoords.Activated += (sender, args) =>
            {
                var pos = Game.PlayerPed.Position;
                var coordString = $"vector3({pos.X:F4}, {pos.Y:F4}, {pos.Z:F4})";
                Debug.WriteLine($"[comboom.sucht] Coordinates: {coordString}");
                Main.ShowNotification("~g~Coordinates copied to F8 console!");
            };
            Menu.Add(copyCoords);

            // Add separator
            Menu.Add(new NativeSeparatorItem());

            // Info Header
            var infoHeader = new NativeItem("~b~=== About ===", "Menu information");
            infoHeader.Enabled = false;
            Menu.Add(infoHeader);

            // Version Info
            var versionItem = new NativeItem("comboom.sucht Menu v1.0.0", "A vMenu-style menu using LemonUI");
            versionItem.Enabled = false;
            Menu.Add(versionItem);

            // Credits
            var creditsItem = new NativeItem("Credits", "Show credits");
            creditsItem.Activated += (sender, args) =>
            {
                Main.ShowNotification("~b~comboom.sucht Menu~s~ - A vMenu clone using LemonUI\n~y~Based on vMenu by TomGrobbe");
            };
            Menu.Add(creditsItem);

            // GitHub
            var githubItem = new NativeItem("GitHub Repository", "View source code");
            githubItem.Activated += (sender, args) =>
            {
                Main.ShowNotification("~b~Check the resource folder for the GitHub link!");
            };
            Menu.Add(githubItem);
        }

        #endregion

        #region Theme Management

        private void ApplyTheme(string themeName)
        {
            switch (themeName)
            {
                case "Nord (Default)":
                    ThemeManager.SetNordTheme();
                    break;
                case "Blue":
                    ThemeManager.SetCustomTheme(
                        Color.FromArgb(255, 0, 120, 215),
                        Color.FromArgb(255, 0, 150, 255),
                        Color.FromArgb(255, 255, 255, 255)
                    );
                    break;
                case "Red":
                    ThemeManager.SetCustomTheme(
                        Color.FromArgb(255, 220, 20, 60),
                        Color.FromArgb(255, 255, 50, 50),
                        Color.FromArgb(255, 255, 255, 255)
                    );
                    break;
                case "Green":
                    ThemeManager.SetCustomTheme(
                        Color.FromArgb(255, 34, 139, 34),
                        Color.FromArgb(255, 50, 180, 50),
                        Color.FromArgb(255, 255, 255, 255)
                    );
                    break;
                case "Purple":
                    ThemeManager.SetCustomTheme(
                        Color.FromArgb(255, 138, 43, 226),
                        Color.FromArgb(255, 180, 100, 255),
                        Color.FromArgb(255, 255, 255, 255)
                    );
                    break;
                case "Orange":
                    ThemeManager.SetCustomTheme(
                        Color.FromArgb(255, 255, 140, 0),
                        Color.FromArgb(255, 255, 180, 50),
                        Color.FromArgb(255, 0, 0, 0)
                    );
                    break;
                case "Dark":
                    ThemeManager.SetCustomTheme(
                        Color.FromArgb(255, 30, 30, 30),
                        Color.FromArgb(255, 60, 60, 60),
                        Color.FromArgb(255, 255, 255, 255)
                    );
                    break;
            }

            RefreshAllMenuThemes();
            Main.ShowNotification($"~g~Theme changed to: {themeName}");
        }

        private void RefreshAllMenuThemes()
        {
            // Apply theme to all menus
            ThemeManager.ApplyThemeColors(Main.MainMenu);
            
            if (Main.PlayerMenuInstance?.Menu != null)
                ThemeManager.ApplyThemeColors(Main.PlayerMenuInstance.Menu);
            
            if (Main.VehicleMenuInstance?.Menu != null)
                ThemeManager.ApplyThemeColors(Main.VehicleMenuInstance.Menu);
            
            if (Main.VehicleSpawnerMenuInstance?.Menu != null)
                ThemeManager.ApplyThemeColors(Main.VehicleSpawnerMenuInstance.Menu);
            
            if (Main.SavedVehiclesMenuInstance?.Menu != null)
                ThemeManager.ApplyThemeColors(Main.SavedVehiclesMenuInstance.Menu);
            
            if (Main.WeaponMenuInstance?.Menu != null)
                ThemeManager.ApplyThemeColors(Main.WeaponMenuInstance.Menu);
            
            if (Main.WorldMenuInstance?.Menu != null)
                ThemeManager.ApplyThemeColors(Main.WorldMenuInstance.Menu);
            
            if (Main.VoiceMenuInstance?.Menu != null)
                ThemeManager.ApplyThemeColors(Main.VoiceMenuInstance.Menu);
            
            if (Main.RaceMenuInstance?.Menu != null)
                ThemeManager.ApplyThemeColors(Main.RaceMenuInstance.Menu);
            
            if (Menu != null)
                ThemeManager.ApplyThemeColors(Menu);
        }

        #endregion
    }
}
