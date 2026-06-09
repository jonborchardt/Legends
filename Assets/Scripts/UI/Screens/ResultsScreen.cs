// VISUAL LAYOUT
// ┌─────────────────────────────┐
// │  Results                    │  ← title, pinned top (80px)
// ├─────────────────────────────┤
// │  Final Standings            │  ← section header
// │  1. Athlete Name            │  ← ResultRow per placement
// │  2. Athlete Name            │
// │  3. ...                     │
// │                             │
// │  Highlights                 │  ← section header (shown only if notable events exist)
// │  Someone dropped the egg!   │  ← up to 3 EggDrop / Surge / Stumble events,
// │  Someone surged ahead!      │    colour-coded (red / green / secondary)
// │  ...                        │
// ├─────────────────────────────┤
// │  [ Back to Team ]           │  ← full-width button, pinned bottom (80px)
// └─────────────────────────────┘
//
// GOAL: Show race outcome after the 3D replay.
// Reads GameSession.LatestResult — rebuilt on every Show() so it's always fresh.
// "Back to Team" returns the player to TeamHubScreen to run another event.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Legends.Data;
using Legends.Services;

namespace Legends.UI
{
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

            // Content area — fills between title and button, no scroll/mask needed for 4-6 athletes
            var bodyGo = UIFactory.CreateVerticalGroup(root.transform, "Body", spacing: 8f);
            Object.Destroy(bodyGo.GetComponent<UnityEngine.UI.ContentSizeFitter>());
            var bodyRt = bodyGo.GetComponent<RectTransform>();
            bodyRt.anchorMin = new Vector2(0, 0);
            bodyRt.anchorMax = new Vector2(1, 1);
            bodyRt.offsetMin = new Vector2(0, FooterH);
            bodyRt.offsetMax = new Vector2(0, -HeaderH);
            var body = bodyGo.transform;

            // Rankings
            CreateLabel(body, "RankingsHeader", "Final Standings", UIStyle.FontSection);
            for (int i = 0; i < result.Placements.Count; i++)
            {
                string name = nameMap.GetValueOrDefault(result.Placements[i], "Unknown");
                ResultRowFactory.Create(body, name, i + 1);
            }

            // Highlights
            var notable = result.Timeline
                .Where(e => e.EventType is EventTypes.EggDrop or EventTypes.Surge or EventTypes.Stumble)
                .Take(3)
                .ToList();

            if (notable.Count > 0)
            {
                CreateLabel(body, "EventsHeader", "Highlights", UIStyle.FontSection);
                foreach (var evt in notable)
                {
                    string evtName = nameMap.GetValueOrDefault(evt.AthleteId, "Someone");
                    string text    = FormatEvent(evt.EventType, evtName);
                    var lbl = CreateLabel(body, $"Evt_{evt.EventType}", text, UIStyle.FontBody);
                    lbl.color = EventColor(evt.EventType);
                }
            }

            return canvas;
        }

        static TextMeshProUGUI CreateLabel(Transform parent, string name, string text, float fontSize)
        {
            var panel   = UIFactory.CreatePanel(parent, name, Color.clear);
            var panelRt = panel.GetComponent<RectTransform>();
            panelRt.sizeDelta = new Vector2(0, fontSize * 1.5f);
            panel.AddComponent<UnityEngine.UI.LayoutElement>().minHeight = fontSize * 1.5f;

            var tmp   = UIFactory.CreateText(panel.transform, "Lbl", text, fontSize);
            var tmpRt = tmp.GetComponent<RectTransform>();
            UIFactory.FillParent(tmpRt);
            return tmp;
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
