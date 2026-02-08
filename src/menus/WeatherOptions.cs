using System;
using System.Collections.Generic;

using CitizenFX.Core;

using LemonUI.Menus;

using CBPSMenu.Shared;

using static CitizenFX.Core.Native.API;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Weather Options submenu.
    /// </summary>
    public class WeatherOptions
    {
        private NativeMenu menu;

        public bool DynamicWeather { get; private set; } = true;
        public bool Blackout { get; private set; } = false;

        private static readonly List<string> WeatherTypes = new List<string>
        {
            "EXTRASUNNY",
            "CLEAR",
            "NEUTRAL",
            "SMOG",
            "FOGGY",
            "OVERCAST",
            "CLOUDS",
            "CLEARING",
            "RAIN",
            "THUNDER",
            "SNOW",
            "BLIZZARD",
            "SNOWLIGHT",
            "XMAS",
            "HALLOWEEN"
        };

        private void CreateMenu()
        {
            menu = new NativeMenu("Weather Options", "Change the weather");

            // Dynamic Weather checkbox
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.WODynamic))
            {
                var dynamicCheckbox = new NativeCheckboxItem("Dynamic Weather", "Enable dynamic weather changes.", DynamicWeather);
                dynamicCheckbox.CheckboxChanged += (sender, e) =>
                {
                    DynamicWeather = dynamicCheckbox.Checked;
                    TriggerServerEvent("cbps:setDynamicWeather", DynamicWeather);
                };
                menu.Add(dynamicCheckbox);
            }

            // Blackout checkbox
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.WOBlackout))
            {
                var blackoutCheckbox = new NativeCheckboxItem("Blackout", "Disable all city lights.", Blackout);
                blackoutCheckbox.CheckboxChanged += (sender, e) =>
                {
                    Blackout = blackoutCheckbox.Checked;
                    SetArtificialLightsState(Blackout);
                };
                menu.Add(blackoutCheckbox);
            }

            // Weather type selector
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.WOSetWeather))
            {
                menu.Add(new NativeSeparatorItem());

                foreach (var weather in WeatherTypes)
                {
                    var weatherItem = new NativeItem(FormatWeatherName(weather), $"Set weather to {FormatWeatherName(weather)}.");
                    weatherItem.Activated += (sender, e) =>
                    {
                        TriggerServerEvent("cbps:setWeather", weather);
                        Notify.Success($"Weather set to {FormatWeatherName(weather)}.");
                    };
                    menu.Add(weatherItem);
                }
            }
        }

        private string FormatWeatherName(string weather)
        {
            var formatted = weather.ToLower();
            return char.ToUpper(formatted[0]) + formatted.Substring(1);
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
