using System.Drawing;

namespace LibraryManagementSystem.Utils
{
    public static class Theme
    {
        // Colors
        public static Color Primary = Color.FromArgb(59, 130, 246);    // Blue 500
        public static Color Secondary = Color.FromArgb(31, 41, 55);  // Gray 800 (Sidebar)
        public static Color Success = Color.FromArgb(16, 185, 129);    // Emerald 500
        public static Color Danger = Color.FromArgb(239, 68, 68);      // Red 500
        public static Color Background = Color.FromArgb(245, 247, 251);
        public static Color CardBackground = Color.White;
        public static Color TextPrimary = Color.FromArgb(17, 24, 39);
        public static Color TextSecondary = Color.FromArgb(107, 114, 128);

        // Fonts
        public static Font TitleFont = new Font("Segoe UI", 16, FontStyle.Bold);
        public static Font HeaderFont = new Font("Segoe UI", 14, FontStyle.Bold);
        public static Font BodyFont = new Font("Segoe UI", 10);
        public static Font ButtonFont = new Font("Segoe UI", 10, FontStyle.Bold);
    }
}
