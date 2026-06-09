using UnityEngine;
using Legends.Data;
using Legends.Services;
using Legends.Simulation;

namespace Legends.UI
{
    public class TeamHubScreen : UIScreen
    {
        const float HeaderH = 80f;
        const float FooterH = 80f;

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

            // Title — pinned to top
            var title   = UIFactory.CreateText(root.transform, "Title", gameState.Team.TeamName, UIStyle.FontTitle);
            var titleRt = title.GetComponent<RectTransform>();
            titleRt.anchorMin = new Vector2(0, 1);
            titleRt.anchorMax = new Vector2(1, 1);
            titleRt.offsetMin = new Vector2(16, -HeaderH);
            titleRt.offsetMax = new Vector2(-16, 0);

            // Scroll — fills space between header and footer
            Transform scrollContent;
            var scroll   = UIFactory.CreateScrollView(root.transform, "Scroll", out scrollContent);
            var scrollRt = scroll.GetComponent<RectTransform>();
            scrollRt.anchorMin = Vector2.zero;
            scrollRt.anchorMax = Vector2.one;
            scrollRt.offsetMin = new Vector2(0, FooterH);
            scrollRt.offsetMax = new Vector2(0, -HeaderH);

            foreach (var athlete in gameState.Team.Roster)
                AthleteCardFactory.Create(scrollContent, athlete);

            // Run Event button — pinned to bottom
            var btn   = UIFactory.CreateButton(root.transform, "RunEvent", "Run Event", () => OnRunEvent(saveService, gameState));
            var btnRt = btn.GetComponent<RectTransform>();
            btnRt.anchorMin = new Vector2(0, 0);
            btnRt.anchorMax = new Vector2(1, 0);
            btnRt.offsetMin = new Vector2(16, 8);
            btnRt.offsetMax = new Vector2(-16, FooterH - 8);

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
