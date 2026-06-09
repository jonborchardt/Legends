using System;
using System.Collections.Generic;
using UnityEngine;
using Legends.Services;

namespace Legends.UI
{
    public class ScreenRegistry : MonoBehaviour
    {
        static ScreenRegistry _instance;

        public static ScreenRegistry Instance
        {
            get
            {
                if (_instance == null)
                    Bootstrap();
                return _instance;
            }
        }

        // Auto-creates before any scene loads so SceneFlow delegates are ready
        // when BootController.Start() fires.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Bootstrap()
        {
            if (_instance != null) return;
            var go = new GameObject("ScreenRegistry");
            DontDestroyOnLoad(go);
            _instance = go.AddComponent<ScreenRegistry>();
            // Awake() fires immediately via AddComponent
        }

        readonly Dictionary<Type, UIScreen> _screens = new();

        public Transform CanvasParent => transform;

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            RegisterNavigationHandlers();
        }

        void RegisterNavigationHandlers()
        {
            SceneFlow.GoToMainMenuHandler  = () => ShowScreen<MainMenuScreen>();
            SceneFlow.GoToTeamHubHandler   = () => ShowScreen<TeamHubScreen>();
            SceneFlow.GoToResultsHandler   = () => ShowScreen<ResultsScreen>();
            SceneFlow.HideAllScreensHandler = HideAll;
        }

        public void ShowScreen<T>() where T : UIScreen, new()
        {
            HideAll();
            if (!_screens.TryGetValue(typeof(T), out var screen))
            {
                screen = new T();
                _screens[typeof(T)] = screen;
            }
            screen.Show();
        }

        public void HideAll()
        {
            foreach (var s in _screens.Values)
                s.Hide();
        }
    }
}
