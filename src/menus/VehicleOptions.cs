using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CitizenFX.Core;

using LemonUI;
using LemonUI.Menus;

using CBPSMenu.Shared;

using static CitizenFX.Core.Native.API;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Vehicle Options submenu - Full vMenu clone.
    /// </summary>
    public class VehicleOptions
    {
        private NativeMenu menu;

        // Submenus
        private NativeMenu vehicleGodMenu;
        private NativeMenu vehicleDoorsMenu;
        private NativeMenu vehicleWindowsMenu;
        private NativeMenu vehicleColorsMenu;
        private NativeMenu vehicleExtrasMenu;
        private NativeMenu vehicleLiveriesMenu;
        private NativeMenu deleteConfirmMenu;

        // Vehicle state
        public bool VehicleGodMode { get; private set; } = false;
        public bool VehicleGodInvincible { get; private set; } = false;
        public bool VehicleGodEngine { get; private set; } = false;
        public bool VehicleGodVisual { get; private set; } = false;
        public bool VehicleGodStrongWheels { get; private set; } = false;
        public bool VehicleGodAutoRepair { get; private set; } = false;
        public bool VehicleNeverDirty { get; private set; } = false;
        public bool VehicleEngineAlwaysOn { get; private set; } = false;
        public bool VehicleNoSiren { get; private set; } = false;
        public bool VehicleNoBikeHelmet { get; private set; } = false;
        public bool VehicleFrozen { get; private set; } = false;
        public bool VehicleInvisible { get; private set; } = false;
        public bool VehicleBikeSeatbelt { get; private set; } = false;
        public bool TorqueEnabled { get; private set; } = false;
        public bool PowerEnabled { get; private set; } = false;
        public float TorqueMultiplier { get; private set; } = 2f;
        public float PowerMultiplier { get; private set; } = 2f;

        private readonly List<string> torqueMultipliers = new List<string> { "x2", "x4", "x8", "x16", "x32", "x64", "x128", "x256", "x512", "x1024" };
        private readonly List<string> powerMultipliers = new List<string> { "x2", "x4", "x8", "x16", "x32", "x64", "x128", "x256", "x512", "x1024" };
        private readonly List<string> dirtLevels = new List<string> { "No Dirt", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15" };
        private readonly List<string> licensePlateTypes = new List<string> { "Blue/White 1", "Blue/White 2", "Blue/White 3", "Yellow/Blue", "Yellow/Black", "North Yankton" };

        private void CreateMenu()
        {
            menu = new NativeMenu("Vehicle Options", "Vehicle Options Menu");

            // Create submenus
            vehicleGodMenu = new NativeMenu("God Mode Options", "Vehicle God Mode Options");
            vehicleDoorsMenu = new NativeMenu("Vehicle Doors", "Vehicle Doors Management");
            vehicleWindowsMenu = new NativeMenu("Vehicle Windows", "Vehicle Windows Management");
            vehicleColorsMenu = new NativeMenu("Vehicle Colors", "Vehicle Colors");
            vehicleExtrasMenu = new NativeMenu("Vehicle Extras", "Vehicle Extras/Components");
            vehicleLiveriesMenu = new NativeMenu("Vehicle Liveries", "Vehicle Liveries");
            deleteConfirmMenu = new NativeMenu("Confirm Delete", "Are you sure?");

            #region God Mode Section

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VOGod))
            {
                var vehicleGod = new NativeCheckboxItem("Vehicle God Mode", "Makes your vehicle not take any damage.", VehicleGodMode);
                vehicleGod.CheckboxChanged += (s, e) =>
                {
                    VehicleGodMode = vehicleGod.Checked;
                    if (Game.PlayerPed.IsInVehicle())
                    {
                        var veh = Game.PlayerPed.CurrentVehicle;
                        veh.IsInvincible = VehicleGodMode;
                        veh.CanBeVisiblyDamaged = !VehicleGodMode;
                    }
                };
                menu.Add(vehicleGod);

                var godMenuBtn = new NativeItem("God Mode Options", "Enable/disable specific damage types.") { AltTitle = "→→→" };
                menu.Add(godMenuBtn);

                // God mode submenu items
                var godInvincible = new NativeCheckboxItem("Invincible", "Makes the car invincible to all damage.", VehicleGodInvincible);
                godInvincible.CheckboxChanged += (s, e) => VehicleGodInvincible = godInvincible.Checked;
                vehicleGodMenu.Add(godInvincible);

                var godEngine = new NativeCheckboxItem("Engine Damage", "Disables engine damage.", VehicleGodEngine);
                godEngine.CheckboxChanged += (s, e) => VehicleGodEngine = godEngine.Checked;
                vehicleGodMenu.Add(godEngine);

                var godVisual = new NativeCheckboxItem("Visual Damage", "Prevents scratches and decals.", VehicleGodVisual);
                godVisual.CheckboxChanged += (s, e) => VehicleGodVisual = godVisual.Checked;
                vehicleGodMenu.Add(godVisual);

                var godStrongWheels = new NativeCheckboxItem("Strong Wheels", "Prevents wheel deformation.", VehicleGodStrongWheels);
                godStrongWheels.CheckboxChanged += (s, e) => VehicleGodStrongWheels = godStrongWheels.Checked;
                vehicleGodMenu.Add(godStrongWheels);

                var godAutoRepair = new NativeCheckboxItem("~r~Auto Repair", "Automatically repairs vehicle when damaged.", VehicleGodAutoRepair);
                godAutoRepair.CheckboxChanged += (s, e) => VehicleGodAutoRepair = godAutoRepair.Checked;
                vehicleGodMenu.Add(godAutoRepair);
            }

            #endregion

            #region Repair/Clean Section

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VORepair))
            {
                var fixVehicle = new NativeItem("Repair Vehicle", "Repair all vehicle damage.");
                fixVehicle.Activated += (s, e) =>
                {
                    if (!CheckVehicle()) return;
                    Game.PlayerPed.CurrentVehicle.Repair();
                    Notify.Success("Vehicle repaired.");
                };
                menu.Add(fixVehicle);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VOKeepClean))
            {
                var neverDirty = new NativeCheckboxItem("Keep Vehicle Clean", "Constantly cleans your vehicle.", VehicleNeverDirty);
                neverDirty.CheckboxChanged += (s, e) => VehicleNeverDirty = neverDirty.Checked;
                menu.Add(neverDirty);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VOWash))
            {
                var cleanVehicle = new NativeItem("Wash Vehicle", "Clean your vehicle.");
                cleanVehicle.Activated += (s, e) =>
                {
                    if (!CheckVehicle()) return;
                    Game.PlayerPed.CurrentVehicle.DirtLevel = 0f;
                    Game.PlayerPed.CurrentVehicle.Wash();
                    Notify.Success("Vehicle washed.");
                };
                menu.Add(cleanVehicle);

                var setDirtLevel = new NativeListItem<string>("Set Dirt Level", "Set dirt visibility.", dirtLevels.ToArray());
                setDirtLevel.ItemChanged += (s, e) =>
                {
                    if (!CheckVehicle()) return;
                    Game.PlayerPed.CurrentVehicle.DirtLevel = (float)dirtLevels.IndexOf(e.Object);
                };
                menu.Add(setDirtLevel);
            }

            #endregion

            #region Doors Menu

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VODoors))
            {
                var doorsMenuBtn = new NativeItem("Vehicle Doors", "Open, close, remove and restore doors.") { AltTitle = "→→→" };
                menu.Add(doorsMenuBtn);

                CreateDoorsMenu();
            }

            #endregion

            #region Windows Menu

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VOWindows))
            {
                var windowsMenuBtn = new NativeItem("Vehicle Windows", "Roll windows up/down or remove them.") { AltTitle = "→→→" };
                menu.Add(windowsMenuBtn);

                CreateWindowsMenu();
            }

            #endregion

            #region Engine/Performance

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VOEngine))
            {
                var toggleEngine = new NativeItem("Toggle Engine On/Off", "Turn your engine on or off.");
                toggleEngine.Activated += (s, e) =>
                {
                    if (!CheckVehicle()) return;
                    var veh = Game.PlayerPed.CurrentVehicle;
                    SetVehicleEngineOn(veh.Handle, !veh.IsEngineRunning, false, true);
                    Notify.Info($"Engine {(veh.IsEngineRunning ? "started" : "stopped")}.");
                };
                menu.Add(toggleEngine);

                var engineAlwaysOn = new NativeCheckboxItem("Engine Always On", "Keeps engine on when you exit.", VehicleEngineAlwaysOn);
                engineAlwaysOn.CheckboxChanged += (s, e) => VehicleEngineAlwaysOn = engineAlwaysOn.Checked;
                menu.Add(engineAlwaysOn);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VODestroyEngine))
            {
                var destroyEngine = new NativeItem("~r~Destroy Engine", "Destroys your vehicle's engine.");
                destroyEngine.Activated += (s, e) =>
                {
                    if (!CheckVehicle()) return;
                    SetVehicleEngineHealth(Game.PlayerPed.CurrentVehicle.Handle, -4000f);
                    Notify.Success("Engine destroyed.");
                };
                menu.Add(destroyEngine);
            }

            #endregion

            #region Speed Limiter

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VOSpeedLimiter))
            {
                var speedLimiterOptions = new List<string> { "Set Current Speed", "Reset Limit", "Custom Speed" };
                var speedLimiter = new NativeListItem<string>("Speed Limiter", "Limit your vehicle's max speed.", speedLimiterOptions.ToArray());
                speedLimiter.ItemChanged += (s, e) =>
                {
                    if (!CheckVehicle()) return;
                    var veh = Game.PlayerPed.CurrentVehicle;
                    var index = speedLimiterOptions.IndexOf(e.Object);
                    if (index == 0)
                    {
                        SetVehicleMaxSpeed(veh.Handle, GetEntitySpeed(veh.Handle));
                        Notify.Success("Speed limit set to current speed.");
                    }
                    else if (index == 1)
                    {
                        SetVehicleMaxSpeed(veh.Handle, 0f);
                        Notify.Success("Speed limit reset.");
                    }
                };
                menu.Add(speedLimiter);
            }

            #endregion

            #region License Plate

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VOChangePlate))
            {
                var plateText = new NativeItem("Set License Plate Text", "Enter custom plate text.");
                plateText.Activated += async (s, e) =>
                {
                    if (!CheckVehicle()) return;
                    var text = await GetUserInput("Enter plate text (max 8 chars)", "", 8);
                    if (!string.IsNullOrEmpty(text))
                    {
                        SetVehicleNumberPlateText(Game.PlayerPed.CurrentVehicle.Handle, text);
                        Notify.Success($"Plate set to: {text}");
                    }
                };
                menu.Add(plateText);

                var plateType = new NativeListItem<string>("License Plate Type", "Choose plate style.", licensePlateTypes.ToArray());
                plateType.ItemChanged += (s, e) =>
                {
                    if (!CheckVehicle()) return;
                    var index = licensePlateTypes.IndexOf(e.Object);
                    SetVehicleNumberPlateTextIndex(Game.PlayerPed.CurrentVehicle.Handle, index);
                };
                menu.Add(plateType);
            }

            #endregion

            #region Torque/Power Multipliers

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VOTorqueMultiplier))
            {
                var torqueEnabled = new NativeCheckboxItem("Enable Torque Multiplier", "Enable engine torque boost.", TorqueEnabled);
                torqueEnabled.CheckboxChanged += (s, e) => TorqueEnabled = torqueEnabled.Checked;
                menu.Add(torqueEnabled);

                var torqueList = new NativeListItem<string>("Torque Multiplier", "Set torque multiplier.", torqueMultipliers.ToArray());
                torqueList.ItemChanged += (s, e) =>
                {
                    var value = torqueMultipliers.IndexOf(e.Object);
                    TorqueMultiplier = (float)Math.Pow(2, value + 1);
                };
                menu.Add(torqueList);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VOPowerMultiplier))
            {
                var powerEnabled = new NativeCheckboxItem("Enable Power Multiplier", "Enable engine power boost.", PowerEnabled);
                powerEnabled.CheckboxChanged += (s, e) =>
                {
                    PowerEnabled = powerEnabled.Checked;
                    if (CheckVehicle())
                    {
                        SetVehicleEnginePowerMultiplier(Game.PlayerPed.CurrentVehicle.Handle, PowerEnabled ? PowerMultiplier : 1f);
                    }
                };
                menu.Add(powerEnabled);

                var powerList = new NativeListItem<string>("Power Multiplier", "Set power multiplier.", powerMultipliers.ToArray());
                powerList.ItemChanged += (s, e) =>
                {
                    var value = powerMultipliers.IndexOf(e.Object);
                    PowerMultiplier = (float)Math.Pow(2, value + 1);
                    if (PowerEnabled && CheckVehicle())
                    {
                        SetVehicleEnginePowerMultiplier(Game.PlayerPed.CurrentVehicle.Handle, PowerMultiplier);
                    }
                };
                menu.Add(powerList);
            }

            #endregion

            #region Tires

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VOFixOrDestroyTires))
            {
                var tiresList = new List<string> { "All Tires", "Tire #1", "Tire #2", "Tire #3", "Tire #4", "Tire #5", "Tire #6", "Tire #7", "Tire #8" };
                var tireOptions = new List<string> { "Fix", "Destroy" };

                var selectTire = new NativeListItem<string>("Select Tire", "Select which tire to fix/destroy.", tiresList.ToArray());
                menu.Add(selectTire);

                var fixTire = new NativeItem("Fix Selected Tire", "Fix the selected tire.");
                fixTire.Activated += (s, e) =>
                {
                    if (!CheckVehicle()) return;
                    var veh = Game.PlayerPed.CurrentVehicle;
                    var tireIndex = selectTire.SelectedIndex;
                    if (tireIndex == 0)
                    {
                        for (var i = 0; i < 8; i++) SetVehicleTyreFixed(veh.Handle, i);
                        Notify.Success("All tires fixed.");
                    }
                    else
                    {
                        SetVehicleTyreFixed(veh.Handle, tireIndex - 1);
                        Notify.Success($"Tire #{tireIndex} fixed.");
                    }
                };
                menu.Add(fixTire);

                var burstTire = new NativeItem("~r~Burst Selected Tire", "Burst the selected tire.");
                burstTire.Activated += (s, e) =>
                {
                    if (!CheckVehicle()) return;
                    var veh = Game.PlayerPed.CurrentVehicle;
                    var tireIndex = selectTire.SelectedIndex;
                    if (tireIndex == 0)
                    {
                        for (var i = 0; i < 8; i++) SetVehicleTyreBurst(veh.Handle, i, true, 1000f);
                        Notify.Success("All tires burst.");
                    }
                    else
                    {
                        SetVehicleTyreBurst(veh.Handle, tireIndex - 1, true, 1000f);
                        Notify.Success($"Tire #{tireIndex} burst.");
                    }
                };
                menu.Add(burstTire);
            }

            #endregion

            #region Misc Vehicle Options

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VOFlip))
            {
                var flipVehicle = new NativeItem("Flip Vehicle", "Set your vehicle on all 4 wheels.");
                flipVehicle.Activated += (s, e) =>
                {
                    if (!CheckVehicle()) return;
                    SetVehicleOnGroundProperly(Game.PlayerPed.CurrentVehicle.Handle);
                    Notify.Success("Vehicle flipped.");
                };
                menu.Add(flipVehicle);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VOAlarm))
            {
                var vehicleAlarm = new NativeItem("Toggle Vehicle Alarm", "Start/stop vehicle alarm.");
                vehicleAlarm.Activated += (s, e) =>
                {
                    if (!CheckVehicle()) return;
                    var veh = Game.PlayerPed.CurrentVehicle;
                    if (IsVehicleAlarmActivated(veh.Handle))
                    {
                        SetVehicleAlarm(veh.Handle, false);
                        SetVehicleAlarmTimeLeft(veh.Handle, 0);
                        Notify.Info("Alarm stopped.");
                    }
                    else
                    {
                        SetVehicleAlarm(veh.Handle, true);
                        SetVehicleAlarmTimeLeft(veh.Handle, int.MaxValue);
                        Notify.Info("Alarm started.");
                    }
                };
                menu.Add(vehicleAlarm);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VOCycleSeats))
            {
                var cycleSeats = new NativeItem("Cycle Through Seats", "Move to the next available seat.");
                cycleSeats.Activated += async (s, e) =>
                {
                    if (!CheckVehicle()) return;
                    await CycleThroughSeats();
                };
                menu.Add(cycleSeats);
            }

            var vehicleLights = new List<string> { "Hazard Lights", "Left Indicator", "Right Indicator", "Interior Lights", "Helicopter Spotlight" };
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VOLights))
            {
                var lightsControl = new NativeListItem<string>("Vehicle Lights", "Control vehicle lights.", vehicleLights.ToArray());
                lightsControl.ItemChanged += (s, e) =>
                {
                    if (!CheckVehicle()) return;
                    var veh = Game.PlayerPed.CurrentVehicle;
                    var index = vehicleLights.IndexOf(e.Object);
                    switch (index)
                    {
                        case 0: // Hazard
                            var hazardState = GetVehicleIndicatorLights(veh.Handle);
                            SetVehicleIndicatorLights(veh.Handle, 0, hazardState != 1);
                            SetVehicleIndicatorLights(veh.Handle, 1, hazardState != 1);
                            break;
                        case 1: // Left
                            SetVehicleIndicatorLights(veh.Handle, 1, true);
                            SetVehicleIndicatorLights(veh.Handle, 0, false);
                            break;
                        case 2: // Right
                            SetVehicleIndicatorLights(veh.Handle, 0, true);
                            SetVehicleIndicatorLights(veh.Handle, 1, false);
                            break;
                        case 3: // Interior
                            SetVehicleInteriorlight(veh.Handle, !IsVehicleInteriorLightOn(veh.Handle));
                            break;
                        case 4: // Heli spotlight
                            if (veh.Model.IsHelicopter)
                            {
                                SetVehicleSearchlight(veh.Handle, !IsVehicleSearchlightOn(veh.Handle), true);
                            }
                            break;
                    }
                };
                menu.Add(lightsControl);
            }

            // Bike seatbelt
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VOBikeSeatbelt))
            {
                var bikeSeatbelt = new NativeCheckboxItem("Bike Seatbelt", "Prevents falling off bikes.", VehicleBikeSeatbelt);
                bikeSeatbelt.CheckboxChanged += (s, e) => VehicleBikeSeatbelt = bikeSeatbelt.Checked;
                menu.Add(bikeSeatbelt);
            }

            // No siren
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VONoSiren))
            {
                var noSiren = new NativeCheckboxItem("Disable Siren", "Disables vehicle siren.", VehicleNoSiren);
                noSiren.CheckboxChanged += (s, e) =>
                {
                    VehicleNoSiren = noSiren.Checked;
                    if (CheckVehicle())
                    {
                        Game.PlayerPed.CurrentVehicle.IsSirenSilent = VehicleNoSiren;
                    }
                };
                menu.Add(noSiren);
            }

            // No bike helmet
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VONoHelmet))
            {
                var noBikeHelmet = new NativeCheckboxItem("No Bike Helmet", "Don't auto-equip helmet on bikes.", VehicleNoBikeHelmet);
                noBikeHelmet.CheckboxChanged += (s, e) => VehicleNoBikeHelmet = noBikeHelmet.Checked;
                menu.Add(noBikeHelmet);
            }

            #endregion

            #region Freeze/Invisible

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VOFreeze))
            {
                var freezeVehicle = new NativeCheckboxItem("Freeze Vehicle", "Freeze vehicle position.", VehicleFrozen);
                freezeVehicle.CheckboxChanged += (s, e) =>
                {
                    VehicleFrozen = freezeVehicle.Checked;
                    if (CheckVehicle())
                    {
                        FreezeEntityPosition(Game.PlayerPed.CurrentVehicle.Handle, VehicleFrozen);
                    }
                };
                menu.Add(freezeVehicle);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VOInvisible))
            {
                var invisibleVehicle = new NativeItem("Toggle Vehicle Visibility", "Make vehicle invisible.");
                invisibleVehicle.Activated += (s, e) =>
                {
                    if (!CheckVehicle()) return;
                    var veh = Game.PlayerPed.CurrentVehicle;
                    veh.IsVisible = !veh.IsVisible;
                    VehicleInvisible = !veh.IsVisible;
                    Notify.Info($"Vehicle is now {(veh.IsVisible ? "visible" : "invisible")}.");
                };
                menu.Add(invisibleVehicle);
            }

            #endregion

            #region Delete Vehicle

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VODelete))
            {
                var deleteBtn = new NativeItem("~r~Delete Vehicle", "Delete your current vehicle.") { AltTitle = "→→→" };
                menu.Add(deleteBtn);

                var deleteNo = new NativeItem("NO, CANCEL", "Do NOT delete my vehicle.");
                deleteNo.Activated += (s, e) => deleteConfirmMenu.Visible = false;
                deleteConfirmMenu.Add(deleteNo);

                var deleteYes = new NativeItem("~r~YES, DELETE", "Delete my vehicle.");
                deleteYes.Activated += (s, e) =>
                {
                    if (CheckVehicle() && Game.PlayerPed.CurrentVehicle.Driver == Game.PlayerPed)
                    {
                        var veh = Game.PlayerPed.CurrentVehicle;
                        SetVehicleHasBeenOwnedByPlayer(veh.Handle, false);
                        SetEntityAsMissionEntity(veh.Handle, false, false);
                        veh.Delete();
                        Notify.Success("Vehicle deleted.");
                    }
                    else
                    {
                        Notify.Error("You must be the driver to delete the vehicle.");
                    }
                    deleteConfirmMenu.Visible = false;
                    menu.Visible = false;
                };
                deleteConfirmMenu.Add(deleteYes);
            }

            #endregion
        }

        private void CreateDoorsMenu()
        {
            var doorNames = new string[] { "Front Left", "Front Right", "Rear Left", "Rear Right", "Hood", "Trunk", "Extra 1", "Extra 2" };

            for (var i = 0; i < doorNames.Length; i++)
            {
                var doorIndex = i;
                var openDoor = new NativeItem($"Open {doorNames[i]}", $"Open the {doorNames[i].ToLower()} door.");
                openDoor.Activated += (s, e) =>
                {
                    if (!CheckVehicle()) return;
                    SetVehicleDoorOpen(Game.PlayerPed.CurrentVehicle.Handle, doorIndex, false, false);
                };
                vehicleDoorsMenu.Add(openDoor);
            }

            var closeAllDoors = new NativeItem("Close All Doors", "Close all vehicle doors.");
            closeAllDoors.Activated += (s, e) =>
            {
                if (!CheckVehicle()) return;
                SetVehicleDoorsShut(Game.PlayerPed.CurrentVehicle.Handle, false);
                Notify.Success("All doors closed.");
            };
            vehicleDoorsMenu.Add(closeAllDoors);

            var openAllDoors = new NativeItem("Open All Doors", "Open all vehicle doors.");
            openAllDoors.Activated += (s, e) =>
            {
                if (!CheckVehicle()) return;
                for (var i = 0; i < 8; i++)
                {
                    SetVehicleDoorOpen(Game.PlayerPed.CurrentVehicle.Handle, i, false, false);
                }
                Notify.Success("All doors opened.");
            };
            vehicleDoorsMenu.Add(openAllDoors);
        }

        private void CreateWindowsMenu()
        {
            var windowNames = new string[] { "Front Left", "Front Right", "Rear Left", "Rear Right" };

            for (var i = 0; i < windowNames.Length; i++)
            {
                var windowIndex = i;
                var rollDown = new NativeItem($"Roll Down {windowNames[i]}", $"Roll down the {windowNames[i].ToLower()} window.");
                rollDown.Activated += (s, e) =>
                {
                    if (!CheckVehicle()) return;
                    RollDownWindow(Game.PlayerPed.CurrentVehicle.Handle, windowIndex);
                };
                vehicleWindowsMenu.Add(rollDown);
            }

            var rollUpAll = new NativeItem("Roll Up All Windows", "Roll up all windows.");
            rollUpAll.Activated += (s, e) =>
            {
                if (!CheckVehicle()) return;
                RollUpWindow(Game.PlayerPed.CurrentVehicle.Handle, 0);
                RollUpWindow(Game.PlayerPed.CurrentVehicle.Handle, 1);
                RollUpWindow(Game.PlayerPed.CurrentVehicle.Handle, 2);
                RollUpWindow(Game.PlayerPed.CurrentVehicle.Handle, 3);
                Notify.Success("All windows rolled up.");
            };
            vehicleWindowsMenu.Add(rollUpAll);

            var rollDownAll = new NativeItem("Roll Down All Windows", "Roll down all windows.");
            rollDownAll.Activated += (s, e) =>
            {
                if (!CheckVehicle()) return;
                RollDownWindows(Game.PlayerPed.CurrentVehicle.Handle);
                Notify.Success("All windows rolled down.");
            };
            vehicleWindowsMenu.Add(rollDownAll);
        }

        private bool CheckVehicle()
        {
            if (!Game.PlayerPed.IsInVehicle())
            {
                Notify.Error("You need to be in a vehicle.");
                return false;
            }
            return true;
        }

        private async Task<string> GetUserInput(string windowTitle, string defaultText, int maxLength)
        {
            AddTextEntry("FMMC_KEY_TIP1", windowTitle);
            DisplayOnscreenKeyboard(1, "FMMC_KEY_TIP1", "", defaultText, "", "", "", maxLength);
            while (UpdateOnscreenKeyboard() == 0)
            {
                await BaseScript.Delay(0);
            }
            if (UpdateOnscreenKeyboard() == 1)
            {
                return GetOnscreenKeyboardResult();
            }
            return null;
        }

        private async Task CycleThroughSeats()
        {
            var veh = Game.PlayerPed.CurrentVehicle;
            var maxSeats = GetVehicleMaxNumberOfPassengers(veh.Handle) + 1;
            var currentSeat = -2;

            // Find current seat
            for (var i = -1; i < maxSeats; i++)
            {
                if (GetPedInVehicleSeat(veh.Handle, i) == Game.PlayerPed.Handle)
                {
                    currentSeat = i;
                    break;
                }
            }

            // Find next empty seat
            for (var i = 1; i <= maxSeats; i++)
            {
                var nextSeat = (currentSeat + i) % maxSeats;
                if (nextSeat == -1) nextSeat = -1;

                if (IsVehicleSeatFree(veh.Handle, nextSeat))
                {
                    Game.PlayerPed.Task.WarpIntoVehicle(veh, (VehicleSeat)nextSeat);
                    return;
                }
            }

            Notify.Error("No empty seats available.");
        }

        public NativeMenu GetMenu()
        {
            if (menu == null)
            {
                CreateMenu();
            }
            return menu;
        }

        public NativeMenu GetGodMenu() => vehicleGodMenu;
        public NativeMenu GetDoorsMenu() => vehicleDoorsMenu;
        public NativeMenu GetWindowsMenu() => vehicleWindowsMenu;
        public NativeMenu GetColorsMenu() => vehicleColorsMenu;
        public NativeMenu GetDeleteConfirmMenu() => deleteConfirmMenu;
    }
}
