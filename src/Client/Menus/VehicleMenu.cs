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
    /// Vehicle Options Menu - Enhanced with vMenu features
    /// Based on vMenu/vMenu/menus/VehicleOptions.cs
    /// </summary>
    public class VehicleMenu
    {
        #region Variables

        public NativeMenu Menu { get; private set; }

        // Checkbox items for state tracking
        private NativeCheckboxItem _godModeItem;
        private NativeCheckboxItem _neverDirtyItem;
        private NativeCheckboxItem _engineAlwaysOnItem;
        private NativeCheckboxItem _freezeVehicleItem;
        private NativeCheckboxItem _noSirenItem;
        private NativeCheckboxItem _noBikeHelmetItem;
        private NativeCheckboxItem _bikeSeatbeltItem;
        private NativeCheckboxItem _highbeamsOnHonkItem;
        private NativeCheckboxItem _infiniteFuelItem;
        private NativeCheckboxItem _torqueMultiplierItem;
        private NativeCheckboxItem _powerMultiplierItem;

        // Multiplier amounts
        private float _torqueMultiplierAmount = 2f;
        private float _powerMultiplierAmount = 2f;

        // Submenus
        private NativeMenu _doorsMenu;
        private NativeMenu _colorsMenu;

        // Door states
        private static readonly string[] DoorNames = { "Front Left", "Front Right", "Rear Left", "Rear Right", "Hood", "Trunk", "All Doors" };

        // Colors
        private static readonly string[] VehicleColors = { "Black", "White", "Red", "Blue", "Green", "Yellow", "Orange", "Purple", "Pink", "Silver", "Gold" };
        private static readonly int[] ColorIndices = { 0, 1, 27, 64, 55, 88, 38, 145, 135, 4, 37 };

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

            // === REPAIR & CLEAN SECTION ===
            var repairHeader = new NativeItem("~b~=== Repair & Clean ===", "Repair and clean vehicle")
            {
                Enabled = false
            };
            Menu.Add(repairHeader);

            // Repair Vehicle
            var repairItem = new NativeItem("Repair Vehicle", "Fully repair your vehicle");
            repairItem.Activated += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    veh.Repair();
                    Main.ShowNotification("~g~Vehicle repaired!");
                }
                else
                {
                    Main.ShowNotification("~r~You are not in a vehicle!");
                }
            };
            Menu.Add(repairItem);

            // Wash Vehicle
            var washItem = new NativeItem("Wash Vehicle", "Clean your vehicle");
            washItem.Activated += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    veh.Wash();
                    Main.ShowNotification("~g~Vehicle washed!");
                }
                else
                {
                    Main.ShowNotification("~r~You are not in a vehicle!");
                }
            };
            Menu.Add(washItem);

            // Set Dirt Level
            var dirtLevels = new List<string> { "Clean", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };
            var dirtLevelItem = new NativeListItem<string>("Set Dirt Level", "Set the dirt level on your vehicle", dirtLevels.ToArray());
            dirtLevelItem.Activated += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    veh.DirtLevel = dirtLevelItem.SelectedIndex;
                    Main.ShowNotification($"~b~Dirt level set to: {dirtLevelItem.SelectedIndex}");
                }
            };
            Menu.Add(dirtLevelItem);

            Menu.Add(new NativeSeparatorItem());

            // === VEHICLE TOGGLES SECTION ===
            var togglesHeader = new NativeItem("~b~=== Vehicle Toggles ===", "Toggle various vehicle options")
            {
                Enabled = false
            };
            Menu.Add(togglesHeader);

            // God Mode
            _godModeItem = new NativeCheckboxItem("Vehicle God Mode", "Make your vehicle invincible", false);
            _godModeItem.CheckboxChanged += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    veh.IsInvincible = _godModeItem.Checked;
                    veh.CanBeVisiblyDamaged = !_godModeItem.Checked;
                    veh.CanTiresBurst = !_godModeItem.Checked;
                    veh.CanWheelsBreak = !_godModeItem.Checked;
                    Main.ShowNotification(_godModeItem.Checked ? "~g~Vehicle God Mode: ON" : "~r~Vehicle God Mode: OFF");
                }
            };
            Menu.Add(_godModeItem);

            // Keep Vehicle Clean
            _neverDirtyItem = new NativeCheckboxItem("Keep Vehicle Clean", "Your vehicle stays clean", false);
            _neverDirtyItem.CheckboxChanged += (sender, args) =>
            {
                Main.ShowNotification(_neverDirtyItem.Checked ? "~g~Keep Clean: ON" : "~r~Keep Clean: OFF");
            };
            Menu.Add(_neverDirtyItem);

            // Engine Always On
            _engineAlwaysOnItem = new NativeCheckboxItem("Engine Always On", "Keep engine running when you exit", false);
            _engineAlwaysOnItem.CheckboxChanged += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    API.SetVehicleEngineOn(veh.Handle, true, true, _engineAlwaysOnItem.Checked);
                }
                Main.ShowNotification(_engineAlwaysOnItem.Checked ? "~g~Engine Always On: ON" : "~r~Engine Always On: OFF");
            };
            Menu.Add(_engineAlwaysOnItem);

            // Freeze Vehicle
            _freezeVehicleItem = new NativeCheckboxItem("Freeze Vehicle", "Lock vehicle in place", false);
            _freezeVehicleItem.CheckboxChanged += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    API.FreezeEntityPosition(veh.Handle, _freezeVehicleItem.Checked);
                    Main.ShowNotification(_freezeVehicleItem.Checked ? "~g~Vehicle Frozen: ON" : "~r~Vehicle Frozen: OFF");
                }
            };
            Menu.Add(_freezeVehicleItem);

            // No Siren
            _noSirenItem = new NativeCheckboxItem("Disable Siren", "Disable vehicle siren", false);
            _noSirenItem.CheckboxChanged += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    veh.IsSirenSilent = _noSirenItem.Checked;
                    Main.ShowNotification(_noSirenItem.Checked ? "~g~Siren Disabled" : "~r~Siren Enabled");
                }
            };
            Menu.Add(_noSirenItem);

            // No Bike Helmet
            _noBikeHelmetItem = new NativeCheckboxItem("No Bike Helmet", "Don't auto-equip helmet on bikes", false);
            _noBikeHelmetItem.CheckboxChanged += (sender, args) =>
            {
                API.SetPedHelmet(Game.PlayerPed.Handle, !_noBikeHelmetItem.Checked);
                Main.ShowNotification(_noBikeHelmetItem.Checked ? "~g~No Helmet: ON" : "~r~No Helmet: OFF");
            };
            Menu.Add(_noBikeHelmetItem);

            // Bike Seatbelt
            _bikeSeatbeltItem = new NativeCheckboxItem("Bike Seatbelt", "Can't be knocked off bikes", false);
            _bikeSeatbeltItem.CheckboxChanged += (sender, args) =>
            {
                API.SetPedCanBeKnockedOffVehicle(Game.PlayerPed.Handle, _bikeSeatbeltItem.Checked ? 1 : 0);
                Main.ShowNotification(_bikeSeatbeltItem.Checked ? "~g~Bike Seatbelt: ON" : "~r~Bike Seatbelt: OFF");
            };
            Menu.Add(_bikeSeatbeltItem);

            // Flash Highbeams on Honk
            _highbeamsOnHonkItem = new NativeCheckboxItem("Flash Highbeams On Honk", "Flash lights when honking", false);
            _highbeamsOnHonkItem.CheckboxChanged += (sender, args) =>
            {
                Main.ShowNotification(_highbeamsOnHonkItem.Checked ? "~g~Highbeams on Honk: ON" : "~r~Highbeams on Honk: OFF");
            };
            Menu.Add(_highbeamsOnHonkItem);

            // Infinite Fuel (for FRFuel)
            _infiniteFuelItem = new NativeCheckboxItem("Infinite Fuel", "Infinite fuel (requires fuel script)", false);
            _infiniteFuelItem.CheckboxChanged += (sender, args) =>
            {
                BaseScript.TriggerEvent("vMenu:InfiniteFuelToggled", _infiniteFuelItem.Checked);
                Main.ShowNotification(_infiniteFuelItem.Checked ? "~g~Infinite Fuel: ON" : "~r~Infinite Fuel: OFF");
            };
            Menu.Add(_infiniteFuelItem);

            Menu.Add(new NativeSeparatorItem());

            // === POWER SECTION ===
            var powerHeader = new NativeItem("~b~=== Power Options ===", "Modify vehicle power and torque")
            {
                Enabled = false
            };
            Menu.Add(powerHeader);

            // Torque Multiplier
            _torqueMultiplierItem = new NativeCheckboxItem("Enable Torque Multiplier", "Enable torque boost", false);
            _torqueMultiplierItem.CheckboxChanged += (sender, args) =>
            {
                Main.ShowNotification(_torqueMultiplierItem.Checked ? "~g~Torque Multiplier: ON" : "~r~Torque Multiplier: OFF");
            };
            Menu.Add(_torqueMultiplierItem);

            var torqueValues = new List<string> { "x2", "x4", "x8", "x16", "x32", "x64", "x128" };
            var torqueListItem = new NativeListItem<string>("Torque Amount", "Set torque multiplier amount", torqueValues.ToArray());
            torqueListItem.ItemChanged += (sender, args) =>
            {
                _torqueMultiplierAmount = (float)Math.Pow(2, torqueListItem.SelectedIndex + 1);
                Main.ShowNotification($"~b~Torque multiplier: x{_torqueMultiplierAmount}");
            };
            Menu.Add(torqueListItem);

            // Power Multiplier
            _powerMultiplierItem = new NativeCheckboxItem("Enable Power Multiplier", "Enable power boost", false);
            _powerMultiplierItem.CheckboxChanged += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    API.SetVehicleEnginePowerMultiplier(veh.Handle, _powerMultiplierItem.Checked ? _powerMultiplierAmount : 1f);
                }
                Main.ShowNotification(_powerMultiplierItem.Checked ? "~g~Power Multiplier: ON" : "~r~Power Multiplier: OFF");
            };
            Menu.Add(_powerMultiplierItem);

            var powerValues = new List<string> { "x2", "x4", "x8", "x16", "x32", "x64", "x128" };
            var powerListItem = new NativeListItem<string>("Power Amount", "Set power multiplier amount", powerValues.ToArray());
            powerListItem.ItemChanged += (sender, args) =>
            {
                _powerMultiplierAmount = (float)Math.Pow(2, powerListItem.SelectedIndex + 1);
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null && _powerMultiplierItem.Checked)
                {
                    API.SetVehicleEnginePowerMultiplier(veh.Handle, _powerMultiplierAmount);
                }
                Main.ShowNotification($"~b~Power multiplier: x{_powerMultiplierAmount}");
            };
            Menu.Add(powerListItem);

            Menu.Add(new NativeSeparatorItem());

            // === VEHICLE ACTIONS SECTION ===
            var actionsHeader = new NativeItem("~b~=== Vehicle Actions ===", "Various vehicle actions")
            {
                Enabled = false
            };
            Menu.Add(actionsHeader);

            // Toggle Engine
            var toggleEngineItem = new NativeItem("Toggle Engine On/Off", "Turn engine on or off");
            toggleEngineItem.Activated += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    bool currentState = API.GetIsVehicleEngineRunning(veh.Handle);
                    API.SetVehicleEngineOn(veh.Handle, !currentState, false, true);
                    Main.ShowNotification(!currentState ? "~g~Engine: ON" : "~r~Engine: OFF");
                }
            };
            Menu.Add(toggleEngineItem);

            // Flip Vehicle
            var flipItem = new NativeItem("Flip Vehicle", "Set vehicle on all 4 wheels");
            flipItem.Activated += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    API.SetVehicleOnGroundProperly(veh.Handle);
                    Main.ShowNotification("~g~Vehicle flipped!");
                }
            };
            Menu.Add(flipItem);

            // Toggle Alarm
            var alarmItem = new NativeItem("Toggle Vehicle Alarm", "Start or stop the alarm");
            alarmItem.Activated += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    bool alarmActive = API.IsVehicleAlarmActivated(veh.Handle);
                    if (alarmActive)
                    {
                        API.SetVehicleAlarm(veh.Handle, false);
                        API.SetVehicleAlarmTimeLeft(veh.Handle, 0);
                    }
                    else
                    {
                        API.SetVehicleAlarm(veh.Handle, true);
                        API.SetVehicleAlarmTimeLeft(veh.Handle, int.MaxValue);
                        API.StartVehicleAlarm(veh.Handle);
                    }
                    Main.ShowNotification(!alarmActive ? "~y~Alarm: ON" : "~g~Alarm: OFF");
                }
            };
            Menu.Add(alarmItem);

            // Cycle Seats
            var cycleSeatsItem = new NativeItem("Cycle Through Seats", "Move to next available seat");
            cycleSeatsItem.Activated += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    CycleThroughSeats(veh);
                }
            };
            Menu.Add(cycleSeatsItem);

            // Destroy Engine
            var destroyEngineItem = new NativeItem("~r~Destroy Engine", "~r~Damage the engine beyond repair");
            destroyEngineItem.Activated += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    API.SetVehicleEngineHealth(veh.Handle, -4000f);
                    Main.ShowNotification("~r~Engine destroyed!");
                }
            };
            Menu.Add(destroyEngineItem);

            Menu.Add(new NativeSeparatorItem());

            // === LICENSE PLATE SECTION ===
            var plateHeader = new NativeItem("~b~=== License Plate ===", "Customize license plate")
            {
                Enabled = false
            };
            Menu.Add(plateHeader);

            // Set License Plate Text
            var plateTextItem = new NativeItem("Set License Plate Text", "Enter custom plate text");
            plateTextItem.Activated += async (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    var input = await Main.GetUserInput("Enter license plate text", "", 8);
                    if (!string.IsNullOrEmpty(input))
                    {
                        veh.Mods.LicensePlate = input.ToUpper();
                        Main.ShowNotification($"~g~License plate set to: {input.ToUpper()}");
                    }
                }
            };
            Menu.Add(plateTextItem);

            // License Plate Style
            var plateStyles = new List<string> { "Blue on White 1", "Blue on White 2", "Blue on White 3", "Yellow on Blue", "Yellow on Black", "North Yankton" };
            var plateStyleItem = new NativeListItem<string>("License Plate Style", "Change the plate style", plateStyles.ToArray());
            plateStyleItem.Activated += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    veh.Mods.LicensePlateStyle = (LicensePlateStyle)plateStyleItem.SelectedIndex;
                    Main.ShowNotification($"~b~Plate style: {plateStyles[plateStyleItem.SelectedIndex]}");
                }
            };
            Menu.Add(plateStyleItem);

            Menu.Add(new NativeSeparatorItem());

            // === SUBMENUS ===

            // Create Doors submenu
            CreateDoorsSubmenu();
            var doorsMenuButton = Menu.AddSubMenu(_doorsMenu);
            doorsMenuButton.Title = "Vehicle Doors";
            doorsMenuButton.Description = "Open, close, remove doors";

            // Create Colors submenu
            CreateColorsSubmenu();
            var colorsMenuButton = Menu.AddSubMenu(_colorsMenu);
            colorsMenuButton.Title = "Vehicle Colors";
            colorsMenuButton.Description = "Change vehicle color";

            // Add more upgrade options
            AddUpgradeOptions();

            Menu.Add(new NativeSeparatorItem());

            // === DELETE VEHICLE ===
            var deleteItem = new NativeItem("~r~Delete Vehicle", "~r~Delete your current vehicle (cannot be undone!)");
            deleteItem.Activated += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    if (veh.Driver == Game.PlayerPed)
                    {
                        Game.PlayerPed.Task.LeaveVehicle();
                        API.SetVehicleHasBeenOwnedByPlayer(veh.Handle, false);
                        API.SetEntityAsMissionEntity(veh.Handle, false, false);
                        veh.Delete();
                        Main.ShowNotification("~r~Vehicle deleted!");
                    }
                    else
                    {
                        Main.ShowNotification("~r~You must be the driver to delete this vehicle!");
                    }
                }
            };
            Menu.Add(deleteItem);
        }

        private void CreateDoorsSubmenu()
        {
            _doorsMenu = ThemeManager.CreateThemedMenu("comboom.sucht", "Vehicle Doors");
            Main.Pool.Add(_doorsMenu);

            // Open/Close each door
            for (int i = 0; i < DoorNames.Length; i++)
            {
                int doorIndex = i;
                string doorName = DoorNames[i];

                if (doorIndex == 6) // All Doors
                {
                    var openAllItem = new NativeItem("Open All Doors", "Open all vehicle doors");
                    openAllItem.Activated += (sender, args) =>
                    {
                        var veh = Game.PlayerPed.CurrentVehicle;
                        if (veh != null)
                        {
                            for (int d = 0; d < 6; d++)
                            {
                                API.SetVehicleDoorOpen(veh.Handle, d, false, false);
                            }
                            Main.ShowNotification("~g~All doors opened!");
                        }
                    };
                    _doorsMenu.Add(openAllItem);

                    var closeAllItem = new NativeItem("Close All Doors", "Close all vehicle doors");
                    closeAllItem.Activated += (sender, args) =>
                    {
                        var veh = Game.PlayerPed.CurrentVehicle;
                        if (veh != null)
                        {
                            API.SetVehicleDoorsShut(veh.Handle, false);
                            Main.ShowNotification("~g~All doors closed!");
                        }
                    };
                    _doorsMenu.Add(closeAllItem);
                }
                else
                {
                    var toggleDoorItem = new NativeItem($"Toggle {doorName}", $"Open/Close the {doorName.ToLower()} door");
                    toggleDoorItem.Activated += (sender, args) =>
                    {
                        var veh = Game.PlayerPed.CurrentVehicle;
                        if (veh != null)
                        {
                            float angle = API.GetVehicleDoorAngleRatio(veh.Handle, doorIndex);
                            if (angle < 0.1f)
                            {
                                API.SetVehicleDoorOpen(veh.Handle, doorIndex, false, false);
                            }
                            else
                            {
                                API.SetVehicleDoorShut(veh.Handle, doorIndex, false);
                            }
                        }
                    };
                    _doorsMenu.Add(toggleDoorItem);
                }
            }
        }

        private void CreateColorsSubmenu()
        {
            _colorsMenu = ThemeManager.CreateThemedMenu("comboom.sucht", "Vehicle Colors");
            Main.Pool.Add(_colorsMenu);

            // Primary Color
            var primaryColorItem = new NativeListItem<string>("Primary Color", "Change primary color", VehicleColors);
            primaryColorItem.ItemChanged += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    int colorIndex = ColorIndices[primaryColorItem.SelectedIndex];
                    int secondary = 0;
                    API.GetVehicleColours(veh.Handle, ref colorIndex, ref secondary);
                    API.SetVehicleColours(veh.Handle, ColorIndices[primaryColorItem.SelectedIndex], secondary);
                }
            };
            _colorsMenu.Add(primaryColorItem);

            // Secondary Color
            var secondaryColorItem = new NativeListItem<string>("Secondary Color", "Change secondary color", VehicleColors);
            secondaryColorItem.ItemChanged += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    int primary = 0, secondary = 0;
                    API.GetVehicleColours(veh.Handle, ref primary, ref secondary);
                    API.SetVehicleColours(veh.Handle, primary, ColorIndices[secondaryColorItem.SelectedIndex]);
                }
            };
            _colorsMenu.Add(secondaryColorItem);

            _colorsMenu.Add(new NativeSeparatorItem());

            // Custom RGB Color
            var customRgbItem = new NativeItem("Set Custom RGB Color", "Enter custom RGB values for primary color");
            customRgbItem.Activated += async (sender, args) =>
            {
                var input = await Main.GetUserInput("Enter RGB (R, G, B)", "255, 0, 0", 20);
                if (!string.IsNullOrEmpty(input))
                {
                    try
                    {
                        var parts = input.Split(',');
                        if (parts.Length >= 3)
                        {
                            int r = int.Parse(parts[0].Trim());
                            int g = int.Parse(parts[1].Trim());
                            int b = int.Parse(parts[2].Trim());
                            var veh = Game.PlayerPed.CurrentVehicle;
                            if (veh != null)
                            {
                                API.SetVehicleCustomPrimaryColour(veh.Handle, r, g, b);
                                Main.ShowNotification($"~g~Custom color set: RGB({r}, {g}, {b})");
                            }
                        }
                    }
                    catch
                    {
                        Main.ShowNotification("~r~Invalid RGB format!");
                    }
                }
            };
            _colorsMenu.Add(customRgbItem);
        }

        private void AddUpgradeOptions()
        {
            Menu.Add(new NativeSeparatorItem());

            var upgradeHeader = new NativeItem("~b~=== Quick Upgrades ===", "Quick vehicle upgrades")
            {
                Enabled = false
            };
            Menu.Add(upgradeHeader);

            // Max Performance
            var maxPerformanceItem = new NativeItem("Max Performance", "Apply all performance upgrades");
            maxPerformanceItem.Activated += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    API.SetVehicleModKit(veh.Handle, 0);

                    // Engine
                    API.SetVehicleMod(veh.Handle, 11, API.GetNumVehicleMods(veh.Handle, 11) - 1, false);
                    // Brakes
                    API.SetVehicleMod(veh.Handle, 12, API.GetNumVehicleMods(veh.Handle, 12) - 1, false);
                    // Transmission
                    API.SetVehicleMod(veh.Handle, 13, API.GetNumVehicleMods(veh.Handle, 13) - 1, false);
                    // Suspension
                    API.SetVehicleMod(veh.Handle, 15, API.GetNumVehicleMods(veh.Handle, 15) - 1, false);
                    // Armor
                    API.SetVehicleMod(veh.Handle, 16, API.GetNumVehicleMods(veh.Handle, 16) - 1, false);
                    // Turbo
                    API.ToggleVehicleMod(veh.Handle, 18, true);

                    Main.ShowNotification("~g~Max performance upgrades applied!");
                }
            };
            Menu.Add(maxPerformanceItem);

            // Max Armor
            var maxArmorItem = new NativeItem("Max Armor", "Apply maximum vehicle armor");
            maxArmorItem.Activated += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    API.SetVehicleModKit(veh.Handle, 0);
                    API.SetVehicleMod(veh.Handle, 16, API.GetNumVehicleMods(veh.Handle, 16) - 1, false);
                    Main.ShowNotification("~g~Max armor applied!");
                }
            };
            Menu.Add(maxArmorItem);

            // Bulletproof Tires
            var bulletproofTiresItem = new NativeCheckboxItem("Bulletproof Tires", "Make tires bulletproof", false);
            bulletproofTiresItem.CheckboxChanged += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    veh.CanTiresBurst = !bulletproofTiresItem.Checked;
                    Main.ShowNotification(bulletproofTiresItem.Checked ? "~g~Bulletproof Tires: ON" : "~r~Bulletproof Tires: OFF");
                }
            };
            Menu.Add(bulletproofTiresItem);

            // Fix/Burst Tires
            var tireOptions = new List<string> { "All Tires", "Front Left", "Front Right", "Rear Left", "Rear Right" };
            var tireItem = new NativeListItem<string>("Fix / Burst Tires", "Fix or burst specific tires", tireOptions.ToArray());
            tireItem.Activated += (sender, args) =>
            {
                var veh = Game.PlayerPed.CurrentVehicle;
                if (veh != null)
                {
                    if (tireItem.SelectedIndex == 0)
                    {
                        // All tires
                        for (int i = 0; i < 8; i++)
                        {
                            if (API.IsVehicleTyreBurst(veh.Handle, i, false))
                            {
                                API.SetVehicleTyreFixed(veh.Handle, i);
                            }
                            else
                            {
                                API.SetVehicleTyreBurst(veh.Handle, i, false, 1000f);
                            }
                        }
                    }
                    else
                    {
                        int tireIndex = tireItem.SelectedIndex - 1;
                        if (API.IsVehicleTyreBurst(veh.Handle, tireIndex, false))
                        {
                            API.SetVehicleTyreFixed(veh.Handle, tireIndex);
                        }
                        else
                        {
                            API.SetVehicleTyreBurst(veh.Handle, tireIndex, false, 1000f);
                        }
                    }
                    Main.ShowNotification("~b~Tire state toggled!");
                }
            };
            Menu.Add(tireItem);
        }

        #endregion

        #region Helper Methods

        private void CycleThroughSeats(Vehicle vehicle)
        {
            int maxSeats = API.GetVehicleMaxNumberOfPassengers(vehicle.Handle);
            int currentSeat = -2;

            // Find current seat
            for (int i = -1; i < maxSeats; i++)
            {
                if (API.GetPedInVehicleSeat(vehicle.Handle, i) == Game.PlayerPed.Handle)
                {
                    currentSeat = i;
                    break;
                }
            }

            // Find next available seat
            for (int i = 1; i <= maxSeats + 1; i++)
            {
                int nextSeat = (currentSeat + i) % (maxSeats + 1);
                if (nextSeat == -1) nextSeat = 0;

                if (API.IsVehicleSeatFree(vehicle.Handle, nextSeat - 1))
                {
                    Game.PlayerPed.Task.WarpIntoVehicle(vehicle, (VehicleSeat)(nextSeat - 1));
                    Main.ShowNotification($"~b~Moved to seat {nextSeat}");
                    return;
                }
            }

            Main.ShowNotification("~r~No available seats!");
        }

        private int GetColorIndex(int row, int column)
        {
            return row * 12 + column;
        }

        private int GetWindowTintIndex(float alpha)
        {
            if (alpha <= 0.0f) return 0;  // None
            if (alpha <= 0.2f) return 1;  // Pure Black
            if (alpha <= 0.4f) return 2;  // Dark Smoke
            if (alpha <= 0.6f) return 3;  // Light Smoke
            if (alpha <= 0.8f) return 4;  // Stock
            return 5;  // Limo
        }

        #endregion
    }
}
