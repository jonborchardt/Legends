using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Legends.Data;
using Legends.Services;

namespace Legends.UI
{
    // Rebuilds on every Show() so it always reflects the latest race result.
    public class ResultsScreen : UIScreen
    {
        public override void Show()
        {
            if (IsBuilt)
            {
                Object.DestroyImmediate(Root);
                Root = null;
            }
            base.Show();
        }

        protected override GameObject Build()
        {
            var result = GameSession.LatestResult;
            if (result == null)
            {
                ScreenRegistry.Instance.ShowScreen<TeamHubScreen>();
                return null;
            }

            var saveService = new PlayerPrefsSaveService();
            var state       = saveService.Load();
            var nameMap     = BuildNameMap(state);

            const float HeaderH = 80f;
            const float FooterH = 80f;

            var canvas = UIFactory.CreateCanvas("Results");
            var root   = UIFactory.CreatePanel(canvas.transform, "Root", UIStyle.Background);
            UIFactory.FillParent(root.GetComponent<RectTransform>());

            // Title — pinned to top
            var title   = UIFactory.CreateText(root.transform, "Title", "Results", UIStyle.FontTitle);
            var titleRt = title.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0, 1);
            titleRt.anchorMax = new Vector2(1, 1);
            titleRt.offsetMin = new Vector2(16, -HeaderH);
            titleRt.offsetMax = new Vector2(-16, 0);

            // Button — pinned to bottom
            var btn   = UIFactory.CreateButton(root.transform, "Back", "Back to Team", OnBack);
            var btnRt = btn.GetComponent<RectTransform>();
            btnRt.anchorMin = new Vector2(0, 0);
            btnRt.anchorMax = new Vector2(1, 0);
            btnRt.offsetMin = new Vector2(16, 8);
            btnRt.offsetMax = new Vector2(-16, FooterH - 8);

            // Scroll — fills between title and button
            Transform scrollContent;
            var scroll   = UIFactory.CreateScrollView(root.transform, "Scroll", out scrollContent);
            var scrollRt = scroll.GetComponent<RectTransform>();
            scrollRt.anchorMin = Vector2.zero;
            scrollRt.anchorMax = Vector2.one;
            scrollRt.offsetMin = new Vector2(0, FooterH);
            scrollRt.offsetMax = new Vector2(0, -HeaderH);

            // Rankings
            UIFactory.CreateText(scrollContent, "RankingsHeader", "Final Standings", UIStyle.FontSection);
            for (int i = 0; i < result.Placements.Count; i++)
            {
                string name = nameMap.GetValueOrDefault(result.Placements[i], "Unknown");
                ResultRowFactory.Create(scrollContent, name, i + 1);
            }

            // Highlights
            var notable = result.Timeline
                .Where(e => e.EventType is EventTypes.EggDrop or EventTypes.Surge or EventTypes.Stumble)
                .Take(3)
                .ToList();

            if (notable.Count > 0)
            {
                UIFactory.CreateText(scrollContent, "EventsHeader", "Highlights", UIStyle.FontSection);
                foreach (var evt in notable)
                {
                    string name = nameMap.GetValueOrDefault(evt.AthleteId, "Someone");
                    string text = FormatEvent(evt.EventType, name);
                    var lbl = UIFactory.CreateText(scrollContent, $"Evt_{evt.EventType}", text, UIStyle.FontBody);
                    lbl.color = EventColor(evt.EventType);
                }
            }

            return canvas;
        }

        static Dictionary<string, string> BuildNameMap(GameState state)
        {
            if (state?.Team?.Roster == null) return new Dictionary<string, string>();
            return state.Team.Roster.ToDictionary(a => a.Id, a => a.Name);
        }

        static string FormatEvent(string eventType, string name) => eventType switch
        {
            EventTypes.EggDrop => $"{name} dropped the egg!",
            EventTypes.Surge   => $"{name} surged ahead!",
            EventTypes.Stumble => $"{name} stumbled!",
            _                  => eventType
        };

        static UnityEngine.Color EventColor(string eventType) => eventType switch
        {
            EventTypes.EggDrop => UIStyle.Danger,
            EventTypes.Surge   => UIStyle.Success,
            _                  => UIStyle.TextSecondary,
        };

        static void OnBack() => ScreenRegistry.Instance.ShowScreen<TeamHubScreen>();
    }
}
