using System;
using System.Collections.Generic;

using CitizenFX.Core;

using LemonUI.Menus;

using CBPSMenu.Shared;

using static CitizenFX.Core.Native.API;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Voice Settings Menu - Configure pma-voice settings.
    /// </summary>
    public class VoiceSettings
    {
        private NativeMenu menu;

        public float CurrentProximity { get; private set; }
        public int CurrentRadioChannel { get; private set; } = 0;
        public bool RadioEnabled { get; private set; } = false;

        private static readonly List<float> ProximityRanges = new List<float>
        {
            5f,    // 5m
            10f,   // 10m
            15f,   // 15m (default)
            20f,   // 20m
            30f,   // 30m
            50f,   // 50m
            100f,  // 100m
        };

        private void CreateMenu()
        {
            menu = new NativeMenu("Voice Settings", "Configure voice chat settings");

            CurrentProximity = Config.DefaultProximity;
            RadioEnabled = Config.EnableRadioByDefault;

            // Voice Proximity selector
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VCSetProximity))
            {
                var proximityStrings = new List<string>();
                int defaultIndex = 2; // 15m
                for (int i = 0; i < ProximityRanges.Count; i++)
                {
                    proximityStrings.Add($"{ProximityRanges[i]}m");
                    if (Math.Abs(ProximityRanges[i] - CurrentProximity) < 0.1f)
                    {
                        defaultIndex = i;
                    }
                }

                var proximityList = new NativeListItem<string>("Voice Proximity", "Set the distance others can hear you.", proximityStrings.ToArray());
                proximityList.SelectedIndex = defaultIndex;
                proximityList.ItemChanged += (sender, e) =>
                {
                    var index = proximityStrings.IndexOf(e.Object);
                    CurrentProximity = ProximityRanges[index];
                    SetProximity(CurrentProximity);
                };
                menu.Add(proximityList);
            }

            // Radio Enable checkbox
            if (PermissionsManager.IsAllowed(PermissionsManager.Permission.VCSetRadioChannel))
            {
                var radioEnabledCheckbox = new NativeCheckboxItem("Radio Enabled", "Enable/disable radio communications.", RadioEnabled);
                radioEnabledCheckbox.CheckboxChanged += (sender, e) =>
                {
                    RadioEnabled = radioEnabledCheckbox.Checked;
                    SetRadioEnabled(RadioEnabled);
                };
                menu.Add(radioEnabledCheckbox);

                // Radio Channel input
                var setRadioChannel = new NativeItem("Set Radio Channel", $"Current: {CurrentRadioChannel} MHz");
                setRadioChannel.Activated += async (sender, e) =>
                {
                    var result = await GetUserInput("Enter Radio Channel (MHz)", "100", 5);
                    if (int.TryParse(result, out int channel) && channel >= 0 && channel <= 999)
                    {
                        CurrentRadioChannel = channel;
                        SetRadioChannel(channel);
                        setRadioChannel.Description = $"Current: {channel} MHz";
                        Notify.Success($"Radio channel set to {channel} MHz.");
                    }
                    else if (!string.IsNullOrEmpty(result))
                    {
                        Notify.Error("Invalid channel. Use 0-999.");
                    }
                };
                menu.Add(setRadioChannel);
            }

            menu.Add(new NativeSeparatorItem());

            // Quick proximity presets
            var whisperItem = new NativeItem("Whisper (5m)", "Set proximity to whisper range.");
            whisperItem.Activated += (sender, e) =>
            {
                CurrentProximity = 5f;
                SetProximity(5f);
                Notify.Info("Voice set to whisper.");
            };
            menu.Add(whisperItem);

            var normalItem = new NativeItem("Normal (15m)", "Set proximity to normal range.");
            normalItem.Activated += (sender, e) =>
            {
                CurrentProximity = 15f;
                SetProximity(15f);
                Notify.Info("Voice set to normal.");
            };
            menu.Add(normalItem);

            var shoutItem = new NativeItem("Shout (30m)", "Set proximity to shout range.");
            shoutItem.Activated += (sender, e) =>
            {
                CurrentProximity = 30f;
                SetProximity(30f);
                Notify.Info("Voice set to shout.");
            };
            menu.Add(shoutItem);
        }

        /// <summary>
        /// Sets the voice proximity via pma-voice export.
        /// </summary>
        private void SetProximity(float proximity)
        {
            try
            {
                // pma-voice uses setVoiceProperty for proximity
                Exports["pma-voice"].setVoiceProperty("proximity", proximity);
                Notify.Info($"Voice proximity set to {proximity}m.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[comboom.sucht Menu] Error setting proximity: {ex.Message}");
            }
        }

        /// <summary>
        /// Enables or disables radio via pma-voice export.
        /// </summary>
        private void SetRadioEnabled(bool enabled)
        {
            try
            {
                Exports["pma-voice"].setVoiceProperty("radioEnabled", enabled);
                Notify.Info($"Radio {(enabled ? "enabled" : "disabled")}.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[comboom.sucht Menu] Error setting radio enabled: {ex.Message}");
            }
        }

        /// <summary>
        /// Sets the radio channel via pma-voice export.
        /// </summary>
        private void SetRadioChannel(int channel)
        {
            try
            {
                Exports["pma-voice"].setRadioChannel(channel);
                CurrentRadioChannel = channel;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[comboom.sucht Menu] Error setting radio channel: {ex.Message}");
            }
        }

        /// <summary>
        /// Helper to get user input.
        /// </summary>
        private async System.Threading.Tasks.Task<string> GetUserInput(string windowTitle, string defaultText, int maxLength)
        {
            AddTextEntry("FMMC_KEY_TIP1", windowTitle);
            DisplayOnscreenKeyboard(1, "FMMC_KEY_TIP1", "", defaultText, "", "", "", maxLength);
            while (UpdateOnscreenKeyboard() == 0)
            {
                await BaseScript.Delay(0);
            }
            return GetOnscreenKeyboardResult();
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
