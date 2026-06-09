using System;
using UnityEngine.SceneManagement;

namespace Legends.Services
{
    // All scene/screen navigation goes through here.
    // UI navigation uses Action delegates registered by ScreenRegistry at startup.
    // SceneManager is only called for EventReplay (the sole separate scene).
    public static class SceneFlow
    {
        public static Action GoToMainMenuHandler;
        public static Action GoToTeamHubHandler;
        public static Action GoToResultsHandler;

        public static void GoToMainMenu()    => GoToMainMenuHandler?.Invoke();
        public static void GoToTeamHub()     => GoToTeamHubHandler?.Invoke();
        public static void GoToEventReplay() => SceneManager.LoadScene("Replay3D");

        public static void GoToResults()
        {
            if (SceneManager.GetSceneByName("Replay3D").isLoaded)
                SceneManager.UnloadSceneAsync("Replay3D");
            GoToResultsHandler?.Invoke();
        }
    }
}
