// VISUAL OUTPUT — one fixed-height row per placement:
// ┌────────────────────────────────────┐
// │  1st   │  Athlete Name            │  ← rank (80px min-width) + name, 56px tall
// └────────────────────────────────────┘  background: Accent for 1st, Surface for 2–3, Background otherwise
//
// GOAL: Render a single race placement entry inside the Results screen body.
// Caller creates one row per entry in EventResult.Placements, top to bottom.

using UnityEngine;
using Legends.Data;

namespace Legends.UI
{
    public static class ResultRowFactory
    {
        public static GameObject Create(Transform parent, string athleteName, int rank)
        {
            var row   = UIFactory.CreatePanel(parent, $"Row_{rank}", RowColor(rank));
            var rowRt = row.GetComponent<RectTransform>();
            rowRt.sizeDelta = new Vector2(0, 56f);
            row.AddComponent<UnityEngine.UI.LayoutElement>().minHeight = 56f;

            var layout = UIFactory.CreateHorizontalGroup(row.transform, "Layout", spacing: 16f);
            var layoutRt = layout.GetComponent<RectTransform>();
            UIFactory.FillParent(layoutRt);
            Object.Destroy(layout.GetComponent<UnityEngine.UI.ContentSizeFitter>());

            var rankTmp = UIFactory.CreateText(layout.transform, "Rank", Ordinal(rank), UIStyle.FontSection);
            rankTmp.GetComponent<UnityEngine.UI.LayoutElement>().minWidth = 80f;

            UIFactory.CreateText(layout.transform, "Name", athleteName, UIStyle.FontBody);

            return row;
        }

        static string Ordinal(int n)
        {
            if (n % 100 is 11 or 12 or 13) return $"{n}th";
            return (n % 10) switch { 1 => $"{n}st", 2 => $"{n}nd", 3 => $"{n}rd", _ => $"{n}th" };
        }

        static UnityEngine.Color RowColor(int rank) => rank switch
        {
            1 => UIStyle.Accent,
            2 => UIStyle.Surface,
            3 => UIStyle.Surface,
            _ => UIStyle.Background
        };
    }
}
