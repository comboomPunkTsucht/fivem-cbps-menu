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
    /// Misc Settings submenu - vMenu clone.
    /// </summary>
    public class MiscSettings
    {
        private NativeMenu menu;

        // State
        public bool ShowCoordinates { get; private set; } = false;
        public bool ShowLocation { get; private set; } = false;
        public bool HideHud { get; private set; } = false;
        public bool HideRadar { get; private set; } = false;
        public bool LockCameraX { get; private set; } = false;
        public bool LockCameraY { get; private set; } = false;
        public bool NightVision { get; private set; } = false;
        public bool ThermalVision { get; private set; } = false;
        public bool RestoreAppearance { get; private set; } = false;
        public bool RestoreWeapons { get; private set; } = false;

        private void CreateMenu()
        {
            menu = new NativeMenu("Misc Settings", "Miscellaneous Settings Menu");

            #region Teleport Options

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.MSTeleportToWp))
            {
                var teleportToWaypoint = new NativeItem("Teleport To Waypoint", "Teleport to the waypoint on your map.");
                teleportToWaypoint.Activated += async (s, e) =>
                {
                    if (IsWaypointActive())
                    {
                        var waypointPos = GetBlipInfoIdCoord(GetFirstBlipInfoId(8));
                        var groundZ = 0f;

                        // Try to find ground Z
                        for (var ztest = 1000f; ztest >= 0f; ztest -= 25f)
                        {
                            if (GetGroundZFor_3dCoord(waypointPos.X, waypointPos.Y, ztest, ref groundZ, false))
                            {
                                break;
                            }
                            await BaseScript.Delay(5);
                        }

                        if (groundZ == 0f)
                        {
                            groundZ = 200f; // Default if ground not found
                        }

                        SetPedCoordsKeepVehicle(Game.PlayerPed.Handle, waypointPos.X, waypointPos.Y, groundZ + 1f);
                        Notify.Success("Teleported to waypoint.");
                    }
                    else
                    {
                        Notify.Error("No waypoint set.");
                    }
                };
                menu.Add(teleportToWaypoint);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.MSTeleportLocations))
            {
                // Teleport location data (name => x, y, z)
                var locationData = new Dictionary<string, Vector3>
                {
                    { "LSIA", new Vector3(-1034.6f, -2733.6f, 20.2f) },
                    { "Sandy Shores", new Vector3(1697.6f, 3245.7f, 41.5f) },
                    { "Paleto Bay", new Vector3(-319.8f, 6083.7f, 31.5f) },
                    { "Mount Chiliad", new Vector3(497.8f, 5591.2f, 795.0f) },
                    { "Fort Zancudo", new Vector3(-2358.8f, 3249.1f, 101.5f) },
                    { "Maze Bank Tower", new Vector3(-75.0f, -818.2f, 326.2f) },
                    { "Del Perro Pier", new Vector3(-1850.1f, -1231.8f, 13.0f) },
                    { "Vinewood Sign", new Vector3(711.4f, 1198.2f, 348.5f) },
                    { "Downtown Vinewood", new Vector3(287.5f, 180.5f, 104.6f) },
                    { "Eclipse Towers", new Vector3(-773.5f, 312.0f, 85.7f) },
                    { "Vespucci Beach", new Vector3(-1373.6f, -1398.8f, 6.1f) },
                    { "Diamond Casino", new Vector3(924.9f, 47.5f, 81.1f) },
                };

                var locationNames = new List<string>(locationData.Keys);

                var teleportList = new NativeListItem<string>("Teleport Locations", "Teleport to preset locations.", locationNames.ToArray());
                menu.Add(teleportList);

                var goToLocation = new NativeItem("Go To Selected Location", "Teleport to the selected location.");
                goToLocation.Activated += (s, e) =>
                {
                    var name = locationNames[teleportList.SelectedIndex];
                    var pos = locationData[name];
                    SetPedCoordsKeepVehicle(Game.PlayerPed.Handle, pos.X, pos.Y, pos.Z);
                    Notify.Success($"Teleported to {name}.");
                };
                menu.Add(goToLocation);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.MSTeleportToCoord))
            {
                var teleportToCoord = new NativeItem("Teleport To Coordinates", "Enter coordinates to teleport to.");
                teleportToCoord.Activated += async (s, e) =>
                {
                    var input = await GetUserInput("Enter X,Y,Z (e.g., 100.0,200.0,50.0)", "", 50);
                    if (!string.IsNullOrEmpty(input))
                    {
                        var parts = input.Split(',');
                        if (parts.Length == 3 &&
                            float.TryParse(parts[0].Trim(), out float x) &&
                            float.TryParse(parts[1].Trim(), out float y) &&
                            float.TryParse(parts[2].Trim(), out float z))
                        {
                            SetPedCoordsKeepVehicle(Game.PlayerPed.Handle, x, y, z);
                            Notify.Success($"Teleported to ({x}, {y}, {z}).");
                        }
                        else
                        {
                            Notify.Error("Invalid coordinates format.");
                        }
                    }
                };
                menu.Add(teleportToCoord);
            }

            #endregion

            #region Display Options

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.MSShowCoordinates))
            {
                var showCoords = new NativeCheckboxItem("Show Coordinates", "Display current coordinates on screen.", ShowCoordinates);
                showCoords.CheckboxChanged += (s, e) => ShowCoordinates = showCoords.Checked;
                menu.Add(showCoords);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.MSShowLocation))
            {
                var showLoc = new NativeCheckboxItem("Show Location", "Display current street name and area.", ShowLocation);
                showLoc.CheckboxChanged += (s, e) => ShowLocation = showLoc.Checked;
                menu.Add(showLoc);
            }

            var hideHud = new NativeCheckboxItem("Hide HUD", "Hide the game HUD.", HideHud);
            hideHud.CheckboxChanged += (s, e) =>
            {
                HideHud = hideHud.Checked;
                DisplayHud(!HideHud);
            };
            menu.Add(hideHud);

            var hideRadar = new NativeCheckboxItem("Hide Radar/Minimap", "Hide the minimap.", HideRadar);
            hideRadar.CheckboxChanged += (s, e) =>
            {
                HideRadar = hideRadar.Checked;
                DisplayRadar(!HideRadar);
            };
            menu.Add(hideRadar);

            #endregion

            #region Camera Options

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.MSLockCameraX))
            {
                var lockCamX = new NativeCheckboxItem("Lock Camera X", "Lock camera horizontal rotation.", LockCameraX);
                lockCamX.CheckboxChanged += (s, e) =>
                {
                    LockCameraX = lockCamX.Checked;
                    SetGameplayCamRelativeHeading(0f);
                };
                menu.Add(lockCamX);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.MSLockCameraY))
            {
                var lockCamY = new NativeCheckboxItem("Lock Camera Y", "Lock camera vertical rotation.", LockCameraY);
                lockCamY.CheckboxChanged += (s, e) =>
                {
                    LockCameraY = lockCamY.Checked;
                    SetGameplayCamRelativePitch(0f, 1f);
                };
                menu.Add(lockCamY);
            }

            #endregion

            #region Vision Options

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.MSNightVision))
            {
                var nightVis = new NativeCheckboxItem("Night Vision", "Toggle night vision.", NightVision);
                nightVis.CheckboxChanged += (s, e) =>
                {
                    NightVision = nightVis.Checked;
                    SetNightvision(NightVision);
                };
                menu.Add(nightVis);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.MSThermalVision))
            {
                var thermalVis = new NativeCheckboxItem("Thermal Vision", "Toggle thermal vision.", ThermalVision);
                thermalVis.CheckboxChanged += (s, e) =>
                {
                    ThermalVision = thermalVis.Checked;
                    SetSeethrough(ThermalVision);
                };
                menu.Add(thermalVis);
            }

            #endregion

            #region Restore Options

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.MSRestoreAppearance))
            {
                var restoreApp = new NativeCheckboxItem("Restore Appearance on Respawn", "Restore your ped appearance after respawn.", RestoreAppearance);
                restoreApp.CheckboxChanged += (s, e) => RestoreAppearance = restoreApp.Checked;
                menu.Add(restoreApp);
            }

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.MSRestoreWeapons))
            {
                var restoreWeps = new NativeCheckboxItem("Restore Weapons on Respawn", "Keep your weapons after respawn.", RestoreWeapons);
                restoreWeps.CheckboxChanged += (s, e) => RestoreWeapons = restoreWeps.Checked;
                menu.Add(restoreWeps);
            }

            #endregion

            #region Utility

            var connectionSubmenu = new NativeItem("Connection Options", "Disconnect or reconnect from the server.") { AltTitle = "→→→" };
            menu.Add(connectionSubmenu);

            var clearArea = new NativeItem("Clear Area", "Clear all vehicles and peds in a radius around you.");
            clearArea.Activated += (s, e) =>
            {
                var pos = Game.PlayerPed.Position;
                ClearAreaOfVehicles(pos.X, pos.Y, pos.Z, 100f, false, false, false, false, false);
                ClearAreaOfPeds(pos.X, pos.Y, pos.Z, 100f, 0);
                ClearAreaOfObjects(pos.X, pos.Y, pos.Z, 100f, 0);
                Notify.Success("Area cleared.");
            };
            menu.Add(clearArea);

            var killNPCs = new NativeItem("Kill Nearby NPCs", "Kill all NPCs around you.");
            killNPCs.Activated += (s, e) =>
            {
                var playerPos = Game.PlayerPed.Position;
                var peds = World.GetAllPeds();
                var count = 0;
                foreach (var ped in peds)
                {
                    if (ped != Game.PlayerPed && ped.Position.DistanceToSquared(playerPos) < 10000f)
                    {
                        ped.Kill();
                        count++;
                    }
                }
                Notify.Success($"Killed {count} NPCs.");
            };
            menu.Add(killNPCs);

            #endregion
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

        public NativeMenu GetMenu()
        {
            if (menu == null)
            {
                CreateMenu();
            }
            return menu;
        }
    }
}
