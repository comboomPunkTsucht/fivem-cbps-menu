using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace CBPSMenu.Shared
{
    /// <summary>
    /// Manages configuration settings for the menu system
    /// Similar to vMenu's ConfigManager
    /// </summary>
    public static class ConfigManager
    {
        #region Configuration Keys

        // Menu Settings
        public const string CFG_MENU_KEY = "cbps_menu_key";
        public const string CFG_NOCLIP_KEY = "cbps_noclip_key";
        public const string CFG_RESET_KEY = "cbps_reset_key";

        // Voice Settings
        public const string CFG_VOICE_ENABLED = "cbps_voice_enabled";
        public const string CFG_VOICE_DEFAULT_RANGE = "cbps_voice_default_range";

        // Race Settings
        public const string CFG_RACE_ENABLED = "cbps_race_enabled";
        public const string CFG_RACE_MAX_CHECKPOINTS = "cbps_race_max_checkpoints";
        public const string CFG_RACE_CHECKPOINT_RADIUS = "cbps_race_checkpoint_radius";

        // Theme Settings
        public const string CFG_THEME = "cbps_theme";

        #endregion

        #region Default Values

        public static readonly Dictionary<string, object> DefaultSettings = new Dictionary<string, object>
        {
            { CFG_MENU_KEY, "F1" },
            { CFG_NOCLIP_KEY, "F2" },
            { CFG_RESET_KEY, "F9" },
            { CFG_VOICE_ENABLED, true },
            { CFG_VOICE_DEFAULT_RANGE, 5.0f },
            { CFG_RACE_ENABLED, true },
            { CFG_RACE_MAX_CHECKPOINTS, 20 },
            { CFG_RACE_CHECKPOINT_RADIUS, 10.0f },
            { CFG_THEME, "nord" }
        };

        #endregion

        #region Configuration Getters

        /// <summary>
        /// Get a string setting from KVP storage
        /// </summary>
        public static string GetSettingsString(string key, string defaultValue = "")
        {
            var value = API.GetResourceKvpString(key);
            if (string.IsNullOrEmpty(value))
            {
                if (DefaultSettings.ContainsKey(key))
                {
                    return DefaultSettings[key].ToString();
                }
                return defaultValue;
            }
            return value;
        }

        /// <summary>
        /// Get a boolean setting from KVP storage
        /// </summary>
        public static bool GetSettingsBool(string key, bool defaultValue = false)
        {
            var value = API.GetResourceKvpString(key);
            if (string.IsNullOrEmpty(value))
            {
                if (DefaultSettings.ContainsKey(key))
                {
                    return (bool)DefaultSettings[key];
                }
                return defaultValue;
            }
            return value.ToLower() == "true" || value == "1";
        }

        /// <summary>
        /// Get an integer setting from KVP storage
        /// Note: Uses string-based storage to properly detect missing vs zero values
        /// </summary>
        public static int GetSettingsInt(string key, int defaultValue = 0)
        {
            // Use string storage to distinguish between "not set" and "set to 0"
            var strValue = API.GetResourceKvpString(key);
            if (!string.IsNullOrEmpty(strValue) && int.TryParse(strValue, out int result))
            {
                return result;
            }
            
            if (DefaultSettings.ContainsKey(key))
            {
                return Convert.ToInt32(DefaultSettings[key]);
            }
            return defaultValue;
        }

        /// <summary>
        /// Get a float setting from KVP storage
        /// Note: Uses string-based storage to properly detect missing vs zero values
        /// </summary>
        public static float GetSettingsFloat(string key, float defaultValue = 0f)
        {
            // Use string storage to distinguish between "not set" and "set to 0.0"
            var strValue = API.GetResourceKvpString(key);
            if (!string.IsNullOrEmpty(strValue) && float.TryParse(strValue, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float result))
            {
                return result;
            }
            
            if (DefaultSettings.ContainsKey(key))
            {
                return Convert.ToSingle(DefaultSettings[key]);
            }
            return defaultValue;
        }

        #endregion

        #region Configuration Setters

        /// <summary>
        /// Set a string setting to KVP storage
        /// </summary>
        public static void SetSettingsString(string key, string value)
        {
            API.SetResourceKvp(key, value);
        }

        /// <summary>
        /// Set a boolean setting to KVP storage
        /// </summary>
        public static void SetSettingsBool(string key, bool value)
        {
            API.SetResourceKvp(key, value ? "true" : "false");
        }

        /// <summary>
        /// Set an integer setting to KVP storage
        /// Note: Uses string storage for consistency with GetSettingsInt
        /// </summary>
        public static void SetSettingsInt(string key, int value)
        {
            API.SetResourceKvp(key, value.ToString());
        }

        /// <summary>
        /// Set a float setting to KVP storage
        /// Note: Uses string storage for consistency with GetSettingsFloat
        /// </summary>
        public static void SetSettingsFloat(string key, float value)
        {
            API.SetResourceKvp(key, value.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        #endregion

        #region Vehicle Categories

        /// <summary>
        /// Vehicle categories for the spawner menu
        /// </summary>
        public static readonly Dictionary<string, string[]> VehicleCategories = new Dictionary<string, string[]>
        {
            { "Super", new[] { "adder", "autarch", "banshee2", "bullet", "cheetah", "cyclone", "entityxf", "fmj", "gp1", "infernus", "nero", "osiris", "penetrator", "reaper", "t20", "taipan", "tempesta", "turismor", "tyrus", "vacca", "visione", "voltic", "xa21", "zentorno" } },
            { "Sports", new[] { "alpha", "banshee", "bestiagts", "blista2", "buffalo", "buffalo2", "buffalo3", "carbonizzare", "comet2", "comet3", "coquette", "elegy", "elegy2", "feltzer2", "furoregt", "fusilade", "futo", "jester", "jester2", "khamelion", "kuruma", "lynx", "massacro", "neon", "ninef", "ninef2", "pariah", "penumbra", "raiden", "rapidgt", "rapidgt2", "revolter", "ruston", "schafter3", "schafter4", "schafter5", "schwarzer", "sentinel3", "seven70", "specter", "specter2", "streiter", "sultan", "surano", "tampa2", "tropos", "verlierer2" } },
            { "SUVs", new[] { "baller", "baller2", "baller3", "baller4", "baller5", "baller6", "bjxl", "cavalcade", "cavalcade2", "contender", "dubsta", "dubsta2", "fq2", "granger", "gresley", "habanero", "huntley", "landstalker", "mesa", "patriot", "radi", "rocoto", "seminole", "serrano", "xls", "xls2" } },
            { "Sedans", new[] { "asea", "asterope", "cog55", "cog552", "cognoscenti", "cognoscenti2", "emperor", "emperor2", "emperor3", "fugitive", "glendale", "ingot", "intruder", "limo2", "premier", "primo", "primo2", "regina", "schafter2", "stanier", "stratum", "stretch", "surge", "tailgater", "warrener", "washington" } },
            { "Motorcycles", new[] { "akuma", "avarus", "bagger", "bati", "bati2", "bf400", "carbonrs", "chimera", "cliffhanger", "daemon", "daemon2", "defiler", "diablous", "diablous2", "double", "enduro", "esskey", "faggio", "faggio2", "faggio3", "gargoyle", "hakuchou", "hakuchou2", "hexer", "innovation", "lectro", "manchez", "nemesis", "nightblade", "pcj", "ratbike", "ruffian", "sanchez", "sanchez2", "sanctus", "shotaro", "sovereign", "thrust", "vader", "vindicator", "vortex", "wolfsbane", "zombiea", "zombieb" } },
            { "Emergency", new[] { "ambulance", "fbi", "fbi2", "firetruk", "lguard", "pbus", "police", "police2", "police3", "police4", "policeb", "policeold1", "policeold2", "policet", "pranger", "predator", "riot", "sheriff", "sheriff2" } },
            { "Military", new[] { "apc", "barracks", "barracks2", "barracks3", "crusader", "halftrack", "khanjali", "rhino", "scarab", "scarab2", "scarab3", "trailersmall2" } },
            { "Helicopters", new[] { "akula", "annihilator", "buzzard", "buzzard2", "cargobob", "cargobob2", "cargobob3", "cargobob4", "frogger", "frogger2", "havok", "hunter", "maverick", "polmav", "savage", "seasparrow", "skylift", "supervolito", "supervolito2", "swift", "swift2", "valkyrie", "valkyrie2", "volatus" } },
            { "Planes", new[] { "alphaz1", "avenger", "avenger2", "besra", "bombushka", "cargoplane", "cuban800", "dodo", "duster", "howard", "hydra", "jet", "lazer", "luxor", "luxor2", "mammatus", "miljet", "mogul", "molotok", "nimbus", "nokota", "pyro", "rogue", "seabreeze", "shamal", "starling", "strikeforce", "stunt", "titan", "tula", "velum", "velum2", "vestra", "volatol" } },
            { "Boats", new[] { "dinghy", "dinghy2", "dinghy3", "dinghy4", "jetmax", "marquis", "seashark", "seashark2", "seashark3", "speeder", "speeder2", "squalo", "submersible", "submersible2", "suntrap", "toro", "toro2", "tropic", "tropic2", "tug" } }
        };

        #endregion

        #region Weapon Categories

        /// <summary>
        /// Weapon categories for the weapon menu
        /// </summary>
        public static readonly Dictionary<string, string[]> WeaponCategories = new Dictionary<string, string[]>
        {
            { "Melee", new[] { "WEAPON_KNIFE", "WEAPON_NIGHTSTICK", "WEAPON_HAMMER", "WEAPON_BAT", "WEAPON_GOLFCLUB", "WEAPON_CROWBAR", "WEAPON_BOTTLE", "WEAPON_DAGGER", "WEAPON_HATCHET", "WEAPON_KNUCKLE", "WEAPON_MACHETE", "WEAPON_FLASHLIGHT", "WEAPON_SWITCHBLADE", "WEAPON_POOLCUE", "WEAPON_WRENCH" } },
            { "Handguns", new[] { "WEAPON_PISTOL", "WEAPON_COMBATPISTOL", "WEAPON_APPISTOL", "WEAPON_PISTOL50", "WEAPON_SNSPISTOL", "WEAPON_HEAVYPISTOL", "WEAPON_VINTAGEPISTOL", "WEAPON_MARKSMANPISTOL", "WEAPON_REVOLVER", "WEAPON_DOUBLEACTION" } },
            { "SMGs", new[] { "WEAPON_MICROSMG", "WEAPON_SMG", "WEAPON_ASSAULTSMG", "WEAPON_COMBATPDW", "WEAPON_MACHINEPISTOL", "WEAPON_MINISMG", "WEAPON_GUSENBERG" } },
            { "Shotguns", new[] { "WEAPON_PUMPSHOTGUN", "WEAPON_SAWNOFFSHOTGUN", "WEAPON_ASSAULTSHOTGUN", "WEAPON_BULLPUPSHOTGUN", "WEAPON_MUSKET", "WEAPON_HEAVYSHOTGUN", "WEAPON_DBSHOTGUN", "WEAPON_AUTOSHOTGUN" } },
            { "Assault Rifles", new[] { "WEAPON_ASSAULTRIFLE", "WEAPON_CARBINERIFLE", "WEAPON_ADVANCEDRIFLE", "WEAPON_SPECIALCARBINE", "WEAPON_BULLPUPRIFLE", "WEAPON_COMPACTRIFLE" } },
            { "Sniper Rifles", new[] { "WEAPON_SNIPERRIFLE", "WEAPON_HEAVYSNIPER", "WEAPON_MARKSMANRIFLE" } },
            { "Heavy Weapons", new[] { "WEAPON_RPG", "WEAPON_GRENADELAUNCHER", "WEAPON_MINIGUN", "WEAPON_FIREWORK", "WEAPON_RAILGUN", "WEAPON_HOMINGLAUNCHER", "WEAPON_COMPACTLAUNCHER" } },
            { "Throwables", new[] { "WEAPON_GRENADE", "WEAPON_BZGAS", "WEAPON_SMOKEGRENADE", "WEAPON_FLARE", "WEAPON_MOLOTOV", "WEAPON_STICKYBOMB", "WEAPON_PROXMINE", "WEAPON_SNOWBALL", "WEAPON_PIPEBOMB", "WEAPON_BALL" } }
        };

        #endregion

        #region Voice Settings

        /// <summary>
        /// Voice range options in meters
        /// </summary>
        public static readonly float[] VoiceRanges = { 3.0f, 5.0f, 10.0f, 15.0f, 20.0f, 30.0f };

        /// <summary>
        /// Voice range labels
        /// </summary>
        public static readonly string[] VoiceRangeLabels = { "Whisper (3m)", "Normal (5m)", "Shout (10m)", "Extended (15m)", "Long (20m)", "Max (30m)" };

        #endregion
    }
}
