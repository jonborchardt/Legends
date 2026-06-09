// DESIGN TOKENS — single source of truth for all colours and font sizes.
//
// COLOUR PALETTE
//   Background   #1A1A2E  ████  deep navy        — full-screen panel fill
//   Surface      #252545  ████  mid navy         — cards, rows, raised panels
//   TextPrimary  #EAEAEA  ████  near-white        — all body / label text
//   TextSecondary#9090A0  ████  muted grey        — secondary labels, captions
//   Accent       #E4A853  ████  warm gold         — buttons, 1st-place rows
//   Danger       #C0392B  ████  red               — egg-drop events, errors
//   Success      #27AE60  ████  green             — surge events, positive feedback
//   StatBarBg    #2A2A4A  ████  dark blue-grey    — empty portion of stat bar
//   StatBarFill  #4A90D9  ████  sky blue          — filled portion of stat bar
//
// FONT SCALE
//   FontTitle   48 pt  — screen title (one per screen, top-centre)
//   FontSection 28 pt  — sub-headers, card names, rank ordinals
//   FontBody    22 pt  — general readable text, button labels
//   FontSmall   16 pt  — stat labels (DEX / CON / …), captions
//
// GOAL: Changing any value here repaints every widget that references it —
// no hunting through individual screens or factories.

using UnityEngine;

namespace Legends.UI
{
    public static class UIStyle
    {
        // Backgrounds
        public static readonly Color Background = Hex("1A1A2E");
        public static readonly Color Surface     = Hex("252545");

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
