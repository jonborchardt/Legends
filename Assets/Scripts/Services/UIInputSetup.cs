using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Legends.Services
{
    public class UIInputSetup : MonoBehaviour
    {
        void Awake()
        {
            var module = GetComponent<InputSystemUIInputModule>();
            if (module == null) return;

            // Use the project-level action asset configured in Project Settings > Input System
            var actions = InputSystem.actions;
            if (actions == null) return;

            var ui = actions.FindActionMap("UI");
            if (ui == null) return;

            module.actionsAsset             = actions;
            module.point                    = Ref(ui, "Point");
            module.leftClick                = Ref(ui, "Click");
            module.rightClick               = Ref(ui, "RightClick");
            module.middleClick              = Ref(ui, "MiddleClick");
            module.scrollWheel              = Ref(ui, "ScrollWheel");
            module.move                     = Ref(ui, "Navigate");
            module.submit                   = Ref(ui, "Submit");
            module.cancel                   = Ref(ui, "Cancel");
            module.trackedDevicePosition    = Ref(ui, "TrackedDevicePosition");
            module.trackedDeviceOrientation = Ref(ui, "TrackedDeviceOrientation");
        }

        static InputActionReference Ref(InputActionMap map, string actionName)
        {
            var action = map.FindAction(actionName);
            return action != null ? InputActionReference.Create(action) : null;
        }
    }
}
