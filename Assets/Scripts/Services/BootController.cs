using UnityEngine;
using UnityEngine.SceneManagement;

public class BootController : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
