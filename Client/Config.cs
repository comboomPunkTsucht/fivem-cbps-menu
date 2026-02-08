using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using CitizenFX.Core;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using static CitizenFX.Core.Native.API;

namespace CBPSMenu.Client
{
    /// <summary>
    /// Configuration loader for comboom.sucht Menu settings.
    /// </summary>
    public static class Config
    {
        public static string MenuTitle { get; private set; } = "comboom.sucht Menu";
        public static string MenuSubtitle { get; private set; } = "Server Menu";
        public static string MenuKey { get; private set; } = "F1";

        // Theme colors
        public static Color HeaderColor { get; private set; } = Color.FromArgb(255, 94, 129, 172);
        public static Color HighlightColor { get; private set; } = Color.FromArgb(255, 136, 192, 208);
        public static Color BackgroundColor { get; private set; } = Color.FromArgb(200, 46, 52, 64);
        public static Color TextColor { get; private set; } = Color.FromArgb(255, 236, 239, 244);

        // Banner texture
        public static string BannerDictionary { get; private set; } = "commonmenu";
        public static string BannerTexture { get; private set; } = "interaction_bgd";

        // Teams configuration
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

        // Racing settings
        public static string CheckpointModel { get; private set; } = "prop_mp_cone_01";
        public static string FinishModel { get; private set; } = "prop_mp_cone_02";
        public static int CountdownSeconds { get; private set; } = 3;

        /// <summary>
        /// Loads configuration from config.json file.
        /// </summary>
        public static void Load()
        {
            try
            {
                var resourceName = GetCurrentResourceName();
                var configFile = LoadResourceFile(resourceName, "config.json");

                if (string.IsNullOrEmpty(configFile))
                {
                    Debug.WriteLine("[comboom.sucht Menu] config.json not found, using defaults.");
                    return;
                }

                var config = JObject.Parse(configFile);

                // Basic settings
                MenuTitle = config["menuTitle"]?.ToString() ?? MenuTitle;
                MenuSubtitle = config["menuSubtitle"]?.ToString() ?? MenuSubtitle;
                MenuKey = config["menuKey"]?.ToString() ?? MenuKey;

                // Theme colors
                if (config["theme"] is JObject theme)
                {
                    HeaderColor = ParseColor(theme["headerColor"]) ?? HeaderColor;
                    HighlightColor = ParseColor(theme["highlightColor"]) ?? HighlightColor;
                    BackgroundColor = ParseColor(theme["backgroundColor"]) ?? BackgroundColor;
                    TextColor = ParseColor(theme["textColor"]) ?? TextColor;
                }

                // Banner texture
                if (config["bannerTexture"] is JObject banner)
                {
                    BannerDictionary = banner["dictionary"]?.ToString() ?? BannerDictionary;
                    BannerTexture = banner["texture"]?.ToString() ?? BannerTexture;
                }

                // Teams
                if (config["teams"] is JObject teams)
                {
                    Teams.Clear();
                    foreach (var team in teams.Properties())
                    {
                        var teamData = team.Value as JObject;
                        if (teamData != null)
                        {
                            Teams[team.Name] = new TeamConfig
                            {
                                Frequency = teamData["frequency"]?.Value<int>() ?? 100,
                                Color = teamData["color"]?.ToString() ?? "#FFFFFF"
                            };
                        }
                    }
                }

                // Voice settings
                if (config["voiceSettings"] is JObject voice)
                {
                    DefaultProximity = voice["defaultProximity"]?.Value<float>() ?? DefaultProximity;
                    EnableRadioByDefault = voice["enableRadioByDefault"]?.Value<bool>() ?? EnableRadioByDefault;
                }

                // Racing settings
                if (config["racing"] is JObject racing)
                {
                    CheckpointModel = racing["checkpointModel"]?.ToString() ?? CheckpointModel;
                    FinishModel = racing["finishModel"]?.ToString() ?? FinishModel;
                    CountdownSeconds = racing["countdownSeconds"]?.Value<int>() ?? CountdownSeconds;
                }

                Debug.WriteLine("[comboom.sucht Menu] Configuration loaded successfully.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[comboom.sucht Menu] Error loading config: {ex.Message}");
            }
        }

        /// <summary>
        /// Parses a color from JSON object with r, g, b, a properties.
        /// </summary>
        private static Color? ParseColor(JToken token)
        {
            if (token is JObject colorObj)
            {
                var r = colorObj["r"]?.Value<int>() ?? 255;
                var g = colorObj["g"]?.Value<int>() ?? 255;
                var b = colorObj["b"]?.Value<int>() ?? 255;
                var a = colorObj["a"]?.Value<int>() ?? 255;
                return Color.FromArgb(a, r, g, b);
            }
            return null;
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
