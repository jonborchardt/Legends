using UnityEngine.SceneManagement;

namespace Legends.Services
{
    public static class SceneFlow
    {
        public static void GoToMainMenu()    => SceneManager.LoadScene("MainMenu");
        public static void GoToTeamHub()     => SceneManager.LoadScene("TeamHub");
        public static void GoToEventReplay() => SceneManager.LoadScene("EventReplay");
        public static void GoToResults()     => SceneManager.LoadScene("Results");
    }
}
