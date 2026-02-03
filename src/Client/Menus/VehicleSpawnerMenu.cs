using System;
using System.Collections.Generic;
using CitizenFX.Core;
using LemonUI.Menus;

using CBPSMenu.Client.Managers;
using CBPSMenu.Shared;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Vehicle Spawner Menu - Similar to vMenu's VehicleSpawner.cs
    /// </summary>
    public class VehicleSpawnerMenu
    {
        #region Variables

        public NativeMenu Menu { get; private set; }

        // Category submenus
        private Dictionary<string, NativeMenu> _categoryMenus = new Dictionary<string, NativeMenu>();

        #endregion

        #region Constructor

        public VehicleSpawnerMenu()
        {
            CreateMenu();
        }

        #endregion

        #region Menu Creation

        private void CreateMenu()
        {
            Menu = ThemeManager.CreateThemedMenu("comboom.sucht", "Vehicle Spawner");

            // Spawn by Model Name
            var spawnByNameItem = new NativeItem("Spawn by Model Name", "Enter a vehicle model name to spawn");
            spawnByNameItem.Activated += async (sender, args) =>
            {
                var input = await Main.GetUserInput("Enter vehicle model", "", 32);
                if (!string.IsNullOrEmpty(input))
                {
                    await Main.VehicleManagerInstance.SpawnVehicle(input);
                }
            };
            Menu.Add(spawnByNameItem);

            // Add separator
            Menu.Add(new NativeSeparatorItem());

            // Create category submenus
            foreach (var category in ConfigManager.VehicleCategories)
            {
                CreateCategorySubmenu(category.Key, category.Value);
            }
        }

        private void CreateCategorySubmenu(string categoryName, string[] vehicles)
        {
            var submenu = ThemeManager.CreateThemedMenu("comboom.sucht", $"{categoryName} Vehicles");

            foreach (var vehicleName in vehicles)
            {
                var vehicleItem = new NativeItem(FormatVehicleName(vehicleName), $"Spawn {vehicleName}");
                vehicleItem.Activated += async (sender, args) =>
                {
                    await Main.VehicleManagerInstance.SpawnVehicle(vehicleName);
                };
                submenu.Add(vehicleItem);
            }

            _categoryMenus[categoryName] = submenu;
            Main.Pool.Add(submenu);

            // Add submenu to main spawner menu
            var submenuItem = Menu.AddSubMenu(submenu);
            submenuItem.Title = $"{categoryName} ({vehicles.Length})";
            submenuItem.Description = $"Spawn a {categoryName.ToLower()} vehicle";
        }

        #endregion

        #region Helper Methods

        private string FormatVehicleName(string modelName)
        {
            // Capitalize first letter of each word
            if (string.IsNullOrEmpty(modelName)) return modelName;

            // Convert to title case and handle numbers
            var result = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(modelName.ToLower());
            return result;
        }

        #endregion
    }
}
