using System;
using System.Collections.Generic;
using System.Drawing;

using CitizenFX.Core;

using static CitizenFX.Core.Native.API;

using CBPSMenu.Shared;

namespace CBPSMenu.Client
{
    /// <summary>
    /// Configuration loader for comboom.sucht Menu settings using ConVars.
    /// No external JSON library needed - uses FiveM ConVars and simple parsing.
    /// </summary>
    public static class Config
    {
        public static string MenuTitle { get; private set; } = "comboom.sucht Menu";
        public static string MenuSubtitle { get; private set; } = "Server Menu";
        public static string MenuKey { get; private set; } = "M";

        // Theme colors
        public static Color HeaderColor { get; private set; } = Color.FromArgb(255, 94, 129, 172);
        public static Color HighlightColor { get; private set; } = Color.FromArgb(255, 136, 192, 208);
        public static Color BackgroundColor { get; private set; } = Color.FromArgb(200, 46, 52, 64);
        public static Color TextColor { get; private set; } = Color.FromArgb(255, 236, 239, 244);

        // Banner texture
        public static string BannerDictionary { get; private set; } = "commonmenu";
        public static string BannerTexture { get; private set; } = "interaction_bgd";

        // Teams configuration (hardcoded defaults, can be overridden via ConVars)
        public static Dictionary<string, TeamConfig> Teams { get; private set; } = new Dictionary<string, TeamConfig>
        {
            { "Team A", new TeamConfig { Frequency = 100, Color = "#BF616A" } },
            { "Team B", new TeamConfig { Frequency = 200, Color = "#A3BE8C" } },
            { "Team C", new TeamConfig { Frequency = 300, Color = "#EBCB8B" } },
            { "Team D", new TeamConfig { Frequency = 400, Color = "#B48EAD" } }
        };

        // Voice settings
        public static float DefaultProximity { get; private set; } = 15.0f;
        public static bool EnableRadioByDefault { get; private set; } = true;

        // Permissions settings
        public static bool UsePermissions { get; private set; } = true;

        // Racing settings
        public static string CheckpointModel { get; private set; } = "prop_mp_cone_01";
        public static string FinishModel { get; private set; } = "prop_mp_cone_02";
        public static int CountdownSeconds { get; private set; } = 3;

        /// <summary>
        /// Loads configuration from ConVars.
        /// Set ConVars in your server.cfg, e.g.: setr cbps_menu_title "My Server Menu"
        /// </summary>
        public static void Load()
        {
            try
            {
                // Basic settings from ConVars
                var title = GetConvar("cbps_menu_title", "");
                if (!string.IsNullOrEmpty(title)) MenuTitle = title;

                var subtitle = GetConvar("cbps_menu_subtitle", "");
                if (!string.IsNullOrEmpty(subtitle)) MenuSubtitle = subtitle;

                var key = GetConvar("cbps_menu_key", "");
                if (!string.IsNullOrEmpty(key)) MenuKey = key;

                // Theme colors from ConVars (format: "r,g,b,a")
                HeaderColor = ParseColorConvar("cbps_header_color", HeaderColor);
                HighlightColor = ParseColorConvar("cbps_highlight_color", HighlightColor);
                BackgroundColor = ParseColorConvar("cbps_background_color", BackgroundColor);
                TextColor = ParseColorConvar("cbps_text_color", TextColor);

                // Banner texture
                var bannerDict = GetConvar("cbps_banner_dictionary", "");
                if (!string.IsNullOrEmpty(bannerDict)) BannerDictionary = bannerDict;

                var bannerTex = GetConvar("cbps_banner_texture", "");
                if (!string.IsNullOrEmpty(bannerTex)) BannerTexture = bannerTex;

                // Voice settings
                var proximity = GetConvarInt("cbps_default_proximity", -1);
                if (proximity > 0) DefaultProximity = proximity;

                var radioDefault = GetConvar("cbps_enable_radio_default", "");
                if (radioDefault.ToLower() == "false") EnableRadioByDefault = false;
                else if (radioDefault.ToLower() == "true") EnableRadioByDefault = true;

                // Racing settings
                var checkpoint = GetConvar("cbps_checkpoint_model", "");
                if (!string.IsNullOrEmpty(checkpoint)) CheckpointModel = checkpoint;

                var finish = GetConvar("cbps_finish_model", "");
                if (!string.IsNullOrEmpty(finish)) FinishModel = finish;

                var countdown = GetConvarInt("cbps_countdown_seconds", -1);
                if (countdown > 0) CountdownSeconds = countdown;

                var teamsConfig = GetConvar("cbps_teams", "");
                if (!string.IsNullOrEmpty(teamsConfig))
                {
                    ParseTeamsConfig(teamsConfig);
                }

                // Permissions toggle
                var usePerms = GetConvar("cbps_use_permissions", "true");
                if (usePerms.ToLower() == "false") UsePermissions = false;
                else UsePermissions = true;

                PermissionsManager.UsePermissions = UsePermissions;

                Debug.WriteLine("[comboom.sucht Menu] Configuration loaded successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[comboom.sucht Menu] Error loading config: {ex.Message}");
            }
        }

        /// <summary>
        /// Parses a color from ConVar string format: "r,g,b" or "r,g,b,a"
        /// </summary>
        private static Color ParseColorConvar(string convarName, Color defaultColor)
        {
            try
            {
                var value = GetConvar(convarName, "");
                if (string.IsNullOrEmpty(value)) return defaultColor;

                var parts = value.Split(',');
                if (parts.Length >= 3)
                {
                    int r = int.Parse(parts[0].Trim());
                    int g = int.Parse(parts[1].Trim());
                    int b = int.Parse(parts[2].Trim());
                    int a = parts.Length >= 4 ? int.Parse(parts[3].Trim()) : 255;
                    return Color.FromArgb(a, r, g, b);
                }
            }
            catch { }
            return defaultColor;
        }

        /// <summary>
        /// Parses teams configuration from string format: "TeamName:Frequency:Color;..."
        /// </summary>
        private static void ParseTeamsConfig(string config)
        {
            try
            {
                Teams.Clear();
                var teamEntries = config.Split(';');
                foreach (var entry in teamEntries)
                {
                    var parts = entry.Split(':');
                    if (parts.Length >= 3)
                    {
                        var name = parts[0].Trim();
                        var freq = int.Parse(parts[1].Trim());
                        var color = parts[2].Trim();
                        Teams[name] = new TeamConfig { Frequency = freq, Color = color };
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[comboom.sucht Menu] Error parsing teams config: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Team configuration data.
    /// </summary>
    public class TeamConfig
    {
        public int Frequency { get; set; }
        public string Color { get; set; }
    }
}
