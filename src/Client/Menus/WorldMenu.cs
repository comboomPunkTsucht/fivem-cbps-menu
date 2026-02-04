using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using LemonUI.Menus;

using CBPSMenu.Shared;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// World Options Menu - Enhanced with vMenu features
    /// Based on vMenu's TimeOptions.cs and WeatherOptions.cs
    /// </summary>
    public class WorldMenu
    {
        #region Variables

        public NativeMenu Menu { get; private set; }

        // Checkbox items
        private NativeCheckboxItem _freezeTimeItem;
        private NativeCheckboxItem _blackoutItem;
        private NativeCheckboxItem _vehicleBlackoutItem;
        private NativeCheckboxItem _snowEffectsItem;
        private NativeCheckboxItem _dynamicWeatherItem;

        // Weather types
        private static readonly string[] WeatherTypes =
        {
            "EXTRASUNNY", "CLEAR", "NEUTRAL", "SMOG", "FOGGY", "CLOUDS", "OVERCAST",
            "CLEARING", "RAIN", "THUNDER", "BLIZZARD", "SNOW", "SNOWLIGHT", "XMAS", "HALLOWEEN"
        };

        private static readonly string[] WeatherNames =
        {
            "Extra Sunny", "Clear", "Neutral", "Smog", "Foggy", "Cloudy", "Overcast",
            "Clearing", "Rainy", "Thunder", "Blizzard", "Snow", "Light Snow", "X-MAS", "Halloween"
        };

        // Currently stored time values
        private int _storedHour = 12;
        private int _storedMinute = 0;

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

            // === TIME OPTIONS SECTION ===
            var timeHeader = new NativeItem("~b~=== Time Options ===", "Control world time")
            {
                Enabled = false
            };
            Menu.Add(timeHeader);

            // Quick Time Presets
            var timePresets = new Dictionary<string, int>
            {
                { "Early Morning (06:00)", 6 },
                { "Morning (09:00)", 9 },
                { "Noon (12:00)", 12 },
                { "Early Afternoon (15:00)", 15 },
                { "Afternoon (18:00)", 18 },
                { "Evening (21:00)", 21 },
                { "Midnight (00:00)", 0 },
                { "Night (03:00)", 3 }
            };

            foreach (var preset in timePresets)
            {
                var presetItem = new NativeItem(preset.Key, $"Set time to {preset.Value:00}:00");
                int hour = preset.Value;
                presetItem.Activated += (sender, args) =>
                {
                    API.NetworkOverrideClockTime(hour, 0, 0);
                    _storedHour = hour;
                    _storedMinute = 0;
                    Main.ShowNotification($"~b~Time set to {hour:00}:00");
                };
                Menu.Add(presetItem);
            }

            Menu.Add(new NativeSeparatorItem());

            // Custom Hour
            var hours = new List<string>();
            for (int i = 0; i < 24; i++)
            {
                hours.Add(i.ToString("00"));
            }
            var hourItem = new NativeListItem<string>("Set Hour", "Set custom hour", hours.ToArray());
            hourItem.ItemChanged += (sender, args) =>
            {
                _storedHour = hourItem.SelectedIndex;
                API.NetworkOverrideClockTime(_storedHour, _storedMinute, 0);
                Main.ShowNotification($"~b~Time: {_storedHour:00}:{_storedMinute:00}");
            };
            Menu.Add(hourItem);

            // Custom Minute
            var minutes = new List<string>();
            for (int i = 0; i < 60; i++)
            {
                minutes.Add(i.ToString("00"));
            }
            var minuteItem = new NativeListItem<string>("Set Minute", "Set custom minute", minutes.ToArray());
            minuteItem.ItemChanged += (sender, args) =>
            {
                _storedMinute = minuteItem.SelectedIndex;
                API.NetworkOverrideClockTime(_storedHour, _storedMinute, 0);
                Main.ShowNotification($"~b~Time: {_storedHour:00}:{_storedMinute:00}");
            };
            Menu.Add(minuteItem);

            // Freeze Time
            _freezeTimeItem = new NativeCheckboxItem("Freeze Time", "Freeze the current time", false);
            _freezeTimeItem.CheckboxChanged += (sender, args) =>
            {
                API.PauseClock(_freezeTimeItem.Checked);
                Main.ShowNotification(_freezeTimeItem.Checked ? "~g~Time frozen!" : "~r~Time unfrozen!");
            };
            Menu.Add(_freezeTimeItem);

            // Sync time with server
            var syncTimeItem = new NativeItem("Sync Time with Server", "Reset to server time");
            syncTimeItem.Activated += (sender, args) =>
            {
                API.NetworkClearClockTimeOverride();
                _freezeTimeItem.Checked = false;
                Main.ShowNotification("~g~Time synced with server!");
            };
            Menu.Add(syncTimeItem);

            Menu.Add(new NativeSeparatorItem());

            // === WEATHER OPTIONS SECTION ===
            var weatherHeader = new NativeItem("~b~=== Weather Options ===", "Control world weather")
            {
                Enabled = false
            };
            Menu.Add(weatherHeader);

            // Weather List
            var weatherItem = new NativeListItem<string>("Weather", "Change the weather", WeatherNames);
            weatherItem.Activated += (sender, args) =>
            {
                var weather = WeatherTypes[weatherItem.SelectedIndex];
                API.SetWeatherTypeNowPersist(weather);
                Main.ShowNotification($"~b~Weather changed to: {WeatherNames[weatherItem.SelectedIndex]}");
            };
            Menu.Add(weatherItem);

            Menu.Add(new NativeSeparatorItem());

            // Quick Weather Buttons
            var quickWeatherHeader = new NativeItem("~y~Quick Weather ▼", "Fast weather presets")
            {
                Enabled = false
            };
            Menu.Add(quickWeatherHeader);

            var commonWeathers = new string[] { "EXTRASUNNY", "CLEAR", "RAIN", "THUNDER", "SNOW", "XMAS" };
            var commonWeatherNames = new string[] { "☀ Sunny", "☁ Clear", "🌧 Rain", "⚡ Thunder", "❄ Snow", "🎄 Christmas" };

            for (int i = 0; i < commonWeathers.Length; i++)
            {
                string weather = commonWeathers[i];
                string name = commonWeatherNames[i];
                var quickWeatherItem = new NativeItem(name, $"Set weather to {name}");
                quickWeatherItem.Activated += (sender, args) =>
                {
                    API.SetWeatherTypeNowPersist(weather);
                    Main.ShowNotification($"~b~Weather set to {name}");
                };
                Menu.Add(quickWeatherItem);
            }

            Menu.Add(new NativeSeparatorItem());

            // === WORLD EFFECTS SECTION ===
            var effectsHeader = new NativeItem("~b~=== World Effects ===", "Toggle world effects")
            {
                Enabled = false
            };
            Menu.Add(effectsHeader);

            // Dynamic Weather (informational)
            _dynamicWeatherItem = new NativeCheckboxItem("Dynamic Weather", "Enable dynamic weather changes (server-side)", false);
            _dynamicWeatherItem.CheckboxChanged += (sender, args) =>
            {
                // This would need server-side implementation
                Main.ShowNotification(_dynamicWeatherItem.Checked
                    ? "~g~Dynamic weather enabled (if supported)"
                    : "~r~Dynamic weather disabled");
            };
            Menu.Add(_dynamicWeatherItem);

            // Blackout Mode
            _blackoutItem = new NativeCheckboxItem("Blackout Mode", "Turn off all lights in the city", false);
            _blackoutItem.CheckboxChanged += (sender, args) =>
            {
                API.SetArtificialLightsState(_blackoutItem.Checked);
                Main.ShowNotification(_blackoutItem.Checked ? "~g~Blackout: ON" : "~r~Blackout: OFF");
            };
            Menu.Add(_blackoutItem);

            // Vehicle Blackout
            _vehicleBlackoutItem = new NativeCheckboxItem("Vehicle Lights Blackout", "Disable all vehicle lights", false);
            _vehicleBlackoutItem.CheckboxChanged += (sender, args) =>
            {
                API.SetArtificialLightsStateAffectsVehicles(_vehicleBlackoutItem.Checked);
                Main.ShowNotification(_vehicleBlackoutItem.Checked
                    ? "~g~Vehicle Lights Blackout: ON"
                    : "~r~Vehicle Lights Blackout: OFF");
            };
            Menu.Add(_vehicleBlackoutItem);

            // Snow Effects
            _snowEffectsItem = new NativeCheckboxItem("Force Snow Effects", "Force snow on ground and particles", false);
            _snowEffectsItem.CheckboxChanged += (sender, args) =>
            {
                API.SetForceVehicleTrails(_snowEffectsItem.Checked);
                API.SetForcePedFootstepsTracks(_snowEffectsItem.Checked);
                Main.ShowNotification(_snowEffectsItem.Checked
                    ? "~g~Snow Effects: ON"
                    : "~r~Snow Effects: OFF");
            };
            Menu.Add(_snowEffectsItem);

            Menu.Add(new NativeSeparatorItem());

            // === CLOUDS SECTION ===
            var cloudsHeader = new NativeItem("~b~=== Cloud Options ===", "Modify clouds")
            {
                Enabled = false
            };
            Menu.Add(cloudsHeader);

            // Remove Clouds
            var removeCloudsItem = new NativeItem("Remove All Clouds", "Clear the sky of clouds");
            removeCloudsItem.Activated += (sender, args) =>
            {
                API.ClearCloudHat();
                Main.ShowNotification("~g~Clouds removed!");
            };
            Menu.Add(removeCloudsItem);

            // Randomize Clouds
            var randomizeCloudsItem = new NativeItem("Randomize Clouds", "Add random clouds to the sky");
            randomizeCloudsItem.Activated += (sender, args) =>
            {
                var cloudTypes = new string[]
                {
                    "Altostratus", "Clear01", "Cloudy01", "Contrails", "Horizon",
                    "horizonband1", "horizonband2", "horizonband3", "Nimbus",
                    "Puffs", "Rain", "Shower", "Stormy01", "Stratoscumulus",
                    "Stripey", "wispy"
                };
                Random random = new Random();
                string cloudType = cloudTypes[random.Next(cloudTypes.Length)];
                float opacity = (float)random.NextDouble();

                API.LoadCloudHat(cloudType, opacity);
                Main.ShowNotification($"~b~Clouds randomized: {cloudType}");
            };
            Menu.Add(randomizeCloudsItem);

            // Cloud Type List
            var cloudTypesList = new string[]
            {
                "Clear", "Altostratus", "Cloudy", "Contrails", "Horizon",
                "Nimbus", "Puffs", "Rain", "Stormy", "Wispy"
            };
            var cloudTypesInternal = new string[]
            {
                "Clear01", "Altostratus", "Cloudy01", "Contrails", "Horizon",
                "Nimbus", "Puffs", "Rain", "Stormy01", "wispy"
            };
            var cloudTypeItem = new NativeListItem<string>("Cloud Type", "Select cloud type", cloudTypesList);
            cloudTypeItem.Activated += (sender, args) =>
            {
                string cloudType = cloudTypesInternal[cloudTypeItem.SelectedIndex];
                API.LoadCloudHat(cloudType, 1.0f);
                Main.ShowNotification($"~b~Clouds set to: {cloudTypesList[cloudTypeItem.SelectedIndex]}");
            };
            Menu.Add(cloudTypeItem);

            Menu.Add(new NativeSeparatorItem());

            // === AREA MANAGEMENT ===
            var areaHeader = new NativeItem("~b~=== Area Management ===", "Manage nearby entities")
            {
                Enabled = false
            };
            Menu.Add(areaHeader);

            // Clear Area
            var clearAreaItem = new NativeItem("Clear Area", "Remove all entities in your area");
            clearAreaItem.Activated += (sender, args) =>
            {
                var pos = Game.PlayerPed.Position;
                API.ClearAreaOfEverything(pos.X, pos.Y, pos.Z, 100f, false, false, false, false);
                Main.ShowNotification("~g~Area cleared!");
            };
            Menu.Add(clearAreaItem);

            // Clear Vehicles
            var clearVehiclesItem = new NativeItem("Clear Nearby Vehicles", "Remove vehicles in your area");
            clearVehiclesItem.Activated += (sender, args) =>
            {
                var pos = Game.PlayerPed.Position;
                API.ClearAreaOfVehicles(pos.X, pos.Y, pos.Z, 100f, false, false, false, false, false);
                Main.ShowNotification("~g~Nearby vehicles cleared!");
            };
            Menu.Add(clearVehiclesItem);

            // Clear Peds
            var clearPedsItem = new NativeItem("Clear Nearby Peds", "Remove NPCs in your area");
            clearPedsItem.Activated += (sender, args) =>
            {
                var pos = Game.PlayerPed.Position;
                API.ClearAreaOfPeds(pos.X, pos.Y, pos.Z, 100f, 0);
                Main.ShowNotification("~g~Nearby peds cleared!");
            };
            Menu.Add(clearPedsItem);

            // Clear Objects
            var clearObjectsItem = new NativeItem("Clear Nearby Objects", "Remove objects in your area");
            clearObjectsItem.Activated += (sender, args) =>
            {
                var pos = Game.PlayerPed.Position;
                API.ClearAreaOfObjects(pos.X, pos.Y, pos.Z, 100f, 0);
                Main.ShowNotification("~g~Nearby objects cleared!");
            };
            Menu.Add(clearObjectsItem);

            // Clear Cops
            var clearCopsItem = new NativeItem("Clear Nearby Cops", "Remove police in your area");
            clearCopsItem.Activated += (sender, args) =>
            {
                var pos = Game.PlayerPed.Position;
                API.ClearAreaOfCops(pos.X, pos.Y, pos.Z, 100f, 0);
                Main.ShowNotification("~g~Nearby cops cleared!");
            };
            Menu.Add(clearCopsItem);

            Menu.Add(new NativeSeparatorItem());

            // === WORLD DENSITY ===
            var densityHeader = new NativeItem("~b~=== World Density ===", "Control NPC and traffic density")
            {
                Enabled = false
            };
            Menu.Add(densityHeader);

            // Ped Density
            var pedDensityValues = new string[] { "0%", "25%", "50%", "75%", "100%" };
            var pedDensityItem = new NativeListItem<string>("Ped Density", "Control NPC population", pedDensityValues);
            pedDensityItem.ItemChanged += (sender, args) =>
            {
                float[] densities = { 0f, 0.25f, 0.5f, 0.75f, 1f };
                float density = densities[pedDensityItem.SelectedIndex];
                API.SetPedDensityMultiplierThisFrame(density);
                API.SetScenarioPedDensityMultiplierThisFrame(density, density);
                Main.ShowNotification($"~b~Ped density: {pedDensityValues[pedDensityItem.SelectedIndex]}");
            };
            Menu.Add(pedDensityItem);

            // Vehicle Density
            var vehicleDensityItem = new NativeListItem<string>("Vehicle Density", "Control traffic amount", pedDensityValues);
            vehicleDensityItem.ItemChanged += (sender, args) =>
            {
                float[] densities = { 0f, 0.25f, 0.5f, 0.75f, 1f };
                float density = densities[vehicleDensityItem.SelectedIndex];
                API.SetVehicleDensityMultiplierThisFrame(density);
                API.SetRandomVehicleDensityMultiplierThisFrame(density);
                API.SetParkedVehicleDensityMultiplierThisFrame(density);
                Main.ShowNotification($"~b~Vehicle density: {pedDensityValues[vehicleDensityItem.SelectedIndex]}");
            };
            Menu.Add(vehicleDensityItem);
        }

        #endregion
    }
}
