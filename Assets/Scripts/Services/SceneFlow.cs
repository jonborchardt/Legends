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
        public static Action HideAllScreensHandler;

        public static void GoToMainMenu() => GoToMainMenuHandler?.Invoke();

        public static void GoToTeamHub()
        {
            ExitReplay3D();
            GoToTeamHubHandler?.Invoke();
        }

        public static void GoToEventReplay()
        {
            HideAllScreensHandler?.Invoke();
            if (BootController.UICamera != null) BootController.UICamera.enabled = false;
            SceneManager.LoadScene("Replay3D", LoadSceneMode.Additive);
        }

        public static void GoToResults()
        {
            ExitReplay3D();
            GoToResultsHandler?.Invoke();
        }

        static void ExitReplay3D()
        {
            var scene = SceneManager.GetSceneByName("Replay3D");
            if (scene.isLoaded) SceneManager.UnloadSceneAsync(scene);
            if (BootController.UICamera != null) BootController.UICamera.enabled = true;
        }
    }
}
