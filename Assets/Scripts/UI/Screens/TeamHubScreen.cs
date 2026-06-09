using UnityEngine;
using TMPro;
using Legends.Data;
using Legends.Services;
using Legends.Simulation;

namespace Legends.UI
{
    public class TeamHubScreen : UIScreen
    {
        protected override GameObject Build()
        {
            var saveService = new PlayerPrefsSaveService();
            var gameState   = saveService.Load();
            if (gameState == null)
            {
                ScreenRegistry.Instance.ShowScreen<MainMenuScreen>();
                return new GameObject("TeamHub_Empty");
            }

            var canvas = UIFactory.CreateCanvas("TeamHub");
            var root   = UIFactory.CreatePanel(canvas.transform, "Root", UIStyle.Background);
            UIFactory.FillParent(root.GetComponent<RectTransform>());

            UIFactory.CreateText(root.transform, "Title", gameState.Team.TeamName, UIStyle.FontTitle);

            Transform scrollContent;
            var scroll = UIFactory.CreateScrollView(root.transform, "Scroll", out scrollContent);
            var scrollRt = scroll.GetComponent<RectTransform>();
            UIFactory.FillParent(scrollRt);

            foreach (var athlete in gameState.Team.Roster)
                AthleteCardFactory.Create(scrollContent, athlete);

            UIFactory.CreateButton(root.transform, "RunEvent", "Run Event", () => OnRunEvent(saveService, gameState));

            return canvas;
        }

        void OnRunEvent(ISaveService saveService, GameState gameState)
        {
            int seed   = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            var config = EventConfigFactory.CreateDragonEggRelay().ToData();
            var result = new DragonEggRelaySimulator().Simulate(config, gameState.Team.Roster, seed);

            gameState.CompletedEvents.Add(result);
            saveService.Save(gameState);
            GameSession.LatestResult = result;

            SceneFlow.GoToEventReplay();
        }
    }
}
