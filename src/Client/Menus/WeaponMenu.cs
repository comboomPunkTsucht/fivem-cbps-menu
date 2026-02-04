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
    /// Weapon Options Menu - Enhanced with vMenu features
    /// Based on vMenu/vMenu/menus/WeaponOptions.cs
    /// </summary>
    public class WeaponMenu
    {
        #region Variables

        public NativeMenu Menu { get; private set; }

        // Checkbox items for state tracking
        private NativeCheckboxItem _infiniteAmmoItem;
        private NativeCheckboxItem _noReloadItem;
        private NativeCheckboxItem _unlimitedParachutesItem;
        private NativeCheckboxItem _autoEquipParachuteItem;

        // Category submenus
        private Dictionary<string, NativeMenu> _categoryMenus = new Dictionary<string, NativeMenu>();

        // Weapon Tints
        private static readonly string[] WeaponTints =
        {
            "Normal", "Green", "Gold", "Pink", "Army", "LSPD", "Orange", "Platinum"
        };

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

            // === MAIN ACTIONS ===
            var actionsHeader = new NativeItem("~b~=== Quick Actions ===", "Fast weapon actions")
            {
                Enabled = false
            };
            Menu.Add(actionsHeader);

            // Give All Weapons
            var giveAllItem = new NativeItem("Give All Weapons", "Give all available weapons with max ammo");
            giveAllItem.Activated += (sender, args) =>
            {
                foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
                {
                    int maxAmmo = 0;
                    API.GetMaxAmmo(Game.PlayerPed.Handle, (uint)weapon, ref maxAmmo);
                    API.GiveWeaponToPed(Game.PlayerPed.Handle, (uint)weapon, maxAmmo, false, true);
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

            // Refill All Ammo
            var refillAmmoItem = new NativeItem("Refill All Ammo", "Refill ammo for all weapons");
            refillAmmoItem.Activated += (sender, args) =>
            {
                foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
                {
                    if (API.HasPedGotWeapon(Game.PlayerPed.Handle, (uint)weapon, false))
                    {
                        int maxAmmo = 0;
                        API.GetMaxAmmo(Game.PlayerPed.Handle, (uint)weapon, ref maxAmmo);
                        API.SetPedAmmo(Game.PlayerPed.Handle, (uint)weapon, maxAmmo);
                    }
                }
                Main.ShowNotification("~g~All ammo refilled!");
            };
            Menu.Add(refillAmmoItem);

            // Set All Ammo Count
            var setAmmoItem = new NativeItem("Set All Ammo Count", "Set ammo count for all weapons");
            setAmmoItem.Activated += async (sender, args) =>
            {
                var input = await Main.GetUserInput("Enter ammo count", "999", 10);
                if (!string.IsNullOrEmpty(input) && int.TryParse(input, out int ammo))
                {
                    foreach (WeaponHash weapon in Enum.GetValues(typeof(WeaponHash)))
                    {
                        if (API.HasPedGotWeapon(Game.PlayerPed.Handle, (uint)weapon, false))
                        {
                            API.SetPedAmmo(Game.PlayerPed.Handle, (uint)weapon, ammo);
                        }
                    }
                    Main.ShowNotification($"~g~Set all ammo to: {ammo}");
                }
            };
            Menu.Add(setAmmoItem);

            Menu.Add(new NativeSeparatorItem());

            // === WEAPON TOGGLES ===
            var togglesHeader = new NativeItem("~b~=== Weapon Toggles ===", "Toggle weapon abilities")
            {
                Enabled = false
            };
            Menu.Add(togglesHeader);

            // Infinite Ammo
            _infiniteAmmoItem = new NativeCheckboxItem("Infinite Ammo", "Toggle infinite ammo for all weapons", false);
            _infiniteAmmoItem.CheckboxChanged += (sender, args) =>
            {
                Main.PlayerManagerInstance.InfiniteAmmo = _infiniteAmmoItem.Checked;
                Main.ShowNotification(_infiniteAmmoItem.Checked ? "~g~Infinite Ammo: ON" : "~r~Infinite Ammo: OFF");
            };
            Menu.Add(_infiniteAmmoItem);

            // No Reload
            _noReloadItem = new NativeCheckboxItem("No Reload", "Never need to reload weapons", false);
            _noReloadItem.CheckboxChanged += (sender, args) =>
            {
                Main.PlayerManagerInstance.NoReload = _noReloadItem.Checked;
                Main.ShowNotification(_noReloadItem.Checked ? "~g~No Reload: ON" : "~r~No Reload: OFF");
            };
            Menu.Add(_noReloadItem);

            Menu.Add(new NativeSeparatorItem());

            // === SPAWN WEAPON ===
            var spawnHeader = new NativeItem("~b~=== Spawn Weapons ===", "Spawn specific weapons")
            {
                Enabled = false
            };
            Menu.Add(spawnHeader);

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
                        int maxAmmo = 0;
                        API.GetMaxAmmo(Game.PlayerPed.Handle, (uint)hash, ref maxAmmo);
                        API.GiveWeaponToPed(Game.PlayerPed.Handle, (uint)hash, maxAmmo, false, true);
                        Main.ShowNotification($"~g~Weapon spawned: {input}");
                    }
                    catch
                    {
                        Main.ShowNotification("~r~Invalid weapon name!");
                    }
                }
            };
            Menu.Add(spawnByNameItem);

            Menu.Add(new NativeSeparatorItem());

            // === CURRENT WEAPON OPTIONS ===
            var currentWeaponHeader = new NativeItem("~b~=== Current Weapon ===", "Options for equipped weapon")
            {
                Enabled = false
            };
            Menu.Add(currentWeaponHeader);

            // Set Weapon Tint
            var tintItem = new NativeListItem<string>("Weapon Tint", "Change tint of current weapon", WeaponTints);
            tintItem.ItemChanged += (sender, args) =>
            {
                var currentWeapon = Game.PlayerPed.Weapons.Current;
                if (currentWeapon != null && currentWeapon.Hash != WeaponHash.Unarmed)
                {
                    API.SetPedWeaponTintIndex(Game.PlayerPed.Handle, (uint)currentWeapon.Hash, tintItem.SelectedIndex);
                    Main.ShowNotification($"~b~Tint set to: {WeaponTints[tintItem.SelectedIndex]}");
                }
            };
            Menu.Add(tintItem);

            // Drop Current Weapon
            var dropWeaponItem = new NativeItem("Drop Current Weapon", "Drop your currently held weapon");
            dropWeaponItem.Activated += (sender, args) =>
            {
                var currentWeapon = Game.PlayerPed.Weapons.Current;
                if (currentWeapon != null && currentWeapon.Hash != WeaponHash.Unarmed)
                {
                    API.SetPedDropsWeapon(Game.PlayerPed.Handle);
                    Main.ShowNotification("~y~Weapon dropped!");
                }
                else
                {
                    Main.ShowNotification("~r~No weapon to drop!");
                }
            };
            Menu.Add(dropWeaponItem);

            // Remove Current Weapon
            var removeWeaponItem = new NativeItem("~r~Remove Current Weapon", "~r~Remove your equipped weapon");
            removeWeaponItem.Activated += (sender, args) =>
            {
                var currentWeapon = Game.PlayerPed.Weapons.Current;
                if (currentWeapon != null && currentWeapon.Hash != WeaponHash.Unarmed)
                {
                    API.RemoveWeaponFromPed(Game.PlayerPed.Handle, (uint)currentWeapon.Hash);
                    Main.ShowNotification("~r~Weapon removed!");
                }
                else
                {
                    Main.ShowNotification("~r~No weapon to remove!");
                }
            };
            Menu.Add(removeWeaponItem);

            Menu.Add(new NativeSeparatorItem());

            // === PARACHUTE OPTIONS ===
            var parachuteHeader = new NativeItem("~b~=== Parachute Options ===", "Parachute settings")
            {
                Enabled = false
            };
            Menu.Add(parachuteHeader);

            // Give Parachute
            var giveParachuteItem = new NativeItem("Give Parachute", "Add primary parachute");
            giveParachuteItem.Activated += (sender, args) =>
            {
                uint parachuteHash = (uint)API.GetHashKey("gadget_parachute");
                if (!API.HasPedGotWeapon(Game.PlayerPed.Handle, parachuteHash, false))
                {
                    API.GiveWeaponToPed(Game.PlayerPed.Handle, parachuteHash, 1, false, true);
                    Main.ShowNotification("~g~Parachute added!");
                }
                else
                {
                    Main.ShowNotification("~y~You already have a parachute!");
                }
            };
            Menu.Add(giveParachuteItem);

            // Enable Reserve Parachute
            var reserveParachuteItem = new NativeItem("Enable Reserve Parachute", "Add reserve parachute");
            reserveParachuteItem.Activated += (sender, args) =>
            {
                API.SetPlayerHasReserveParachute(Game.Player.Handle);
                Main.ShowNotification("~g~Reserve parachute enabled!");
            };
            Menu.Add(reserveParachuteItem);

            // Unlimited Parachutes
            _unlimitedParachutesItem = new NativeCheckboxItem("Unlimited Parachutes", "Never run out of parachutes", false);
            _unlimitedParachutesItem.CheckboxChanged += (sender, args) =>
            {
                Main.ShowNotification(_unlimitedParachutesItem.Checked ? "~g~Unlimited Parachutes: ON" : "~r~Unlimited Parachutes: OFF");
            };
            Menu.Add(_unlimitedParachutesItem);

            // Auto Equip Parachute
            _autoEquipParachuteItem = new NativeCheckboxItem("Auto Equip Parachute", "Auto-equip when in aircraft", false);
            _autoEquipParachuteItem.CheckboxChanged += (sender, args) =>
            {
                Main.ShowNotification(_autoEquipParachuteItem.Checked ? "~g~Auto Equip Parachute: ON" : "~r~Auto Equip Parachute: OFF");
            };
            Menu.Add(_autoEquipParachuteItem);

            // Parachute Tint
            var parachuteTints = new string[] { "Rainbow", "Red", "Seaside Stripes", "Widow Maker", "Patriot", "Blue", "Black", "Hornet", "Air Force", "Desert", "Shadow", "High Altitude", "Airborne", "Sunrise" };
            var parachuteTintItem = new NativeListItem<string>("Parachute Style", "Change parachute appearance", parachuteTints);
            parachuteTintItem.ItemChanged += (sender, args) =>
            {
                API.SetPlayerParachuteTintIndex(Game.Player.Handle, parachuteTintItem.SelectedIndex);
                Main.ShowNotification($"~b~Parachute style: {parachuteTints[parachuteTintItem.SelectedIndex]}");
            };
            Menu.Add(parachuteTintItem);

            // Smoke Trail Color
            var smokeColors = new string[] { "No Smoke", "Red", "Orange", "Yellow", "Blue", "Black" };
            var smokeColorItem = new NativeListItem<string>("Smoke Trail Color", "Change parachute smoke color", smokeColors);
            smokeColorItem.Activated += (sender, args) =>
            {
                int[][] colors = new int[][]
                {
                    new int[] { 255, 255, 255 },
                    new int[] { 255, 0, 0 },
                    new int[] { 255, 165, 0 },
                    new int[] { 255, 255, 0 },
                    new int[] { 0, 0, 255 },
                    new int[] { 20, 20, 20 }
                };

                int index = smokeColorItem.SelectedIndex;
                var color = colors[index];
                API.SetPlayerParachuteSmokeTrailColor(Game.Player.Handle, color[0], color[1], color[2]);
                API.SetPlayerCanLeaveParachuteSmokeTrail(Game.Player.Handle, index != 0);
                Main.ShowNotification($"~b~Smoke trail: {smokeColors[index]}");
            };
            Menu.Add(smokeColorItem);

            Menu.Add(new NativeSeparatorItem());

            // === WEAPON CATEGORIES ===
            var categoryHeader = new NativeItem("~b~=== Weapon Categories ===", "Browse weapons by category")
            {
                Enabled = false
            };
            Menu.Add(categoryHeader);

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

                // Create individual weapon menu
                var weaponSubmenu = ThemeManager.CreateThemedMenu("comboom.sucht", displayName);
                Main.Pool.Add(weaponSubmenu);

                // Equip/Remove
                var equipItem = new NativeItem("Equip/Remove", "Add or remove this weapon");
                equipItem.Activated += (sender, args) =>
                {
                    uint hash = (uint)API.GetHashKey(weaponName);
                    if (API.HasPedGotWeapon(Game.PlayerPed.Handle, hash, false))
                    {
                        API.RemoveWeaponFromPed(Game.PlayerPed.Handle, hash);
                        Main.ShowNotification($"~r~{displayName} removed!");
                    }
                    else
                    {
                        int maxAmmo = 0;
                        API.GetMaxAmmo(Game.PlayerPed.Handle, hash, ref maxAmmo);
                        API.GiveWeaponToPed(Game.PlayerPed.Handle, hash, maxAmmo, false, true);
                        Main.ShowNotification($"~g~{displayName} given!");
                    }
                };
                weaponSubmenu.Add(equipItem);

                // Refill Ammo
                var refillItem = new NativeItem("Refill Ammo", "Get max ammo for this weapon");
                refillItem.Activated += (sender, args) =>
                {
                    uint hash = (uint)API.GetHashKey(weaponName);
                    if (API.HasPedGotWeapon(Game.PlayerPed.Handle, hash, false))
                    {
                        int maxAmmo = 0;
                        API.GetMaxAmmo(Game.PlayerPed.Handle, hash, ref maxAmmo);
                        API.SetPedAmmo(Game.PlayerPed.Handle, hash, maxAmmo);
                        Main.ShowNotification($"~g~{displayName} ammo refilled!");
                    }
                    else
                    {
                        Main.ShowNotification("~r~You don't have this weapon!");
                    }
                };
                weaponSubmenu.Add(refillItem);

                // Weapon Tint
                var weaponTintItem = new NativeListItem<string>("Tint", "Change weapon tint", WeaponTints);
                weaponTintItem.ItemChanged += (sender, args) =>
                {
                    uint hash = (uint)API.GetHashKey(weaponName);
                    if (API.HasPedGotWeapon(Game.PlayerPed.Handle, hash, false))
                    {
                        API.SetPedWeaponTintIndex(Game.PlayerPed.Handle, hash, weaponTintItem.SelectedIndex);
                        Main.ShowNotification($"~b~Tint: {WeaponTints[weaponTintItem.SelectedIndex]}");
                    }
                };
                weaponSubmenu.Add(weaponTintItem);

                // Add weapon to category menu
                var weaponMenuButton = submenu.AddSubMenu(weaponSubmenu);
                weaponMenuButton.Title = displayName;
                weaponMenuButton.Description = $"Options for {displayName}";
            }

            submenu.Add(new NativeSeparatorItem());

            // Give All in Category
            var giveAllCategoryItem = new NativeItem($"Give All {categoryName}", $"Give all {categoryName.ToLower()} weapons");
            giveAllCategoryItem.Activated += (sender, args) =>
            {
                foreach (var weaponName in weapons)
                {
                    uint hash = (uint)API.GetHashKey(weaponName);
                    int maxAmmo = 0;
                    API.GetMaxAmmo(Game.PlayerPed.Handle, hash, ref maxAmmo);
                    API.GiveWeaponToPed(Game.PlayerPed.Handle, hash, maxAmmo, false, true);
                }
                Main.ShowNotification($"~g~All {categoryName.ToLower()} weapons given!");
            };
            submenu.Add(giveAllCategoryItem);

            // Remove All in Category
            var removeAllCategoryItem = new NativeItem($"~r~Remove All {categoryName}", $"~r~Remove all {categoryName.ToLower()} weapons");
            removeAllCategoryItem.Activated += (sender, args) =>
            {
                foreach (var weaponName in weapons)
                {
                    uint hash = (uint)API.GetHashKey(weaponName);
                    API.RemoveWeaponFromPed(Game.PlayerPed.Handle, hash);
                }
                Main.ShowNotification($"~r~All {categoryName.ToLower()} weapons removed!");
            };
            submenu.Add(removeAllCategoryItem);

            _categoryMenus[categoryName] = submenu;
            Main.Pool.Add(submenu);

            // Add submenu to main weapon menu
            var submenuItem = Menu.AddSubMenu(submenu);
            submenuItem.Title = $"{categoryName} ({weapons.Length})";
            submenuItem.Description = $"Browse and manage {categoryName.ToLower()} weapons";
        }

        #endregion

        #region Helper Methods

        private string FormatWeaponName(string weaponName)
        {
            // Remove "WEAPON_" prefix and format nicely
            var name = weaponName.Replace("WEAPON_", "").Replace("_", " ");
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name.ToLower());
        }

        /// <summary>
        /// Check if unlimited parachutes is enabled
        /// </summary>
        public bool IsUnlimitedParachutesEnabled => _unlimitedParachutesItem?.Checked ?? false;

        /// <summary>
        /// Check if auto equip parachute is enabled
        /// </summary>
        public bool IsAutoEquipParachuteEnabled => _autoEquipParachuteItem?.Checked ?? false;

        #endregion
    }
}
