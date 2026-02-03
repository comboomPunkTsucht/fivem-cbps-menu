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
    /// Weapon Options Menu - Similar to vMenu's WeaponOptions.cs
    /// </summary>
    public class WeaponMenu
    {
        #region Variables

        public NativeMenu Menu { get; private set; }

        // Checkbox items for state tracking
        private NativeCheckboxItem _infiniteAmmoItem;
        private NativeCheckboxItem _noReloadItem;

        // Category submenus
        private Dictionary<string, NativeMenu> _categoryMenus = new Dictionary<string, NativeMenu>();

        #endregion

        #region Constructor

        public WeaponMenu()
        {
            CreateMenu();
        }

        #endregion

        #region Menu Creation

        private void CreateMenu()
        {
            Menu = ThemeManager.CreateThemedMenu("comboom.sucht", "Weapon Options");

            // Give All Weapons
            var giveAllItem = new NativeItem("Give All Weapons", "Give all available weapons with max ammo");
            giveAllItem.Activated += (sender, args) =>
            {
                foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
                {
                    Game.PlayerPed.Weapons.Give(weapon, 999, false, true);
                }
                Main.ShowNotification("~g~All weapons given!");
            };
            Menu.Add(giveAllItem);

            // Remove All Weapons
            var removeAllItem = new NativeItem("~r~Remove All Weapons", "~r~Remove all weapons from inventory");
            removeAllItem.Activated += (sender, args) =>
            {
                Game.PlayerPed.Weapons.RemoveAll();
                Main.ShowNotification("~r~All weapons removed!");
            };
            Menu.Add(removeAllItem);

            // Refill Ammo
            var refillAmmoItem = new NativeItem("Refill All Ammo", "Refill ammo for all weapons");
            refillAmmoItem.Activated += (sender, args) =>
            {
                var weapon = Game.PlayerPed.Weapons.Current;
                if (weapon != null)
                {
                    weapon.Ammo = weapon.MaxAmmo;
                }
                Main.ShowNotification("~g~All ammo refilled!");
            };
            Menu.Add(refillAmmoItem);

            // Infinite Ammo
            _infiniteAmmoItem = new NativeCheckboxItem("Infinite Ammo", "Toggle infinite ammo for all weapons", false);
            _infiniteAmmoItem.CheckboxChanged += (sender, args) =>
            {
                Main.PlayerManagerInstance.InfiniteAmmo = _infiniteAmmoItem.Checked;
                Main.ShowNotification(_infiniteAmmoItem.Checked ? "~g~Infinite Ammo: ON" : "~r~Infinite Ammo: OFF");
            };
            Menu.Add(_infiniteAmmoItem);

            // No Reload
            _noReloadItem = new NativeCheckboxItem("No Reload", "Toggle no reload for all weapons", false);
            _noReloadItem.CheckboxChanged += (sender, args) =>
            {
                Main.PlayerManagerInstance.NoReload = _noReloadItem.Checked;
                Main.ShowNotification(_noReloadItem.Checked ? "~g~No Reload: ON" : "~r~No Reload: OFF");
            };
            Menu.Add(_noReloadItem);

            // Add separator
            Menu.Add(new NativeSeparatorItem());

            // Spawn Weapon by Name
            var spawnByNameItem = new NativeItem("Spawn Weapon by Name", "Enter a weapon name to spawn");
            spawnByNameItem.Activated += async (sender, args) =>
            {
                var input = await Main.GetUserInput("Enter weapon name (e.g., WEAPON_PISTOL)", "WEAPON_", 50);
                if (!string.IsNullOrEmpty(input))
                {
                    try
                    {
                        var hash = (WeaponHash)API.GetHashKey(input.ToUpper());
                        Game.PlayerPed.Weapons.Give(hash, 999, true, true);
                        Main.ShowNotification($"~g~Weapon spawned: {input}");
                    }
                    catch
                    {
                        Main.ShowNotification("~r~Invalid weapon name!");
                    }
                }
            };
            Menu.Add(spawnByNameItem);

            // Add separator
            Menu.Add(new NativeSeparatorItem());

            // Create category submenus
            foreach (var category in ConfigManager.WeaponCategories)
            {
                CreateCategorySubmenu(category.Key, category.Value);
            }
        }

        private void CreateCategorySubmenu(string categoryName, string[] weapons)
        {
            var submenu = ThemeManager.CreateThemedMenu("comboom.sucht", $"{categoryName} Weapons");

            foreach (var weaponName in weapons)
            {
                var displayName = FormatWeaponName(weaponName);
                var weaponItem = new NativeItem(displayName, $"Give {displayName}");
                weaponItem.Activated += (sender, args) =>
                {
                    try
                    {
                        var hash = (WeaponHash)API.GetHashKey(weaponName);
                        Game.PlayerPed.Weapons.Give(hash, 999, true, true);
                        Main.ShowNotification($"~g~Weapon given: {displayName}");
                    }
                    catch
                    {
                        Main.ShowNotification("~r~Failed to give weapon!");
                    }
                };
                submenu.Add(weaponItem);
            }

            // Add remove weapons option for this category
            var removeItem = new NativeItem($"~r~Remove All {categoryName}", $"~r~Remove all {categoryName.ToLower()} weapons");
            removeItem.Activated += (sender, args) =>
            {
                foreach (var weaponName in weapons)
                {
                    try
                    {
                        var hash = (WeaponHash)API.GetHashKey(weaponName);
                        API.RemoveWeaponFromPed(Game.PlayerPed.Handle, (uint)hash);
                    }
                    catch { }
                }
                Main.ShowNotification($"~r~All {categoryName.ToLower()} weapons removed!");
            };
            submenu.Add(removeItem);

            _categoryMenus[categoryName] = submenu;
            Main.Pool.Add(submenu);

            // Add submenu to main weapon menu
            var submenuItem = Menu.AddSubMenu(submenu);
            submenuItem.Title = $"{categoryName} ({weapons.Length})";
            submenuItem.Description = $"Give or remove {categoryName.ToLower()} weapons";
        }

        #endregion

        #region Helper Methods

        private string FormatWeaponName(string weaponName)
        {
            // Remove "WEAPON_" prefix and format nicely
            var name = weaponName.Replace("WEAPON_", "").Replace("_", " ");
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());
        }

        #endregion
    }
}
