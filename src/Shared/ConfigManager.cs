using System;
using System.Collections.Generic;
using System.Linq;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using Newtonsoft.Json;

namespace CBPSMenu.Shared
{
    /// <summary>
    /// Manages configuration settings for the menu system
    /// Loads from config.json with fallback to default values
    /// </summary>
    public static class ConfigManager
    {
        private const string ConfigFileName = "config.json";
        public static MenuConfig Config { get; private set; }

        static ConfigManager()
        {
            // Initialize with default values immediately
            Config = new MenuConfig();
            LoadConfig();
        }

        public static void LoadConfig()
        {
            try
            {
                string json = API.LoadResourceFile(API.GetCurrentResourceName(), ConfigFileName);
                if (string.IsNullOrEmpty(json))
                {
                    Debug.WriteLine($"[CBPS Menu] {ConfigFileName} not found, using defaults.");
                    return;
                }

                var loadedConfig = JsonConvert.DeserializeObject<MenuConfig>(json);
                if (loadedConfig != null)
                {
                    Config = loadedConfig;
                    Debug.WriteLine($"[CBPS Menu] Configuration loaded successfully.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CBPS Menu] Error loading configuration: {ex.Message}");
            }
        }

        public static void Reload()
        {
            LoadConfig();
        }

        #region Accessors

        public static Dictionary<string, string[]> VehicleCategories => Config.VehicleCategories?.ToDictionary(c => c.Name, c => c.Vehicles) ?? new Dictionary<string, string[]>();

        public static Dictionary<string, string[]> WeaponCategories => Config.WeaponCategories?.ToDictionary(c => c.Name, c => c.Weapons) ?? new Dictionary<string, string[]>();

        public static float[] VoiceRanges => Config.Voice?.Ranges ?? new float[] { 3.0f, 5.0f, 10.0f };

        public static string[] VoiceRangeLabels
        {
            get
            {
                var ranges = VoiceRanges;
                var labels = new string[ranges.Length];
                for (int i = 0; i < ranges.Length; i++)
                {
                    labels[i] = $"{ranges[i]}m";
                }
                return labels;
            }
        }

        // Backward compatibility wrappers for existing code
        public static string GetSettingsString(string key, string defaultValue = "")
        {
            // Simple mapping for common keys, otherwise defaults
            switch (key)
            {
                case "cbps_menu_key": return Config.MenuSettings?.MenuKey ?? defaultValue;
                case "cbps_noclip_key": return Config.Keybinds?["Noclip"]?.Key ?? defaultValue;
                case "cbps_theme": return Config.ThemeSettings?.DefaultTheme ?? defaultValue;
                default: return API.GetResourceKvpString(key) ?? defaultValue;
            }
        }

        public static bool GetSettingsBool(string key, bool defaultValue = false)
        {
            switch (key)
            {
                case "cbps_voice_enabled": return Config.Voice?.Enabled ?? defaultValue;
                case "cbps_race_enabled": return Config.Race?.Enabled ?? defaultValue;
                default:
                    var val = API.GetResourceKvpString(key);
                    return string.IsNullOrEmpty(val) ? defaultValue : (val == "true" || val == "1");
            }
        }

        public static int GetSettingsInt(string key, int defaultValue = 0)
        {
            switch (key)
            {
                case "cbps_race_max_checkpoints": return Config.Race?.MaxCheckpoints ?? defaultValue;
                default:
                    var val = API.GetResourceKvpString(key);
                    return int.TryParse(val, out int res) ? res : defaultValue;
            }
        }

        public static float GetSettingsFloat(string key, float defaultValue = 0f)
        {
            switch (key)
            {
                case "cbps_voice_default_range": return (float)(Config.Voice?.DefaultRange ?? defaultValue);
                case "cbps_race_checkpoint_radius": return (float)(Config.Race?.CheckpointRadius ?? defaultValue);
                default:
                    var val = API.GetResourceKvpString(key);
                    return float.TryParse(val, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float res) ? res : defaultValue;
            }
        }

        #endregion
    }

    #region Configuration Classes

    public class MenuConfig
    {
        public MenuSettingsConfig MenuSettings { get; set; } = new MenuSettingsConfig();
        public ThemeSettingsConfig ThemeSettings { get; set; } = new ThemeSettingsConfig();
        public VoiceConfig Voice { get; set; } = new VoiceConfig();
        public RaceConfig Race { get; set; } = new RaceConfig();
        public List<CategoryConfig> VehicleCategories { get; set; } = new List<CategoryConfig>();
        public List<CategoryConfig> WeaponCategories { get; set; } = new List<CategoryConfig>();
        public Dictionary<string, KeybindConfig> Keybinds { get; set; } = new Dictionary<string, KeybindConfig>();
    }

    public class MenuSettingsConfig
    {
        public string MenuKey { get; set; } = "F1";
        public string MenuTitle { get; set; } = "CBPS Menu";
    }

    public class ThemeSettingsConfig
    {
        public string DefaultTheme { get; set; } = "nord";
    }

    public class VoiceConfig
    {
        public bool Enabled { get; set; } = true;
        public double DefaultRange { get; set; } = 5.0;
        public float[] Ranges { get; set; } = { 3.0f, 8.0f, 15.0f, 32.0f };
    }

    public class RaceConfig
    {
        public bool Enabled { get; set; } = true;
        public int MaxCheckpoints { get; set; } = 20;
        public double CheckpointRadius { get; set; } = 10.0;
    }

    public class CategoryConfig
    {
        public string Name { get; set; }
        public string[] Vehicles { get; set; }
        public string[] Weapons { get; set; }
    }

    public class KeybindConfig
    {
        public string Key { get; set; }
        public string Controller { get; set; }
        public string Description { get; set; }
    }

    #endregion
}
