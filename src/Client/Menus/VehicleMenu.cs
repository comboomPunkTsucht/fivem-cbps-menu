using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using LemonUI.Menus;

using CBPSMenu.Client.Managers;
using CBPSMenu.Shared;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Vehicle Options Menu - Similar to vMenu's VehicleOptions.cs
    /// </summary>
    public class VehicleMenu
    {
        #region Variables

        public NativeMenu Menu { get; private set; }

        // Checkbox items for state tracking
        private NativeCheckboxItem _vehicleInvincibleItem;
        private NativeCheckboxItem _engineAlwaysOnItem;
        private NativeCheckboxItem _noSirenItem;
        private NativeCheckboxItem _noBikeFallItem;

        #endregion

        #region Constructor

        public VehicleMenu()
        {
            CreateMenu();
        }

        #endregion

        #region Menu Creation

        private void CreateMenu()
        {
            Menu = ThemeManager.CreateThemedMenu("comboom.sucht", "Vehicle Options");

            // Repair Vehicle
            var repairItem = new NativeItem("Repair Vehicle", "Fix and clean your current vehicle");
            repairItem.Activated += (sender, args) =>
            {
                Main.VehicleManagerInstance.RepairVehicle();
            };
            Menu.Add(repairItem);

            // Clean Vehicle
            var cleanItem = new NativeItem("Clean Vehicle", "Remove all dirt from your vehicle");
            cleanItem.Activated += (sender, args) =>
            {
                Main.VehicleManagerInstance.CleanVehicle();
            };
            Menu.Add(cleanItem);

            // Flip Vehicle
            var flipItem = new NativeItem("Flip Vehicle", "Flip your vehicle right-side up");
            flipItem.Activated += (sender, args) =>
            {
                Main.VehicleManagerInstance.FlipVehicle();
            };
            Menu.Add(flipItem);

            // Vehicle Invincible
            _vehicleInvincibleItem = new NativeCheckboxItem("Vehicle Invincible", "Make your vehicle indestructible", false);
            _vehicleInvincibleItem.CheckboxChanged += (sender, args) =>
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle != null)
                {
                    Main.VehicleManagerInstance.VehicleInvincible = _vehicleInvincibleItem.Checked;
                    vehicle.IsInvincible = _vehicleInvincibleItem.Checked;
                    vehicle.CanTiresBurst = !_vehicleInvincibleItem.Checked;
                    Main.ShowNotification(_vehicleInvincibleItem.Checked ? "~g~Vehicle Invincible: ON" : "~r~Vehicle Invincible: OFF");
                }
                else
                {
                    Main.ShowNotification("~r~You are not in a vehicle!");
                    _vehicleInvincibleItem.Checked = false;
                }
            };
            Menu.Add(_vehicleInvincibleItem);

            // Engine Always On
            _engineAlwaysOnItem = new NativeCheckboxItem("Engine Always On", "Keep the engine running when exiting", false);
            _engineAlwaysOnItem.CheckboxChanged += (sender, args) =>
            {
                Main.VehicleManagerInstance.VehicleEngineAlwaysOn = _engineAlwaysOnItem.Checked;
                Main.ShowNotification(_engineAlwaysOnItem.Checked ? "~g~Engine Always On: ON" : "~r~Engine Always On: OFF");
            };
            Menu.Add(_engineAlwaysOnItem);

            // No Siren
            _noSirenItem = new NativeCheckboxItem("Mute Siren", "Mute the siren sound (lights still work)", false);
            _noSirenItem.CheckboxChanged += (sender, args) =>
            {
                Main.VehicleManagerInstance.VehicleNoSiren = _noSirenItem.Checked;
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle != null)
                {
                    API.SetVehicleHasMutedSirens(vehicle.Handle, _noSirenItem.Checked);
                }
                Main.ShowNotification(_noSirenItem.Checked ? "~g~Siren Muted: ON" : "~r~Siren Muted: OFF");
            };
            Menu.Add(_noSirenItem);

            // No Bike Fall
            _noBikeFallItem = new NativeCheckboxItem("No Bike Fall", "Prevent falling off motorcycles", false);
            _noBikeFallItem.CheckboxChanged += (sender, args) =>
            {
                Main.VehicleManagerInstance.VehicleNoBikeFall = _noBikeFallItem.Checked;
                if (_noBikeFallItem.Checked)
                {
                    API.SetPedCanBeKnockedOffVehicle(Game.PlayerPed.Handle, 1);
                }
                else
                {
                    API.SetPedCanBeKnockedOffVehicle(Game.PlayerPed.Handle, 0);
                }
                Main.ShowNotification(_noBikeFallItem.Checked ? "~g~No Bike Fall: ON" : "~r~No Bike Fall: OFF");
            };
            Menu.Add(_noBikeFallItem);

            // Add color options
            AddColorOptions();

            // Add vehicle upgrades
            AddUpgradeOptions();

            // Add doors control
            AddDoorOptions();

            // Delete Vehicle
            var deleteItem = new NativeItem("~r~Delete Vehicle", "~r~Delete your current vehicle");
            deleteItem.Activated += (sender, args) =>
            {
                Main.VehicleManagerInstance.DeleteVehicle();
            };
            Menu.Add(deleteItem);
        }

        private void AddColorOptions()
        {
            // Primary Color
            var primaryColorItem = new NativeListItem<string>("Primary Color", "Change primary color",
                "Black", "White", "Red", "Blue", "Green", "Yellow", "Orange", "Purple", "Pink", "Gray", "Chrome");
            primaryColorItem.ItemChanged += (sender, args) =>
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle != null)
                {
                    int colorIndex = GetColorIndex(primaryColorItem.SelectedItem);
                    int primary = 0, secondary = 0;
                    API.GetVehicleColours(vehicle.Handle, ref primary, ref secondary);
                    API.SetVehicleColours(vehicle.Handle, colorIndex, secondary);
                    Main.ShowNotification($"~g~Primary color set to: {primaryColorItem.SelectedItem}");
                }
            };
            Menu.Add(primaryColorItem);

            // Secondary Color
            var secondaryColorItem = new NativeListItem<string>("Secondary Color", "Change secondary color",
                "Black", "White", "Red", "Blue", "Green", "Yellow", "Orange", "Purple", "Pink", "Gray", "Chrome");
            secondaryColorItem.ItemChanged += (sender, args) =>
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle != null)
                {
                    int colorIndex = GetColorIndex(secondaryColorItem.SelectedItem);
                    int primary = 0, secondary = 0;
                    API.GetVehicleColours(vehicle.Handle, ref primary, ref secondary);
                    API.SetVehicleColours(vehicle.Handle, primary, colorIndex);
                    Main.ShowNotification($"~g~Secondary color set to: {secondaryColorItem.SelectedItem}");
                }
            };
            Menu.Add(secondaryColorItem);
        }

        private void AddUpgradeOptions()
        {
            // Max Performance
            var maxPerfItem = new NativeItem("Max Performance Upgrades", "Apply maximum performance upgrades");
            maxPerfItem.Activated += (sender, args) =>
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle != null)
                {
                    vehicle.Mods.InstallModKit();
                    // Engine
                    API.SetVehicleMod(vehicle.Handle, 11, API.GetNumVehicleMods(vehicle.Handle, 11) - 1, false);
                    // Brakes
                    API.SetVehicleMod(vehicle.Handle, 12, API.GetNumVehicleMods(vehicle.Handle, 12) - 1, false);
                    // Transmission
                    API.SetVehicleMod(vehicle.Handle, 13, API.GetNumVehicleMods(vehicle.Handle, 13) - 1, false);
                    // Suspension
                    API.SetVehicleMod(vehicle.Handle, 15, API.GetNumVehicleMods(vehicle.Handle, 15) - 1, false);
                    // Turbo
                    API.ToggleVehicleMod(vehicle.Handle, 18, true);
                    Main.ShowNotification("~g~Max performance upgrades applied!");
                }
                else
                {
                    Main.ShowNotification("~r~You are not in a vehicle!");
                }
            };
            Menu.Add(maxPerfItem);

            // Max Visual
            var maxVisualItem = new NativeItem("Max Visual Upgrades", "Apply maximum visual upgrades");
            maxVisualItem.Activated += (sender, args) =>
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle != null)
                {
                    vehicle.Mods.InstallModKit();
                    // Spoiler
                    API.SetVehicleMod(vehicle.Handle, 0, API.GetNumVehicleMods(vehicle.Handle, 0) - 1, false);
                    // Front Bumper
                    API.SetVehicleMod(vehicle.Handle, 1, API.GetNumVehicleMods(vehicle.Handle, 1) - 1, false);
                    // Rear Bumper
                    API.SetVehicleMod(vehicle.Handle, 2, API.GetNumVehicleMods(vehicle.Handle, 2) - 1, false);
                    // Side Skirt
                    API.SetVehicleMod(vehicle.Handle, 3, API.GetNumVehicleMods(vehicle.Handle, 3) - 1, false);
                    // Exhaust
                    API.SetVehicleMod(vehicle.Handle, 4, API.GetNumVehicleMods(vehicle.Handle, 4) - 1, false);
                    // Xenon Lights
                    API.ToggleVehicleMod(vehicle.Handle, 22, true);
                    Main.ShowNotification("~g~Max visual upgrades applied!");
                }
                else
                {
                    Main.ShowNotification("~r~You are not in a vehicle!");
                }
            };
            Menu.Add(maxVisualItem);

            // Window Tint
            var windowTintItem = new NativeListItem<string>("Window Tint", "Change window tint",
                "None", "Pure Black", "Dark Smoke", "Light Smoke", "Limo", "Green");
            windowTintItem.ItemChanged += (sender, args) =>
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle != null)
                {
                    int tintIndex = GetWindowTintIndex(windowTintItem.SelectedItem);
                    API.SetVehicleWindowTint(vehicle.Handle, tintIndex);
                    Main.ShowNotification($"~g~Window tint set to: {windowTintItem.SelectedItem}");
                }
            };
            Menu.Add(windowTintItem);
        }

        private void AddDoorOptions()
        {
            // Open All Doors
            var openDoorsItem = new NativeItem("Open All Doors", "Open all vehicle doors");
            openDoorsItem.Activated += (sender, args) =>
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle != null)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        API.SetVehicleDoorOpen(vehicle.Handle, i, false, false);
                    }
                    Main.ShowNotification("~g~All doors opened!");
                }
                else
                {
                    Main.ShowNotification("~r~You are not in a vehicle!");
                }
            };
            Menu.Add(openDoorsItem);

            // Close All Doors
            var closeDoorsItem = new NativeItem("Close All Doors", "Close all vehicle doors");
            closeDoorsItem.Activated += (sender, args) =>
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle != null)
                {
                    API.SetVehicleDoorsShut(vehicle.Handle, false);
                    Main.ShowNotification("~g~All doors closed!");
                }
                else
                {
                    Main.ShowNotification("~r~You are not in a vehicle!");
                }
            };
            Menu.Add(closeDoorsItem);
        }

        #endregion

        #region Helper Methods

        private int GetColorIndex(string colorName)
        {
            switch (colorName)
            {
                case "Black": return 0;
                case "White": return 1;
                case "Red": return 27;
                case "Blue": return 64;
                case "Green": return 53;
                case "Yellow": return 42;
                case "Orange": return 38;
                case "Purple": return 145;
                case "Pink": return 135;
                case "Gray": return 7;
                case "Chrome": return 120;
                default: return 0;
            }
        }

        private int GetWindowTintIndex(string tintName)
        {
            switch (tintName)
            {
                case "None": return 0;
                case "Pure Black": return 1;
                case "Dark Smoke": return 2;
                case "Light Smoke": return 3;
                case "Limo": return 5;
                case "Green": return 6;
                default: return 0;
            }
        }

        #endregion
    }
}
