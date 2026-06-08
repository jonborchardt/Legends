using System.Collections.Generic;
using UnityEngine;
using Legends.Data;
using Legends.Services;

namespace Legends.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] GameObject continueButton;

        ISaveService _saveService;

        void Start()
        {
            _saveService = new PlayerPrefsSaveService();
            continueButton.SetActive(_saveService.Exists());
        }

        public void OnNewGameClicked()
        {
            var state = new GameState
            {
                SaveVersion     = "1.0",
                Team            = DefaultTeamFactory.Create(),
                CompletedEvents = new List<EventResult>()
            };
            _saveService.Save(state);
            SceneFlow.GoToTeamHub();
        }

        public void OnContinueClicked()
        {
            SceneFlow.GoToTeamHub();
        }
    }
}
