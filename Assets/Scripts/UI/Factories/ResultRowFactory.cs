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

        static string Ordinal(int n) => n switch
        {
            1 => "1st", 2 => "2nd", 3 => "3rd", _ => $"{n}th"
        };

        static UnityEngine.Color RowColor(int rank) => rank switch
        {
            1 => UIStyle.Accent,
            2 => UIStyle.Surface,
            3 => UIStyle.Surface,
            _ => UIStyle.Background
        };
    }
}
