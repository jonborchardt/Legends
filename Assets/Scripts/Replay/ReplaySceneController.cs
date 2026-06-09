using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Legends.Data;
using Legends.Services;

namespace Legends.Replay
{
    // Entry point MonoBehaviour for the EventReplay scene.
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

        void OnReplayComplete() => BuildResultsButton();

        void BuildResultsButton()
        {
            var canvasGo = new GameObject("ReplayHUD");
            var canvas   = canvasGo.AddComponent<Canvas>();
            canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode =
                CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<GraphicRaycaster>();

            var btnGo = new GameObject("ViewResultsBtn");
            btnGo.transform.SetParent(canvasGo.transform, false);
            var img = btnGo.AddComponent<Image>();
            img.color = new Color(0.89f, 0.66f, 0.32f);
            var btn = btnGo.AddComponent<Button>();
            btn.targetGraphic = img;
            btn.onClick.AddListener(OnViewResults);

            var rt = btnGo.GetComponent<RectTransform>();
            rt.anchorMin        = new Vector2(0.5f, 0f);
            rt.anchorMax        = new Vector2(0.5f, 0f);
            rt.pivot            = new Vector2(0.5f, 0f);
            rt.anchoredPosition = new Vector2(0f, 60f);
            rt.sizeDelta        = new Vector2(320f, 72f);

            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(btnGo.transform, false);
            var tmp = labelGo.AddComponent<TextMeshProUGUI>();
            tmp.text      = "View Results";
            tmp.fontSize  = 24f;
            tmp.color     = Color.black;
            tmp.alignment = TextAlignmentOptions.Center;
            var lrt = labelGo.GetComponent<RectTransform>();
            lrt.anchorMin = Vector2.zero;
            lrt.anchorMax = Vector2.one;
            lrt.offsetMin = lrt.offsetMax = Vector2.zero;
        }

        static void OnViewResults() => SceneFlow.GoToResults();
    }
}
