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
    /// Saved Vehicles submenu - vMenu clone.
    /// </summary>
    public class SavedVehicles
    {
        private NativeMenu menu;
        private List<SavedVehicleData> savedVehicles = new List<SavedVehicleData>();

        private void CreateMenu()
        {
            menu = new NativeMenu("Saved Vehicles", "Your Saved Vehicles");

            LoadSavedVehicles();
            RefreshMenu();
        }

        private void RefreshMenu()
        {
            menu.Clear();

            #region Save Current Vehicle

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.SVSaveVehicle))
            {
                var saveVehicle = new NativeItem("~g~Save Current Vehicle", "Save your current vehicle.");
                saveVehicle.Activated += async (s, e) =>
                {
                    if (!Game.PlayerPed.IsInVehicle())
                    {
                        Notify.Error("You must be in a vehicle.");
                        return;
                    }

                    var name = await GetUserInput("Enter vehicle name", "", 30);
                    if (!string.IsNullOrEmpty(name))
                    {
                        var vehicle = Game.PlayerPed.CurrentVehicle;
                        var data = new SavedVehicleData
                        {
                            Name = name,
                            Model = vehicle.DisplayName,
                            Hash = (uint)vehicle.Model.Hash,
                            PrimaryColor = (int)vehicle.Mods.PrimaryColor,
                            SecondaryColor = (int)vehicle.Mods.SecondaryColor,
                            PlateText = vehicle.Mods.LicensePlate
                        };

                        savedVehicles.Add(data);
                        SaveVehiclesToKvp();
                        RefreshMenu();
                        Notify.Success($"Vehicle saved as '{name}'.");
                    }
                };
                menu.Add(saveVehicle);
            }

            #endregion

            #region Saved Vehicles List

            if (savedVehicles.Count == 0)
            {
                var noVehicles = new NativeItem("~c~No Saved Vehicles", "Save a vehicle to see it here.");
                menu.Add(noVehicles);
            }
            else
            {
                foreach (var vehicle in savedVehicles)
                {
                    var vehicleSubmenu = new NativeMenu(vehicle.Name, $"Manage: {vehicle.Name}");
                    var vehicleBtn = new NativeItem(vehicle.Name, $"Model: {vehicle.Model}") { AltTitle = "→→→" };
                    menu.Add(vehicleBtn);

                    // Spawn option
                    var spawnBtn = new NativeItem("Spawn Vehicle", "Spawn this saved vehicle.");
                    spawnBtn.Activated += async (s, e) =>
                    {
                        await SpawnSavedVehicle(vehicle);
                    };
                    vehicleSubmenu.Add(spawnBtn);

                    // Rename option
                    var renameBtn = new NativeItem("Rename", "Change the vehicle name.");
                    renameBtn.Activated += async (s, e) =>
                    {
                        var newName = await GetUserInput("Enter new name", vehicle.Name, 30);
                        if (!string.IsNullOrEmpty(newName))
                        {
                            vehicle.Name = newName;
                            SaveVehiclesToKvp();
                            RefreshMenu();
                            Notify.Success($"Renamed to '{newName}'.");
                        }
                    };
                    vehicleSubmenu.Add(renameBtn);

                    // Delete option
                    var deleteBtn = new NativeItem("~r~Delete", "Delete this saved vehicle.");
                    deleteBtn.Activated += (s, e) =>
                    {
                        savedVehicles.Remove(vehicle);
                        SaveVehiclesToKvp();
                        RefreshMenu();
                        Notify.Success($"Deleted '{vehicle.Name}'.");
                    };
                    vehicleSubmenu.Add(deleteBtn);
                }
            }

            #endregion

            #region Delete All

            if (savedVehicles.Count > 0)
            {
                var deleteAll = new NativeItem("~r~Delete All Saved Vehicles", "Warning: This cannot be undone!");
                deleteAll.Activated += (s, e) =>
                {
                    savedVehicles.Clear();
                    SaveVehiclesToKvp();
                    RefreshMenu();
                    Notify.Success("All saved vehicles deleted.");
                };
                menu.Add(deleteAll);
            }

            #endregion
        }

        private async Task SpawnSavedVehicle(SavedVehicleData data)
        {
            var modelHash = data.Hash;

            if (!IsModelInCdimage(modelHash))
            {
                Notify.Error($"Model not found.");
                return;
            }

            RequestModel(modelHash);
            while (!HasModelLoaded(modelHash))
            {
                await BaseScript.Delay(0);
            }

            var pos = Game.PlayerPed.Position;
            var heading = Game.PlayerPed.Heading;

            var vehicle = new Vehicle(CreateVehicle(modelHash, pos.X, pos.Y, pos.Z, heading, true, false));
            vehicle.PlaceOnGround();
            vehicle.NeedsToBeHotwired = false;
            vehicle.PreviouslyOwnedByPlayer = true;
            vehicle.IsPersistent = true;

            // Apply saved properties
            vehicle.Mods.PrimaryColor = (VehicleColor)data.PrimaryColor;
            vehicle.Mods.SecondaryColor = (VehicleColor)data.SecondaryColor;
            if (!string.IsNullOrEmpty(data.PlateText))
            {
                vehicle.Mods.LicensePlate = data.PlateText;
            }

            SetModelAsNoLongerNeeded(modelHash);

            Game.PlayerPed.SetIntoVehicle(vehicle, VehicleSeat.Driver);
            Notify.Success($"Spawned '{data.Name}'.");
        }

        private void LoadSavedVehicles()
        {
            var dataString = GetResourceKvpString("cbps_saved_vehicles");
            savedVehicles.Clear();

            if (!string.IsNullOrEmpty(dataString))
            {
                var entries = dataString.Split(new[] { ";;" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var entry in entries)
                {
                    var parts = entry.Split('|');
                    if (parts.Length >= 6)
                    {
                        try
                        {
                            var vehicle = new SavedVehicleData
                            {
                                Name = parts[0],
                                Model = parts[1],
                                Hash = uint.Parse(parts[2]),
                                PrimaryColor = int.Parse(parts[3]),
                                SecondaryColor = int.Parse(parts[4]),
                                PlateText = parts[5]
                            };
                            savedVehicles.Add(vehicle);
                        }
                        catch { }
                    }
                }
            }
        }

        private void SaveVehiclesToKvp()
        {
            var entries = new List<string>();
            foreach (var veh in savedVehicles)
            {
                entries.Add($"{veh.Name}|{veh.Model}|{veh.Hash}|{veh.PrimaryColor}|{veh.SecondaryColor}|{veh.PlateText}");
            }
            SetResourceKvp("cbps_saved_vehicles", string.Join(";;", entries));
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

    public class SavedVehicleData
    {
        public string Name { get; set; }
        public string Model { get; set; }
        public uint Hash { get; set; }
        public int PrimaryColor { get; set; }
        public int SecondaryColor { get; set; }
        public string PlateText { get; set; }
    }
}
