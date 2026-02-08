using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CitizenFX.Core;

using LemonUI;
using LemonUI.Menus;

using CBPSMenu.Client.Data;
using CBPSMenu.Shared;

using static CitizenFX.Core.Native.API;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Weapon Options submenu - vMenu clone.
    /// </summary>
    public class WeaponOptions
    {
        private NativeMenu menu;

        // Weapon state
        public bool UnlimitedAmmo { get; private set; } = false;
        public bool NoReload { get; private set; } = false;

        private void CreateMenu()
        {
            menu = new NativeMenu("Weapon Options", "Weapon Options Menu");

            #region Weapon Spawning

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.WPSpawn))
            {
                // Create submenu for each weapon category
                foreach (var category in WeaponData.WeaponCategories)
                {
                    var categoryMenu = new NativeMenu(category.Key, $"{category.Key} Weapons");
                    var categoryBtn = new NativeItem(category.Key, $"Spawn {category.Key.ToLower()} weapons.") { AltTitle = "→→→" };
                    menu.Add(categoryBtn);

                    foreach (var weapon in category.Value)
                    {
                        var weaponItem = new NativeItem(weapon.Name, $"Give yourself a {weapon.Name}.");
                        weaponItem.Activated += (s, e) =>
                        {
                            var hash = (uint)GetHashKey(weapon.Hash);
                            GiveWeaponToPed(Game.PlayerPed.Handle, hash, 250, false, true);
                            Notify.Success($"Spawned {weapon.Name}.");
                        };
                        categoryMenu.Add(weaponItem);
                    }
                }
            }

            #endregion

            #region Get All Weapons

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.WPGetAll))
            {
                var getAllWeapons = new NativeItem("Get All Weapons", "Give yourself all weapons with max ammo.");
                getAllWeapons.Activated += (s, e) =>
                {
                    foreach (var category in WeaponData.WeaponCategories)
                    {
                        foreach (var weapon in category.Value)
                        {
                            var hash = (uint)GetHashKey(weapon.Hash);
                            GiveWeaponToPed(Game.PlayerPed.Handle, hash, 9999, false, true);
                        }
                    }
                    Notify.Success("All weapons given.");
                };
                menu.Add(getAllWeapons);
            }

            #endregion

            #region Remove All Weapons

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.WPRemoveAll))
            {
                var removeAllWeapons = new NativeItem("~r~Remove All Weapons", "Remove all your weapons.");
                removeAllWeapons.Activated += (s, e) =>
                {
                    RemoveAllPedWeapons(Game.PlayerPed.Handle, true);
                    Notify.Success("All weapons removed.");
                };
                menu.Add(removeAllWeapons);
            }

            #endregion

            #region Unlimited Ammo

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.WPUnlimitedAmmo))
            {
                var unlimitedAmmo = new NativeCheckboxItem("Unlimited Ammo", "Never run out of ammo.", UnlimitedAmmo);
                unlimitedAmmo.CheckboxChanged += (s, e) =>
                {
                    UnlimitedAmmo = unlimitedAmmo.Checked;
                    SetPedInfiniteAmmo(Game.PlayerPed.Handle, UnlimitedAmmo, 0);
                };
                menu.Add(unlimitedAmmo);
            }

            #endregion

            #region No Reload

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.WPNoReload))
            {
                var noReload = new NativeCheckboxItem("No Reload", "Never reload your weapons.", NoReload);
                noReload.CheckboxChanged += (s, e) =>
                {
                    NoReload = noReload.Checked;
                    SetPedInfiniteAmmoClip(Game.PlayerPed.Handle, NoReload);
                };
                menu.Add(noReload);
            }

            #endregion

            #region Set/Refill Ammo

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.WPSetAmmo))
            {
                var refillAmmo = new NativeItem("Refill All Ammo", "Refill ammo for all weapons.");
                refillAmmo.Activated += (s, e) =>
                {
                    foreach (var category in WeaponData.WeaponCategories)
                    {
                        foreach (var weapon in category.Value)
                        {
                            var hash = (uint)GetHashKey(weapon.Hash);
                            if (HasPedGotWeapon(Game.PlayerPed.Handle, hash, false))
                            {
                                var maxAmmo = 0;
                                GetMaxAmmo(Game.PlayerPed.Handle, hash, ref maxAmmo);
                                SetPedAmmo(Game.PlayerPed.Handle, hash, maxAmmo);
                            }
                        }
                    }
                    Notify.Success("All ammo refilled.");
                };
                menu.Add(refillAmmo);
            }

            #endregion

            #region Spawn Ammo

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.WPSpawnAmmo))
            {
                var spawnAmmo = new NativeItem("Spawn Ammo For Current Weapon", "Spawn ammo for your current weapon.");
                spawnAmmo.Activated += (s, e) =>
                {
                    var currentWeapon = (uint)0;
                    if (GetCurrentPedWeapon(Game.PlayerPed.Handle, ref currentWeapon, true))
                    {
                        if (currentWeapon != (uint)GetHashKey("WEAPON_UNARMED"))
                        {
                            var maxAmmo = 0;
                            GetMaxAmmo(Game.PlayerPed.Handle, currentWeapon, ref maxAmmo);
                            SetPedAmmo(Game.PlayerPed.Handle, currentWeapon, maxAmmo);
                            Notify.Success("Ammo spawned for current weapon.");
                        }
                        else
                        {
                            Notify.Error("You need to be holding a weapon.");
                        }
                    }
                };
                menu.Add(spawnAmmo);
            }

            #endregion

            #region Weapon Tints

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.WPTints))
            {
                var tintList = new NativeListItem<string>("Set Weapon Tint", "Change the tint of your current weapon.", WeaponData.WeaponTints.ToArray());
                tintList.ItemChanged += (s, e) =>
                {
                    var currentWeapon = (uint)0;
                    if (GetCurrentPedWeapon(Game.PlayerPed.Handle, ref currentWeapon, true))
                    {
                        if (currentWeapon != (uint)GetHashKey("WEAPON_UNARMED"))
                        {
                            var tintIndex = WeaponData.WeaponTints.IndexOf(e.Object);
                            SetPedWeaponTintIndex(Game.PlayerPed.Handle, currentWeapon, tintIndex);
                            Notify.Success($"Tint set to {e.Object}.");
                        }
                        else
                        {
                            Notify.Error("You need to be holding a weapon.");
                        }
                    }
                };
                menu.Add(tintList);
            }

            #endregion

            #region Drop Current Weapon

            var dropWeapon = new NativeItem("Drop Current Weapon", "Drop the weapon you're currently holding.");
            dropWeapon.Activated += (s, e) =>
            {
                var currentWeapon = (uint)0;
                if (GetCurrentPedWeapon(Game.PlayerPed.Handle, ref currentWeapon, true))
                {
                    if (currentWeapon != (uint)GetHashKey("WEAPON_UNARMED"))
                    {
                        SetPedDropsWeapon(Game.PlayerPed.Handle);
                        Notify.Info("Weapon dropped.");
                    }
                    else
                    {
                        Notify.Error("You're not holding a weapon.");
                    }
                }
            };
            menu.Add(dropWeapon);

            #endregion

            #region Set Weapon Components (attachments placeholder)

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.WPComponents))
            {
                var componentsInfo = new NativeItem("~o~Weapon Components", "Weapon attachment system - use native mod menu for attachments.");
                menu.Add(componentsInfo);
            }

            #endregion
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
