using UnityEngine;
using Legends.Data;

namespace Legends.UI
{
    public static class AthleteCardFactory
    {
        public static GameObject Create(Transform parent, AthleteState athlete)
        {
            var card   = UIFactory.CreatePanel(parent, $"Card_{athlete.Name}", UIStyle.Surface);
            var cardRt = card.GetComponent<RectTransform>();
            cardRt.sizeDelta = new Vector2(0, 160f);
            card.AddComponent<UnityEngine.UI.LayoutElement>().minHeight = 160f;

            var layout = UIFactory.CreateVerticalGroup(card.transform, "Layout", spacing: 4f);
            var layoutRt = layout.GetComponent<RectTransform>();
            UIFactory.FillParent(layoutRt);
            Object.Destroy(layout.GetComponent<UnityEngine.UI.ContentSizeFitter>());

            UIFactory.CreateText(layout.transform, "Name", athlete.Name, UIStyle.FontSection);

            AddStatRow(layout.transform, "DEX", athlete.Stats.Dexterity);
            AddStatRow(layout.transform, "CON", athlete.Stats.Constitution);
            AddStatRow(layout.transform, "FOC", athlete.Stats.Focus);
            AddStatRow(layout.transform, "STR", athlete.Stats.Strength);
            AddStatRow(layout.transform, "LCK", athlete.Stats.Luck);

            return card;
        }

        static void AddStatRow(Transform parent, string label, int value)
        {
            var row = UIFactory.CreateHorizontalGroup(parent, $"Row_{label}", spacing: 8f);
            Object.Destroy(row.GetComponent<UnityEngine.UI.ContentSizeFitter>());
            row.AddComponent<UnityEngine.UI.LayoutElement>().minHeight = 22f;

            var lbl = UIFactory.CreateText(row.transform, "Label", label, UIStyle.FontSmall);
            lbl.GetComponent<UnityEngine.UI.LayoutElement>().minWidth = 50f;

            UIFactory.CreateSlider(row.transform, "Bar", 0f, 10f, value);
        }
    }
}
