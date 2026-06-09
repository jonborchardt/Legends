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

            var canvas = UIFactory.CreateCanvas("Results");
            var root   = UIFactory.CreatePanel(canvas.transform, "Root", UIStyle.Background);
            UIFactory.FillParent(root.GetComponent<RectTransform>());

            UIFactory.CreateText(root.transform, "Title", "Results", UIStyle.FontTitle);

            // Rankings
            UIFactory.CreateText(root.transform, "RankingsHeader", "Final Standings", UIStyle.FontSection);
            Transform rankContent;
            var rankScroll = UIFactory.CreateScrollView(root.transform, "RankScroll", out rankContent);
            rankScroll.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 280f);
            rankScroll.AddComponent<UnityEngine.UI.LayoutElement>().preferredHeight = 280f;

            for (int i = 0; i < result.Placements.Count; i++)
            {
                string name = nameMap.GetValueOrDefault(result.Placements[i], "Unknown");
                ResultRowFactory.Create(rankContent, name, i + 1);
            }

            // Notable events
            var notable = result.Timeline
                .Where(e => e.EventType is EventTypes.EggDrop or EventTypes.Surge or EventTypes.Stumble)
                .Take(3)
                .ToList();

            if (notable.Count > 0)
            {
                UIFactory.CreateText(root.transform, "EventsHeader", "Highlights", UIStyle.FontSection);
                Transform evtContent;
                UIFactory.CreateScrollView(root.transform, "EventScroll", out evtContent);

                foreach (var evt in notable)
                {
                    string name = nameMap.GetValueOrDefault(evt.AthleteId, "Someone");
                    string text = FormatEvent(evt.EventType, name);
                    var lbl = UIFactory.CreateText(evtContent, $"Evt_{evt.EventType}", text, UIStyle.FontBody);
                    lbl.color = EventColor(evt.EventType);
                }
            }

            UIFactory.CreateButton(root.transform, "Back", "Back to Team", OnBack);

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
