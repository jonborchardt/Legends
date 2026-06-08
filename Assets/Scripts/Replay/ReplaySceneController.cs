using UnityEngine;
using Legends.Data;
using Legends.Services;

namespace Legends.Replay
{
    public class ReplaySceneController : MonoBehaviour
    {
        [SerializeField] ReplayDirector director;
        [SerializeField] GameObject viewResultsButton;

        void Start()
        {
            var result = GetResult();
            if (result == null)
            {
                SceneFlow.GoToTeamHub();
                return;
            }

            director.OnReplayComplete += ShowResultsButton;
            director.Load(result);
        }

        EventResult GetResult()
        {
            if (GameSession.LatestResult != null)
                return GameSession.LatestResult;

            var state = new PlayerPrefsSaveService().Load();
            return state?.CompletedEvents?.Count > 0
                ? state.CompletedEvents[^1]
                : null;
        }

        void ShowResultsButton() => viewResultsButton.SetActive(true);

        public void OnViewResultsClicked() => SceneFlow.GoToResults();
    }
}
