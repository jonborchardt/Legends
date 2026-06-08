using UnityEngine;
using TMPro;
using Legends.Data;
using Legends.Services;
using Legends.Simulation;

namespace Legends.UI
{
    public class TeamHubController : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI teamNameLabel;
        [SerializeField] Transform       rosterContainer;
        [SerializeField] GameObject      athleteCardPrefab;
        [SerializeField] EventConfig     eventConfig;

        ISaveService    _saveService;
        GameState       _gameState;
        IEventSimulator _simulator;

        void Start()
        {
            _saveService = new PlayerPrefsSaveService();
            _gameState   = _saveService.Load();
            _simulator   = new DragonEggRelaySimulator();

            if (_gameState == null)
            {
                SceneFlow.GoToMainMenu();
                return;
            }

            teamNameLabel.text = _gameState.Team.TeamName;

            foreach (var athlete in _gameState.Team.Roster)
            {
                var card = Instantiate(athleteCardPrefab, rosterContainer);
                card.GetComponent<AthleteCardUI>().Bind(athlete);
            }
        }

        public void OnEnterEventClicked()
        {
            int seed   = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            var result = _simulator.Simulate(eventConfig.ToData(), _gameState.Team.Roster, seed);

            _gameState.CompletedEvents.Add(result);
            _saveService.Save(_gameState);
            GameSession.LatestResult = result;

            SceneFlow.GoToEventReplay();
        }
    }
}
