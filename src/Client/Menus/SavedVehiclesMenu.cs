using System;
using System.Collections.Generic;
using CitizenFX.Core;
using LemonUI.Menus;

using CBPSMenu.Client.Managers;
using CBPSMenu.Shared;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Saved Vehicles Menu - Similar to vMenu's SavedVehicles.cs
    /// </summary>
    public class SavedVehiclesMenu
    {
        #region Variables

        public NativeMenu Menu { get; private set; }

        #endregion

        #region Constructor

        public SavedVehiclesMenu()
        {
            CreateMenu();
        }

        #endregion

        #region Menu Creation

        private void CreateMenu()
        {
            Menu = ThemeManager.CreateThemedMenu("comboom.sucht", "Saved Vehicles");

            // Save Current Vehicle
            var saveItem = new NativeItem("Save Current Vehicle", "Save your current vehicle configuration");
            saveItem.Activated += async (sender, args) =>
            {
                var vehicle = Game.PlayerPed.CurrentVehicle;
                if (vehicle == null)
                {
                    Main.ShowNotification("~r~You are not in a vehicle!");
                    return;
                }

                var name = await Main.GetUserInput("Enter vehicle name", "", 32);
                if (!string.IsNullOrEmpty(name))
                {
                    Main.VehicleManagerInstance.SaveCurrentVehicle(name);
                    RefreshSavedVehiclesList();
                }
            };
            Menu.Add(saveItem);

            // Add separator
            Menu.Add(new NativeSeparatorItem());

            // Saved Vehicles List header
            var listHeader = new NativeItem("~b~Saved Vehicles List", "Your saved vehicles are listed below");
            listHeader.Enabled = false;
            Menu.Add(listHeader);

            // Populate saved vehicles list
            RefreshSavedVehiclesList();
        }

        /// <summary>
        /// Refresh the list of saved vehicles in the menu
        /// </summary>
        public void RefreshSavedVehiclesList()
        {
            // Remove existing vehicle items (keep first 3 items: save, separator, header)
            while (Menu.Items.Count > 3)
            {
                Menu.Remove(Menu.Items[Menu.Items.Count - 1]);
            }

            // Get saved vehicles
            var savedVehicles = Main.VehicleManagerInstance.GetSavedVehicleNames();

            if (savedVehicles.Count == 0)
            {
                var noVehiclesItem = new NativeItem("No saved vehicles", "Save a vehicle to see it here");
                noVehiclesItem.Enabled = false;
                Menu.Add(noVehiclesItem);
                return;
            }

            foreach (var vehicleName in savedVehicles)
            {
                AddSavedVehicleItem(vehicleName);
            }
        }

        private void AddSavedVehicleItem(string vehicleName)
        {
            // Create a submenu for each saved vehicle
            var vehicleSubmenu = ThemeManager.CreateThemedMenu("comboom.sucht", vehicleName);

            // Spawn Vehicle
            var spawnItem = new NativeItem("Spawn Vehicle", $"Spawn {vehicleName}");
            spawnItem.Activated += async (sender, args) =>
            {
                await Main.VehicleManagerInstance.SpawnSavedVehicle(vehicleName);
            };
            vehicleSubmenu.Add(spawnItem);

            // Rename Vehicle
            var renameItem = new NativeItem("Rename Vehicle", $"Rename {vehicleName}");
            renameItem.Activated += async (sender, args) =>
            {
                var newName = await Main.GetUserInput("Enter new name", vehicleName, 32);
                if (!string.IsNullOrEmpty(newName) && newName != vehicleName)
                {
                    // Save under new name and delete old
                    await Main.VehicleManagerInstance.SpawnSavedVehicle(vehicleName);
                    Main.VehicleManagerInstance.SaveCurrentVehicle(newName);
                    Main.VehicleManagerInstance.DeleteSavedVehicle(vehicleName);
                    Main.VehicleManagerInstance.DeleteVehicle();
                    RefreshSavedVehiclesList();
                    Main.ShowNotification($"~g~Renamed to: {newName}");
                }
            };
            vehicleSubmenu.Add(renameItem);

            // Delete Vehicle
            var deleteItem = new NativeItem("~r~Delete Vehicle", $"~r~Delete {vehicleName}");
            deleteItem.Activated += (sender, args) =>
            {
                Main.VehicleManagerInstance.DeleteSavedVehicle(vehicleName);
                RefreshSavedVehiclesList();
                Menu.Visible = true;
            };
            vehicleSubmenu.Add(deleteItem);

            // Add submenu to main menu
            Main.Pool.Add(vehicleSubmenu);
            var submenuItem = Menu.AddSubMenu(vehicleSubmenu);
            submenuItem.Title = vehicleName;
            submenuItem.Description = $"Manage saved vehicle: {vehicleName}";
        }

        #endregion
    }
}
