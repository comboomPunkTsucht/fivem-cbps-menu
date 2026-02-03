using System.Drawing;
using LemonUI.Menus;
using LemonUI.Elements;

namespace CBPSMenu.Shared
{
    /// <summary>
    /// Manages theme colors and styling for all menus
    /// Uses Nord color palette: https://www.nordtheme.com/
    /// </summary>
    public static class ThemeManager
    {
        #region Nord Color Palette

        // Polar Night (Dark backgrounds)
        public static readonly Color Nord0 = Color.FromArgb(255, 46, 52, 64);      // Background
        public static readonly Color Nord1 = Color.FromArgb(255, 59, 66, 82);      // Lighter background
        public static readonly Color Nord2 = Color.FromArgb(255, 67, 76, 94);      // Selection background
        public static readonly Color Nord3 = Color.FromArgb(255, 76, 86, 106);     // Comments/subtle

        // Snow Storm (Light text)
        public static readonly Color Nord4 = Color.FromArgb(255, 216, 222, 233);   // Primary text
        public static readonly Color Nord5 = Color.FromArgb(255, 229, 233, 240);   // Bright text
        public static readonly Color Nord6 = Color.FromArgb(255, 236, 239, 244);   // Brightest text

        // Frost (Accent colors)
        public static readonly Color Nord7 = Color.FromArgb(255, 143, 188, 187);   // Cyan/Teal
        public static readonly Color Nord8 = Color.FromArgb(255, 136, 192, 208);   // Light blue
        public static readonly Color Nord9 = Color.FromArgb(255, 129, 161, 193);   // Blue
        public static readonly Color Nord10 = Color.FromArgb(255, 94, 129, 172);   // Dark blue

        // Aurora (Status colors)
        public static readonly Color Nord11 = Color.FromArgb(255, 191, 97, 106);   // Red (error)
        public static readonly Color Nord12 = Color.FromArgb(255, 208, 135, 112);  // Orange (warning)
        public static readonly Color Nord13 = Color.FromArgb(255, 235, 203, 139);  // Yellow (highlight)
        public static readonly Color Nord14 = Color.FromArgb(255, 163, 190, 140);  // Green (success)
        public static readonly Color Nord15 = Color.FromArgb(255, 180, 142, 173);  // Purple

        #endregion

        #region Theme Presets

        // Current theme colors (Nord by default)
        public static Color BannerColor { get; set; } = Nord0;
        public static Color HeaderColor { get; set; } = Nord14;
        public static Color ItemBackgroundColor { get; set; } = Nord1;
        public static Color SelectedBackgroundColor { get; set; } = Nord2;
        public static Color TextColor { get; set; } = Nord4;
        public static Color HighlightTextColor { get; set; } = Nord6;
        public static Color AccentColor { get; set; } = Nord8;
        public static Color SuccessColor { get; set; } = Nord14;
        public static Color ErrorColor { get; set; } = Nord11;
        public static Color WarningColor { get; set; } = Nord12;

        #endregion

        #region Theme Application Methods

        /// <summary>
        /// Apply the Nord theme to a menu
        /// </summary>
        public static void ApplyNordTheme(NativeMenu menu)
        {
            if (menu == null) return;

            // Set banner color to Nord0 (dark background)
            menu.Banner.Color = Nord0;

            // Set description background (if accessible)
            // Note: Some properties may not be directly accessible in LemonUI
            
            // Apply to the menu
            ApplyThemeColors(menu);
        }

        /// <summary>
        /// Apply theme colors to a menu
        /// </summary>
        public static void ApplyThemeColors(NativeMenu menu)
        {
            if (menu == null) return;

            // Set the banner (header) background color
            menu.Banner.Color = BannerColor;
        }

        /// <summary>
        /// Create a styled menu with Nord theme applied
        /// </summary>
        public static NativeMenu CreateThemedMenu(string title, string subtitle)
        {
            var menu = new NativeMenu(title, subtitle)
            {
                UseMouse = false
            };

            ApplyNordTheme(menu);
            return menu;
        }

        /// <summary>
        /// Apply theme to all menus in the application
        /// </summary>
        public static void RefreshAllThemes()
        {
            // This will be called when theme is changed
            // Individual menus can subscribe to this
        }

        #endregion

        #region Theme Switching

        /// <summary>
        /// Set the Nord theme (default)
        /// </summary>
        public static void SetNordTheme()
        {
            BannerColor = Nord0;
            HeaderColor = Nord14;
            ItemBackgroundColor = Nord1;
            SelectedBackgroundColor = Nord2;
            TextColor = Nord4;
            HighlightTextColor = Nord6;
            AccentColor = Nord8;
            SuccessColor = Nord14;
            ErrorColor = Nord11;
            WarningColor = Nord12;
        }

        /// <summary>
        /// Set a custom theme
        /// </summary>
        public static void SetCustomTheme(Color banner, Color header, Color text)
        {
            BannerColor = banner;
            HeaderColor = header;
            TextColor = text;
        }

        #endregion
    }
}
