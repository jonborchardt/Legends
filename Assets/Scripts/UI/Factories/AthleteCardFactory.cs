// VISUAL OUTPUT — one card per athlete, stacked vertically in a scroll view:
// ┌───────────────────────────────────┐
// │  Athlete Name                     │  ← FontSection
// │  DEX  [████████░░]                │  ← stat label + slider bar (0–10)
// │  CON  [██████░░░░]                │
// │  FOC  [█████████░]                │
// │  STR  [███░░░░░░░]                │
// │  LCK  [████░░░░░░]                │
// └───────────────────────────────────┘
//
// GOAL: Render a single athlete's identity and stats as a self-sizing card.
// Card height is content-driven (ContentSizeFitter). Caller owns placement in a scroll view.

using UnityEngine;
using UnityEngine.UI;
using Legends.Data;

namespace Legends.UI
{
    public static class AthleteCardFactory
    {
        public static GameObject Create(Transform parent, AthleteState athlete)
        {
            // VLG + CSF live directly on the card panel — no inner GameObject, no deferred-destroy conflict.
            var card = UIFactory.CreatePanel(parent, $"Card_{athlete.Name}", UIStyle.Surface);

            var vl = card.AddComponent<VerticalLayoutGroup>();
            vl.spacing               = 4f;
            vl.childControlWidth     = true;
            vl.childControlHeight    = true;
            vl.childForceExpandWidth  = true;
            vl.childForceExpandHeight = false;
            vl.padding = new RectOffset(16, 16, 16, 16);

            card.AddComponent<ContentSizeFitter>().verticalFit =
                ContentSizeFitter.FitMode.PreferredSize;

            UIFactory.CreateText(card.transform, "Name", athlete.Name, UIStyle.FontSection);

            AddStatRow(card.transform, "DEX", athlete.Stats.Dexterity);
            AddStatRow(card.transform, "CON", athlete.Stats.Constitution);
            AddStatRow(card.transform, "FOC", athlete.Stats.Focus);
            AddStatRow(card.transform, "STR", athlete.Stats.Strength);
            AddStatRow(card.transform, "LCK", athlete.Stats.Luck);

            return card;
        }

        static void AddStatRow(Transform parent, string label, int value)
        {
            // Build the row directly — no factory CSF to destroy, no deferred issues.
            var row = new GameObject($"Row_{label}");
            row.transform.SetParent(parent, false);

            var hg = row.AddComponent<HorizontalLayoutGroup>();
            hg.spacing               = 8f;
            hg.childControlWidth     = true;
            hg.childControlHeight    = true;
            hg.childForceExpandWidth  = false;
            hg.childForceExpandHeight = true;
            hg.padding = new RectOffset(0, 0, 0, 0);

            row.AddComponent<LayoutElement>().minHeight = 22f;

            var lbl = UIFactory.CreateText(row.transform, "Label", label, UIStyle.FontSmall);
            lbl.GetComponent<LayoutElement>().minWidth = 50f;

            var slider = UIFactory.CreateSlider(row.transform, "Bar", 0f, 10f, value);
            slider.GetComponent<LayoutElement>().flexibleWidth = 1f;
        }
    }
}
