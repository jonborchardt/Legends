using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using Legends.Data;
using Legends.Services;

namespace Legends.UI
{
    public class ResultsController : MonoBehaviour
    {
        [SerializeField] Transform  rankingsContainer;
        [SerializeField] GameObject rankingRowPrefab;
        [SerializeField] Transform  notableEventsContainer;
        [SerializeField] GameObject eventRowPrefab;

        Dictionary<string, string> _athleteNames;

        void Start()
        {
            var svc   = new PlayerPrefsSaveService();
            var state = svc.Load();

            if (state == null || state.CompletedEvents == null || state.CompletedEvents.Count == 0)
            {
                SceneFlow.GoToTeamHub();
                return;
            }

            var result = state.CompletedEvents[^1];
            _athleteNames = state.Team.Roster.ToDictionary(a => a.Id, a => a.Name);

            PopulateRankings(result);
            PopulateNotableEvents(result);
        }

        void PopulateRankings(EventResult result)
        {
            for (int i = 0; i < result.Placements.Count; i++)
            {
                string name  = _athleteNames.GetValueOrDefault(result.Placements[i], "Unknown");
                string label = $"{Ordinal(i + 1)} — {name}";

                var row = Instantiate(rankingRowPrefab, rankingsContainer);
                var tmp = row.GetComponentInChildren<TextMeshProUGUI>();
                tmp.text  = label;
                tmp.color = i == 0 ? UIStyle.Accent
                          : i <= 2 ? UIStyle.TextPrimary
                          :          UIStyle.TextSecondary;
            }
        }

        void PopulateNotableEvents(EventResult result)
        {
            var notable = result.Timeline
                .Where(e => e.EventType is EventTypes.EggDrop or EventTypes.Surge or EventTypes.Stumble)
                .Take(3)
                .ToList();

            foreach (var evt in notable)
            {
                var row = Instantiate(eventRowPrefab, notableEventsContainer);
                var tmp = row.GetComponentInChildren<TextMeshProUGUI>();
                tmp.text  = FormatEvent(evt);
                tmp.color = EventColor(evt.EventType);
            }
        }

        string FormatEvent(ReplayEvent evt)
        {
            string name = _athleteNames.GetValueOrDefault(evt.AthleteId, "Someone");
            return evt.EventType switch
            {
                EventTypes.EggDrop => $"{name} dropped the egg!",
                EventTypes.Surge   => $"{name} surged ahead!",
                EventTypes.Stumble => $"{name} stumbled!",
                _                  => evt.EventType
            };
        }

        Color EventColor(string eventType) => eventType switch
        {
            EventTypes.EggDrop => UIStyle.Danger,
            EventTypes.Surge   => UIStyle.Success,
            _                  => UIStyle.TextSecondary,
        };

        string Ordinal(int n) => n switch { 1 => "1st", 2 => "2nd", 3 => "3rd", _ => $"{n}th" };

        public void OnReturnClicked() => SceneFlow.GoToTeamHub();
    }
}
