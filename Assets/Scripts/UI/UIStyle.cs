using UnityEngine;

namespace Legends.UI
{
    public static class UIStyle
    {
        // Backgrounds
        public static readonly Color Background = Hex("1A1A2E");
        public static readonly Color Surface     = Hex("16213E");

        // Text
        public static readonly Color TextPrimary   = Hex("EAEAEA");
        public static readonly Color TextSecondary = Hex("9090A0");

        // Accents
        public static readonly Color Accent  = Hex("E4A853");
        public static readonly Color Danger  = Hex("C0392B");
        public static readonly Color Success = Hex("27AE60");

        // Stat bars
        public static readonly Color StatBarBackground = Hex("2A2A4A");
        public static readonly Color StatBarFill       = Hex("4A90D9");

        // Font sizes (in points)
        public const float FontTitle   = 48f;
        public const float FontSection = 28f;
        public const float FontBody    = 22f;
        public const float FontSmall   = 16f;

        static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString("#" + hex, out Color c);
            return c;
        }
    }
}
