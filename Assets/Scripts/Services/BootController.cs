using UnityEngine;

public class BootController : MonoBehaviour
{
    public static Camera UICamera { get; private set; }

    void Awake()
    {
        if (UICamera != null) return; // already created in a previous load

        var camGo = new GameObject("UICamera");
        Object.DontDestroyOnLoad(camGo);
        var cam = camGo.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
        cam.depth = 1; // always renders on top of any 3D scene camera
        UICamera = cam;
    }

    void Start() => Legends.Services.SceneFlow.GoToMainMenu();
}
