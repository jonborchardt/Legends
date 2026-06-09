using System.Collections.Generic;
using UnityEngine;
using Legends.Data;
using Legends.Services;

namespace Legends.Replay
{
    // Entry point MonoBehaviour for the Replay3D scene.
    // PSC: zero [SerializeField] — all wiring done in Start().
    public class ReplaySceneController : MonoBehaviour
    {
        ReplayDirector _director;

        void Start()
        {
            var result = GetResult();
            if (result == null)
            {
                SceneFlow.GoToTeamHub();
                return;
            }

            var athletes = LoadAthletes();

            _director = GetComponent<ReplayDirector>()
                     ?? gameObject.AddComponent<ReplayDirector>();

            var cam = FindObjectOfType<ReplayCameraController>();
            if (cam != null) cam.Init(_director);

            _director.OnReplayComplete += OnReplayComplete;
            _director.Load(result, athletes);
        }

        static EventResult GetResult()
        {
            if (GameSession.LatestResult != null)
                return GameSession.LatestResult;

            var state = new PlayerPrefsSaveService().Load();
            return state?.CompletedEvents?.Count > 0
                ? state.CompletedEvents[^1]
                : null;
        }

        static List<AthleteState> LoadAthletes()
        {
            var state = new PlayerPrefsSaveService().Load();
            return state?.Team?.Roster ?? new List<AthleteState>();
        }

        static void OnReplayComplete() => SceneFlow.GoToResults();
    }
}
