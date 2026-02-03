using System;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using LemonUI.Menus;

using CBPSMenu.Shared;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Voice Options Menu - Integration with pma-voice
    /// Custom override replacing vMenu's voice logic
    /// </summary>
    public class VoiceMenu
    {
        #region Variables

        public NativeMenu Menu { get; private set; }

        // Current voice state
        private int _currentRangeIndex = 1; // Default to Normal (5m)
        private float _radioFrequency = 0f;

        #endregion

        #region Constructor

        public VoiceMenu()
        {
            CreateMenu();
        }

        #endregion

        #region Menu Creation

        private void CreateMenu()
        {
            Menu = ThemeManager.CreateThemedMenu("comboom.sucht", "Voice Options");

            // Voice Range Header
            var voiceHeader = new NativeItem("~b~=== Voice Range (pma-voice) ===", "Control your voice chat range");
            voiceHeader.Enabled = false;
            Menu.Add(voiceHeader);

            // Voice Range List
            var voiceRangeItem = new NativeListItem<string>("Voice Range", "Set your voice chat range",
                ConfigManager.VoiceRangeLabels);
            voiceRangeItem.SelectedIndex = _currentRangeIndex;
            voiceRangeItem.ItemChanged += (sender, args) =>
            {
                _currentRangeIndex = voiceRangeItem.SelectedIndex;
                float range = ConfigManager.VoiceRanges[_currentRangeIndex];
                SetVoiceRange(range);
            };
            Menu.Add(voiceRangeItem);

            // Quick Voice Range Buttons
            var whisperItem = new NativeItem("Whisper (3m)", "Set voice to whisper range");
            whisperItem.Activated += (sender, args) =>
            {
                SetVoiceRange(3.0f);
                _currentRangeIndex = 0;
                voiceRangeItem.SelectedIndex = 0;
            };
            Menu.Add(whisperItem);

            var normalItem = new NativeItem("Normal (5m)", "Set voice to normal range");
            normalItem.Activated += (sender, args) =>
            {
                SetVoiceRange(5.0f);
                _currentRangeIndex = 1;
                voiceRangeItem.SelectedIndex = 1;
            };
            Menu.Add(normalItem);

            var shoutItem = new NativeItem("Shout (10m)", "Set voice to shout range");
            shoutItem.Activated += (sender, args) =>
            {
                SetVoiceRange(10.0f);
                _currentRangeIndex = 2;
                voiceRangeItem.SelectedIndex = 2;
            };
            Menu.Add(shoutItem);

            // Add separator
            Menu.Add(new NativeSeparatorItem());

            // Radio Header (pma-radio integration)
            var radioHeader = new NativeItem("~b~=== Radio (pma-radio) ===", "Control your radio frequency");
            radioHeader.Enabled = false;
            Menu.Add(radioHeader);

            // Set Radio Frequency
            var setFrequencyItem = new NativeItem("Set Radio Frequency", "Enter a radio frequency to join");
            setFrequencyItem.Activated += async (sender, args) =>
            {
                var input = await Main.GetUserInput("Enter frequency (1.0 - 999.9)", _radioFrequency.ToString("F1"), 6);
                if (float.TryParse(input, out float frequency))
                {
                    if (frequency >= 1.0f && frequency <= 999.9f)
                    {
                        SetRadioFrequency(frequency);
                    }
                    else
                    {
                        Main.ShowNotification("~r~Invalid frequency! Range: 1.0 - 999.9");
                    }
                }
            };
            Menu.Add(setFrequencyItem);

            // Show Current Frequency
            var currentFreqItem = new NativeItem("Current Frequency", $"Current: {(_radioFrequency > 0 ? _radioFrequency.ToString("F1") : "None")}");
            currentFreqItem.Enabled = false;
            Menu.Add(currentFreqItem);

            // Leave Radio
            var leaveRadioItem = new NativeItem("Leave Radio", "~r~Disconnect from current radio frequency");
            leaveRadioItem.Activated += (sender, args) =>
            {
                TurnOffRadio();
            };
            Menu.Add(leaveRadioItem);

            // Add separator
            Menu.Add(new NativeSeparatorItem());

            // Quick Radio Channels
            var radioChannelsHeader = new NativeItem("~b~=== Quick Radio Channels ===", "Quickly join preset channels");
            radioChannelsHeader.Enabled = false;
            Menu.Add(radioChannelsHeader);

            // Add some preset channels
            AddPresetChannel("Emergency Channel", 911.0f);
            AddPresetChannel("Public Channel 1", 100.0f);
            AddPresetChannel("Public Channel 2", 200.0f);
            AddPresetChannel("Team Channel 1", 10.0f);
            AddPresetChannel("Team Channel 2", 20.0f);
        }

        private void AddPresetChannel(string name, float frequency)
        {
            var channelItem = new NativeItem($"{name} ({frequency:F1})", $"Join {name} on frequency {frequency:F1}");
            channelItem.Activated += (sender, args) =>
            {
                SetRadioFrequency(frequency);
            };
            Menu.Add(channelItem);
        }

        #endregion

        #region pma-voice Integration

        /// <summary>
        /// Set the voice range using pma-voice exports
        /// </summary>
        private void SetVoiceRange(float range)
        {
            try
            {
                // Call pma-voice export to set voice range
                BaseScript.TriggerEvent("pma-voice:setVoiceProperty", "range", range);
                
                // Also try the export method
                // exports['pma-voice']:setVoiceProperty('range', range)
                
                Main.ShowNotification($"~b~Voice range set to: {range}m");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[comboom.sucht] Error setting voice range: {ex.Message}");
                Main.ShowNotification("~r~Failed to set voice range. Is pma-voice installed?");
            }
        }

        /// <summary>
        /// Set the radio frequency using pma-radio exports
        /// </summary>
        private void SetRadioFrequency(float frequency)
        {
            try
            {
                _radioFrequency = frequency;
                
                // Call pma-radio export to set frequency
                BaseScript.TriggerEvent("pma-radio:setRadioFrequency", frequency);
                
                Main.ShowNotification($"~b~Radio frequency set to: {frequency:F1}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[comboom.sucht] Error setting radio frequency: {ex.Message}");
                Main.ShowNotification("~r~Failed to set radio frequency. Is pma-radio installed?");
            }
        }

        /// <summary>
        /// Turn off the radio
        /// </summary>
        private void TurnOffRadio()
        {
            if (_radioFrequency > 0)
            {
                try
                {
                    // Call pma-radio export to leave radio
                    BaseScript.TriggerEvent("pma-radio:setRadioFrequency", 0);
                    _radioFrequency = 0;
                    
                    Main.ShowNotification("~r~Radio: OFF");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[comboom.sucht] Error turning off radio: {ex.Message}");
                    Main.ShowNotification("~r~Failed to turn off radio");
                }
            }
            else
            {
                Main.ShowNotification("~y~Radio is already off");
            }
        }

        /// <summary>
        /// Cycle voice range (can be called externally)
        /// </summary>
        public void CycleVoiceRange()
        {
            _currentRangeIndex = (_currentRangeIndex + 1) % ConfigManager.VoiceRanges.Length;
            float range = ConfigManager.VoiceRanges[_currentRangeIndex];
            SetVoiceRange(range);
        }

        #endregion
    }
}
