using System;
using System.Collections.Generic;
using CitizenFX.Core;
using CitizenFX.Core.Native;
using LemonUI.Menus;

using CBPSMenu.Shared;

namespace CBPSMenu.Client.Menus
{
    /// <summary>
    /// Voice Options Menu - Integration with pma-voice
    /// Uses pma-voice exports for voice range and radio channel control
    /// Theme: Nord11 (Red) Header
    /// </summary>
    public class VoiceMenu
    {
        #region Variables

        public NativeMenu Menu { get; private set; }

        // Voice range options (Whisper, Normal, Shout)
        private static readonly float[] VoiceRanges = { 3.0f, 10.0f, 25.0f };
        private static readonly string[] VoiceRangeLabels = { "Whisper (3m)", "Normal (10m)", "Shout (25m)" };

        // Radio channel options (1-10)
        private static readonly int[] RadioChannels = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        private static readonly string[] RadioChannelLabels = { "Off", "Channel 1", "Channel 2", "Channel 3", "Channel 4", "Channel 5", "Channel 6", "Channel 7", "Channel 8", "Channel 9", "Channel 10" };

        // Current state
        private int _currentRangeIndex = 1; // Default to Normal (10m)
        private int _currentRadioChannel = 0; // Default to Off

        // List items for reference
        private NativeListItem<string> _voiceRangeItem;
        private NativeListItem<string> _radioChannelItem;

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
            // Create menu with Nord11 (Red) header color
            Menu = new NativeMenu("comboom.sucht", "Voice Options")
            {
                UseMouse = false
            };

            // Apply Nord11 (Red) theme to header
            Menu.Banner.Color = ThemeManager.Nord11; // Red header for voice menu

            // Voice Range Section Header
            var voiceHeader = new NativeItem("~r~=== Voice Proximity (pma-voice) ===", "Control your voice chat proximity range")
            {
                Enabled = false
            };
            Menu.Add(voiceHeader);

            // Voice Range List Item
            _voiceRangeItem = new NativeListItem<string>("Voice Range", "Set your voice chat range using pma-voice", VoiceRangeLabels);
            _voiceRangeItem.SelectedIndex = _currentRangeIndex;
            _voiceRangeItem.ItemChanged += (sender, args) =>
            {
                _currentRangeIndex = _voiceRangeItem.SelectedIndex;
                float range = VoiceRanges[_currentRangeIndex];
                SetVoiceProximityRange(range);
            };
            Menu.Add(_voiceRangeItem);

            // Quick Voice Range Buttons
            var whisperItem = new NativeItem("~w~Whisper (3m)", "Set voice to whisper range - quiet, close proximity only");
            whisperItem.Activated += (sender, args) =>
            {
                SetVoiceProximityRange(3.0f);
                _currentRangeIndex = 0;
                _voiceRangeItem.SelectedIndex = 0;
            };
            Menu.Add(whisperItem);

            var normalItem = new NativeItem("~w~Normal (10m)", "Set voice to normal conversation range");
            normalItem.Activated += (sender, args) =>
            {
                SetVoiceProximityRange(10.0f);
                _currentRangeIndex = 1;
                _voiceRangeItem.SelectedIndex = 1;
            };
            Menu.Add(normalItem);

            var shoutItem = new NativeItem("~w~Shout (25m)", "Set voice to shout range - can be heard from far away");
            shoutItem.Activated += (sender, args) =>
            {
                SetVoiceProximityRange(25.0f);
                _currentRangeIndex = 2;
                _voiceRangeItem.SelectedIndex = 2;
            };
            Menu.Add(shoutItem);

            // Add separator
            Menu.Add(new NativeSeparatorItem());

            // Radio Channel Section Header
            var radioHeader = new NativeItem("~r~=== Radio Channel (pma-voice) ===", "Select a radio channel to communicate with others on the same channel")
            {
                Enabled = false
            };
            Menu.Add(radioHeader);

            // Radio Channel List Item
            _radioChannelItem = new NativeListItem<string>("Radio Channel", "Select a radio channel (1-10) or Off to leave", RadioChannelLabels);
            _radioChannelItem.SelectedIndex = _currentRadioChannel;
            _radioChannelItem.ItemChanged += (sender, args) =>
            {
                _currentRadioChannel = _radioChannelItem.SelectedIndex;
                int channel = RadioChannels[_currentRadioChannel];
                SetRadioChannel(channel);
            };
            Menu.Add(_radioChannelItem);

            // Quick Radio Channel Buttons
            var joinChannel1Item = new NativeItem("~w~Join Channel 1", "Quick join radio channel 1");
            joinChannel1Item.Activated += (sender, args) =>
            {
                SetRadioChannel(1);
                _currentRadioChannel = 1;
                _radioChannelItem.SelectedIndex = 1;
            };
            Menu.Add(joinChannel1Item);

            var joinChannel2Item = new NativeItem("~w~Join Channel 2", "Quick join radio channel 2");
            joinChannel2Item.Activated += (sender, args) =>
            {
                SetRadioChannel(2);
                _currentRadioChannel = 2;
                _radioChannelItem.SelectedIndex = 2;
            };
            Menu.Add(joinChannel2Item);

            var leaveRadioItem = new NativeItem("~r~Leave Radio", "Disconnect from the current radio channel");
            leaveRadioItem.Activated += (sender, args) =>
            {
                SetRadioChannel(0);
                _currentRadioChannel = 0;
                _radioChannelItem.SelectedIndex = 0;
            };
            Menu.Add(leaveRadioItem);

            // Add separator
            Menu.Add(new NativeSeparatorItem());

            // Status Section
            var statusHeader = new NativeItem("~r~=== Current Status ===", "Your current voice settings")
            {
                Enabled = false
            };
            Menu.Add(statusHeader);

            var currentStatusItem = new NativeItem("Show Status", "Display current voice range and radio channel");
            currentStatusItem.Activated += (sender, args) =>
            {
                string rangeLabel = VoiceRangeLabels[_currentRangeIndex];
                string channelLabel = _currentRadioChannel == 0 ? "Off" : $"Channel {_currentRadioChannel}";
                Main.ShowNotification($"~b~Voice Range: ~w~{rangeLabel}\n~b~Radio: ~w~{channelLabel}");
            };
            Menu.Add(currentStatusItem);
        }

        #endregion

        #region pma-voice Integration

        /// <summary>
        /// Set the voice proximity range using pma-voice export
        /// Uses: exports["pma-voice"].overrideProximityRange(float range, bool enabled)
        /// </summary>
        private void SetVoiceProximityRange(float range)
        {
            try
            {
                // Call pma-voice export using native invoke
                // This calls: exports["pma-voice"]:overrideProximityRange(range, true)
                int resourceExportsIdx = API.GetInvokingResource() != null ? 0 : 0;

                // Use Lua-style export call via events (most compatible method)
                BaseScript.TriggerEvent("pma-voice:setVoiceProperty", "proximity", range);

                // Alternative: Try to call the export function directly
                // The pma-voice resource exposes these exports
                API.ExecuteCommand($"setr voice_proximity {range}");

                string label = GetRangeLabelForValue(range);
                Main.ShowNotification($"~b~Voice Range: ~w~{label}");
                Debug.WriteLine($"[comboom.sucht] Voice range set to: {range}m");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[comboom.sucht] Error setting voice range: {ex.Message}");
                Main.ShowNotification("~r~Failed to set voice range. Is pma-voice installed?");
            }
        }

        /// <summary>
        /// Set the radio channel using pma-voice export
        /// Uses: exports["pma-voice"].setRadioChannel(int channel)
        /// </summary>
        private void SetRadioChannel(int channel)
        {
            try
            {
                // Call pma-voice export using events (most compatible method)
                // This triggers the pma-voice radio channel change
                BaseScript.TriggerEvent("pma-voice:setRadioChannel", channel);

                if (channel == 0)
                {
                    Main.ShowNotification("~r~Radio: ~w~Disconnected");
                    Debug.WriteLine("[comboom.sucht] Radio channel disconnected");
                }
                else
                {
                    Main.ShowNotification($"~b~Radio Channel: ~w~{channel}");
                    Debug.WriteLine($"[comboom.sucht] Radio channel set to: {channel}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[comboom.sucht] Error setting radio channel: {ex.Message}");
                Main.ShowNotification("~r~Failed to set radio channel. Is pma-voice installed?");
            }
        }

        /// <summary>
        /// Get the display label for a voice range value
        /// </summary>
        private string GetRangeLabelForValue(float range)
        {
            if (range <= 3.0f) return "Whisper (3m)";
            if (range <= 10.0f) return "Normal (10m)";
            return "Shout (25m)";
        }

        /// <summary>
        /// Cycle through voice ranges (can be called externally via keybind)
        /// </summary>
        public void CycleVoiceRange()
        {
            _currentRangeIndex = (_currentRangeIndex + 1) % VoiceRanges.Length;
            float range = VoiceRanges[_currentRangeIndex];
            SetVoiceProximityRange(range);
            _voiceRangeItem.SelectedIndex = _currentRangeIndex;
        }

        /// <summary>
        /// Get the current voice range
        /// </summary>
        public float GetCurrentVoiceRange()
        {
            return VoiceRanges[_currentRangeIndex];
        }

        /// <summary>
        /// Get the current radio channel
        /// </summary>
        public int GetCurrentRadioChannel()
        {
            return _currentRadioChannel;
        }

        #endregion
    }
}
