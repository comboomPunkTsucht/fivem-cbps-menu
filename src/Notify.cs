using CitizenFX.Core;
using CitizenFX.Core.UI;

using static CitizenFX.Core.Native.API;

namespace CBPSMenu.Client
{
    /// <summary>
    /// Notification helper class.
    /// </summary>
    public static class Notify
    {
        /// <summary>
        /// Shows a success notification (green).
        /// </summary>
        public static void Success(string message)
        {
            ShowNotification($"~g~✓~s~ {message}");
        }

        /// <summary>
        /// Shows an error notification (red).
        /// </summary>
        public static void Error(string message)
        {
            ShowNotification($"~r~✗~s~ {message}");
        }

        /// <summary>
        /// Shows an info notification (blue).
        /// </summary>
        public static void Info(string message)
        {
            ShowNotification($"~b~ℹ~s~ {message}");
        }

        /// <summary>
        /// Shows a warning notification (orange).
        /// </summary>
        public static void Warning(string message)
        {
            ShowNotification($"~o~⚠~s~ {message}");
        }

        /// <summary>
        /// Shows a custom notification.
        /// </summary>
        public static void Custom(string message)
        {
            ShowNotification(message);
        }

        private static void ShowNotification(string message)
        {
            SetNotificationTextEntry("STRING");
            AddTextComponentSubstringPlayerName(message);
            DrawNotification(false, false);
        }

        /// <summary>
        /// Shows a subtitle at the bottom of the screen.
        /// </summary>
        public static void Subtitle(string message, int duration = 2500)
        {
            BeginTextCommandPrint("STRING");
            AddTextComponentSubstringPlayerName(message);
            EndTextCommandPrint(duration, true);
        }
    }
}
