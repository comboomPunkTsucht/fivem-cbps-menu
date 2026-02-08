using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CitizenFX.Core;

using LemonUI;
using LemonUI.Menus;

using CBPSMenu.Shared;
using CBPSMenu.Client.Data;
using Newtonsoft.Json;

using static CitizenFX.Core.Native.API;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Weapon Loadouts submenu - vMenu clone.
    /// </summary>
    public class WeaponLoadouts
    {
        private NativeMenu menu;
        private List<WeaponLoadoutData> loadouts = new List<WeaponLoadoutData>();

        private void CreateMenu()
        {
            menu = new NativeMenu("Weapon Loadouts", "Manage Weapon Loadouts");

            LoadLoadouts();
            RefreshMenu();
        }

        private void RefreshMenu()
        {
            menu.Clear();

            #region Save Current Loadout

            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.WLSave))
            {
                var saveLoadout = new NativeItem("~g~Save Current Loadout", "Save your current weapons as a loadout.");
                saveLoadout.Activated += async (s, e) =>
                {
                    var name = await GetUserInput("Enter loadout name", "", 30);
                    if (!string.IsNullOrEmpty(name))
                    {
                        var weapons = new List<string>();

                        // Get all weapons the player has
                        foreach (var category in WeaponData.WeaponCategories)
                        {
                            foreach (var weapon in category.Value)
                            {
                                var hash = (uint)GetHashKey(weapon.Hash);
                                if (HasPedGotWeapon(Game.PlayerPed.Handle, hash, false))
                                {
                                    weapons.Add(weapon.Hash);
                                }
                            }
                        }

                        if (weapons.Count == 0)
                        {
                            Notify.Error("You have no weapons to save.");
                            return;
                        }

                        var loadout = new WeaponLoadoutData
                        {
                            Name = name,
                            Weapons = weapons
                        };

                        loadouts.Add(loadout);
                        SaveLoadoutsToKvp();
                        RefreshMenu();
                        Notify.Success($"Loadout '{name}' saved with {weapons.Count} weapons.");
                    }
                };
                menu.Add(saveLoadout);
            }

            #endregion

            #region Loadout List

            if (loadouts.Count == 0)
            {
                var noLoadouts = new NativeItem("~c~No Saved Loadouts", "Save a loadout to see it here.");
                menu.Add(noLoadouts);
            }
            else
            {
                foreach (var loadout in loadouts)
                {
                    var loadoutSubmenu = new NativeMenu(loadout.Name, $"Manage: {loadout.Name}");
                    var loadoutBtn = new NativeItem(loadout.Name, $"{loadout.Weapons.Count} weapons") { AltTitle = "→→→" };
                    menu.Add(loadoutBtn);

                    // Equip loadout
                    var equipBtn = new NativeItem("Equip Loadout", "Give yourself all weapons in this loadout.");
                    equipBtn.Activated += (s, e) =>
                    {
                        EquipLoadout(loadout);
                    };
                    loadoutSubmenu.Add(equipBtn);

                    // Rename
                    var renameBtn = new NativeItem("Rename", "Rename this loadout.");
                    renameBtn.Activated += async (s, e) =>
                    {
                        var newName = await GetUserInput("Enter new name", loadout.Name, 30);
                        if (!string.IsNullOrEmpty(newName))
                        {
                            loadout.Name = newName;
                            SaveLoadoutsToKvp();
                            RefreshMenu();
                            Notify.Success($"Renamed to '{newName}'.");
                        }
                    };
                    loadoutSubmenu.Add(renameBtn);

                    // Delete
                    var deleteBtn = new NativeItem("~r~Delete", "Delete this loadout.");
                    deleteBtn.Activated += (s, e) =>
                    {
                        loadouts.Remove(loadout);
                        SaveLoadoutsToKvp();
                        RefreshMenu();
                        Notify.Success($"Deleted '{loadout.Name}'.");
                    };
                    loadoutSubmenu.Add(deleteBtn);
                }
            }

            #endregion

            #region Delete All

            if (loadouts.Count > 0)
            {
                var deleteAll = new NativeItem("~r~Delete All Loadouts", "Warning: Cannot be undone!");
                deleteAll.Activated += (s, e) =>
                {
                    loadouts.Clear();
                    SaveLoadoutsToKvp();
                    RefreshMenu();
                    Notify.Success("All loadouts deleted.");
                };
                menu.Add(deleteAll);
            }

            #endregion
        }

        private void EquipLoadout(WeaponLoadoutData loadout)
        {
            foreach (var weaponHash in loadout.Weapons)
            {
                var hash = (uint)GetHashKey(weaponHash);
                GiveWeaponToPed(Game.PlayerPed.Handle, hash, 250, false, true);
            }
            Notify.Success($"Equipped '{loadout.Name}' ({loadout.Weapons.Count} weapons).");
        }

        private void LoadLoadouts()
        {
            var json = GetResourceKvpString("cbps_weapon_loadouts");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    loadouts = JsonConvert.DeserializeObject<List<WeaponLoadoutData>>(json) ?? new List<WeaponLoadoutData>();
                }
                catch
                {
                    loadouts = new List<WeaponLoadoutData>();
                }
            }
        }

        private void SaveLoadoutsToKvp()
        {
            var json = JsonConvert.SerializeObject(loadouts);
            SetResourceKvp("cbps_weapon_loadouts", json);
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

    public class WeaponLoadoutData
    {
        public string Name { get; set; }
        public List<string> Weapons { get; set; } = new List<string>();
    }
}
