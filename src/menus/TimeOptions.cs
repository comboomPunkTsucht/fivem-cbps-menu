using System;
using System.Collections.Generic;

using CitizenFX.Core;

using LemonUI.Menus;

using CBPSMenu.Shared;

using static CitizenFX.Core.Native.API;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Time Options submenu.
    /// </summary>
    public class TimeOptions
    {
        private NativeMenu menu;

        public bool FreezeTime { get; private set; } = false;
        public int CurrentHour { get; private set; } = 12;

        private void CreateMenu()
        {
            menu = new NativeMenu("Time Options", "Change the time");

            // Freeze Time checkbox
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.TOFreezeTime))
            {
                var freezeTimeCheckbox = new NativeCheckboxItem("Freeze Time", "Freeze the current time.", FreezeTime);
                freezeTimeCheckbox.CheckboxChanged += (sender, e) =>
                {
                    FreezeTime = freezeTimeCheckbox.Checked;
                    BaseScript.TriggerServerEvent("cbps:setTimeFrozen", FreezeTime);
                };
                menu.Add(freezeTimeCheckbox);
            }

            // Set Time list
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.TOSetTime))
            {
                var hours = new List<string>();
                for (int i = 0; i < 24; i++)
                {
                    hours.Add($"{i:00}:00");
                }

                var setTimeList = new NativeListItem<string>("Set Time", "Set the hour of day.", hours.ToArray());
                setTimeList.SelectedIndex = CurrentHour;
                setTimeList.ItemChanged += (sender, e) =>
                {
                    CurrentHour = hours.IndexOf(e.Object);
                    BaseScript.TriggerServerEvent("cbps:setTime", CurrentHour, 0, 0);
                    Notify.Info($"Time set to {hours[CurrentHour]}.");
                };
                menu.Add(setTimeList);

                menu.Add(new NativeSeparatorItem());

                // Quick time presets
                var morningButton = new NativeItem("Morning (06:00)", "Set time to early morning.");
                morningButton.Activated += (sender, e) =>
                {
                    BaseScript.TriggerServerEvent("cbps:setTime", 6, 0, 0);
                    Notify.Info("Time set to morning.");
                };
                menu.Add(morningButton);

                var noonButton = new NativeItem("Noon (12:00)", "Set time to midday.");
                noonButton.Activated += (sender, e) =>
                {
                    BaseScript.TriggerServerEvent("cbps:setTime", 12, 0, 0);
                    Notify.Info("Time set to noon.");
                };
                menu.Add(noonButton);

                var eveningButton = new NativeItem("Evening (18:00)", "Set time to evening.");
                eveningButton.Activated += (sender, e) =>
                {
                    BaseScript.TriggerServerEvent("cbps:setTime", 18, 0, 0);
                    Notify.Info("Time set to evening.");
                };
                menu.Add(eveningButton);

                var nightButton = new NativeItem("Night (00:00)", "Set time to midnight.");
                nightButton.Activated += (sender, e) =>
                {
                    BaseScript.TriggerServerEvent("cbps:setTime", 0, 0, 0);
                    Notify.Info("Time set to night.");
                };
                menu.Add(nightButton);
            }
        }

        /// <summary>
        /// Gets the menu, creating it if necessary.
        /// </summary>
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
