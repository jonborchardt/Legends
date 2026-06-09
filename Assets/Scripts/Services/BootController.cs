using UnityEngine;

public class BootController : MonoBehaviour
{
    void Awake()
    {
        var camGo = new GameObject("MainCamera");
        camGo.tag = "MainCamera";
        Object.DontDestroyOnLoad(camGo);
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
    }

    void Start() => Legends.Services.SceneFlow.GoToMainMenu();
}
