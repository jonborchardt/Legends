using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Legends.Data;
using Legends.Services;

namespace Legends.UI
{
    public class MainMenuScreen : UIScreen
    {
        Button _continueButton;

        protected override GameObject Build()
        {
            var canvas = UIFactory.CreateCanvas("MainMenu");
            var root   = UIFactory.CreatePanel(canvas.transform, "Root", UIStyle.Background);
            UIFactory.FillParent(root.GetComponent<RectTransform>());

            var layout = UIFactory.CreateVerticalGroup(root.transform, "Layout", spacing: 32f);
            var layoutRt = layout.GetComponent<RectTransform>();
            layoutRt.anchorMin = new Vector2(0.3f, 0.2f);
            layoutRt.anchorMax = new Vector2(0.7f, 0.8f);
            layoutRt.offsetMin = layoutRt.offsetMax = Vector2.zero;
            Object.Destroy(layout.GetComponent<UnityEngine.UI.ContentSizeFitter>());

            UIFactory.CreateText(layout.transform, "Title", "Dragon Egg Relay", UIStyle.FontTitle);
            UIFactory.CreateText(layout.transform, "Subtitle", "Olympic Manager", UIStyle.FontSection);

            UIFactory.CreateButton(layout.transform, "NewGame", "New Game", OnNewGame);
            _continueButton = UIFactory.CreateButton(layout.transform, "Continue", "Continue", OnContinue);

            RefreshContinue();
            return canvas;
        }

        void RefreshContinue()
        {
            if (_continueButton == null) return;
            _continueButton.interactable = new PlayerPrefsSaveService().Exists();
        }

        void OnNewGame()
        {
            var state = new GameState
            {
                SaveVersion     = "1.0",
                Team            = DefaultTeamFactory.Create(),
                CompletedEvents = new List<EventResult>()
            };
            new PlayerPrefsSaveService().Save(state);
            ScreenRegistry.Instance.ShowScreen<TeamHubScreen>();
        }

        void OnContinue()
        {
            ScreenRegistry.Instance.ShowScreen<TeamHubScreen>();
        }
    }
}
