using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using LemonUI.Menus;

using CBPSMenu.Shared;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// World Options Menu - Similar to vMenu's TimeOptions.cs and WeatherOptions.cs
    /// </summary>
    public class WorldMenu
    {
        #region Variables

        public NativeMenu Menu { get; private set; }

        // Checkbox items
        private NativeCheckboxItem _freezeTimeItem;
        private NativeCheckboxItem _blackoutItem;

        // Weather types
        private static readonly string[] WeatherTypes = 
        {
            "EXTRASUNNY", "CLEAR", "CLOUDS", "SMOG", "FOGGY", "OVERCAST",
            "RAIN", "THUNDER", "CLEARING", "NEUTRAL", "SNOW", "BLIZZARD",
            "SNOWLIGHT", "XMAS", "HALLOWEEN"
        };

        #endregion

        #region Constructor

        public WorldMenu()
        {
            CreateMenu();
        }

        #endregion

        #region Menu Creation

        private void CreateMenu()
        {
            Menu = ThemeManager.CreateThemedMenu("comboom.sucht", "World Options");

            // Time Options Section
            var timeHeader = new NativeItem("~b~=== Time Options ===", "Control world time");
            timeHeader.Enabled = false;
            Menu.Add(timeHeader);

            // Set Hour
            var hourItem = new NativeListItem<int>("Set Hour", "Set the current hour",
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23);
            hourItem.ItemChanged += (sender, args) =>
            {
                API.NetworkOverrideClockTime(hourItem.SelectedItem, 0, 0);
                Main.ShowNotification($"~b~Time set to: {hourItem.SelectedItem}:00");
            };
            Menu.Add(hourItem);

            // Quick Time Options
            var morningItem = new NativeItem("Morning (6:00)", "Set time to morning");
            morningItem.Activated += (sender, args) =>
            {
                API.NetworkOverrideClockTime(6, 0, 0);
                Main.ShowNotification("~b~Time set to morning (6:00)");
            };
            Menu.Add(morningItem);

            var noonItem = new NativeItem("Noon (12:00)", "Set time to noon");
            noonItem.Activated += (sender, args) =>
            {
                API.NetworkOverrideClockTime(12, 0, 0);
                Main.ShowNotification("~b~Time set to noon (12:00)");
            };
            Menu.Add(noonItem);

            var eveningItem = new NativeItem("Evening (18:00)", "Set time to evening");
            eveningItem.Activated += (sender, args) =>
            {
                API.NetworkOverrideClockTime(18, 0, 0);
                Main.ShowNotification("~b~Time set to evening (18:00)");
            };
            Menu.Add(eveningItem);

            var nightItem = new NativeItem("Night (23:00)", "Set time to night");
            nightItem.Activated += (sender, args) =>
            {
                API.NetworkOverrideClockTime(23, 0, 0);
                Main.ShowNotification("~b~Time set to night (23:00)");
            };
            Menu.Add(nightItem);

            // Freeze Time
            _freezeTimeItem = new NativeCheckboxItem("Freeze Time", "Freeze the current time", false);
            _freezeTimeItem.CheckboxChanged += (sender, args) =>
            {
                if (_freezeTimeItem.Checked)
                {
                    API.PauseClock(true);
                    Main.ShowNotification("~g~Time frozen!");
                }
                else
                {
                    API.PauseClock(false);
                    Main.ShowNotification("~r~Time unfrozen!");
                }
            };
            Menu.Add(_freezeTimeItem);

            // Add separator
            Menu.Add(new NativeSeparatorItem());

            // Weather Options Section
            var weatherHeader = new NativeItem("~b~=== Weather Options ===", "Control world weather");
            weatherHeader.Enabled = false;
            Menu.Add(weatherHeader);

            // Weather List
            var weatherItem = new NativeListItem<string>("Weather", "Change the weather",
                WeatherTypes);
            weatherItem.ItemChanged += (sender, args) =>
            {
                var weather = weatherItem.SelectedItem;
                API.SetWeatherTypeNowPersist(weather);
                Main.ShowNotification($"~b~Weather changed to: {weather}");
            };
            Menu.Add(weatherItem);

            // Quick Weather Options
            var sunnyItem = new NativeItem("Sunny", "Set weather to clear and sunny");
            sunnyItem.Activated += (sender, args) =>
            {
                API.SetWeatherTypeNowPersist("EXTRASUNNY");
                Main.ShowNotification("~b~Weather set to sunny");
            };
            Menu.Add(sunnyItem);

            var rainItem = new NativeItem("Rain", "Set weather to rainy");
            rainItem.Activated += (sender, args) =>
            {
                API.SetWeatherTypeNowPersist("RAIN");
                Main.ShowNotification("~b~Weather set to rain");
            };
            Menu.Add(rainItem);

            var thunderItem = new NativeItem("Thunder", "Set weather to thunderstorm");
            thunderItem.Activated += (sender, args) =>
            {
                API.SetWeatherTypeNowPersist("THUNDER");
                Main.ShowNotification("~b~Weather set to thunderstorm");
            };
            Menu.Add(thunderItem);

            var snowItem = new NativeItem("Snow", "Set weather to snow");
            snowItem.Activated += (sender, args) =>
            {
                API.SetWeatherTypeNowPersist("SNOW");
                Main.ShowNotification("~b~Weather set to snow");
            };
            Menu.Add(snowItem);

            var xmasItem = new NativeItem("Christmas", "Set weather to Christmas snow");
            xmasItem.Activated += (sender, args) =>
            {
                API.SetWeatherTypeNowPersist("XMAS");
                Main.ShowNotification("~b~Weather set to Christmas");
            };
            Menu.Add(xmasItem);

            // Blackout Mode
            _blackoutItem = new NativeCheckboxItem("Blackout Mode", "Turn off all lights in the city", false);
            _blackoutItem.CheckboxChanged += (sender, args) =>
            {
                API.SetArtificialLightsState(_blackoutItem.Checked);
                Main.ShowNotification(_blackoutItem.Checked ? "~g~Blackout: ON" : "~r~Blackout: OFF");
            };
            Menu.Add(_blackoutItem);

            // Add separator
            Menu.Add(new NativeSeparatorItem());

            // Clear Area
            var clearAreaItem = new NativeItem("Clear Area", "Remove all entities in your area");
            clearAreaItem.Activated += (sender, args) =>
            {
                var pos = Game.PlayerPed.Position;
                API.ClearAreaOfEverything(pos.X, pos.Y, pos.Z, 100f, false, false, false, false);
                Main.ShowNotification("~g~Area cleared!");
            };
            Menu.Add(clearAreaItem);
        }

        #endregion
    }
}
