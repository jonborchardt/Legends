using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Legends.Editor
{
    // Generates Assets/Resources/Animators/Athlete.controller from code.
    // The .controller is a regeneratable artifact — never hand-edit it.
    // Run: Tools > Legends > Regenerate Athlete Controller
    public static class AthleteControllerGenerator
    {
        const string OutputPath = "Assets/Resources/Animators/Athlete.controller";

        // Synty AnimationBaseLocomotion + AnimationSwordCombat clip sources
        const string ClipIdle      = "Assets/Synty/AnimationBaseLocomotion/Animations/Polygon/Masculine/Idle/A_Idle_Standing_Masc.fbx";
        const string ClipRun       = "Assets/Synty/AnimationBaseLocomotion/Animations/Polygon/Masculine/Locomotion/Run/A_Run_F_Masc.fbx";
        const string ClipCarryRun  = "Assets/Synty/AnimationBaseLocomotion/Animations/Polygon/Masculine/Locomotion/Sprint/A_Sprint_F_Masc.fbx";
        const string ClipStumble   = "Assets/Synty/AnimationSwordCombat/Animations/Polygon/Hit/HitStagger/A_Hit_F_Stagger_Sword.fbx";
        const string ClipCelebrate = "Assets/Synty/AnimationSwordCombat/Animations/Polygon/Attack/HeavyFlourish01/A_Attack_HeavyFlourish01_Sword.fbx";
        const string ClipFail      = "Assets/Synty/AnimationSwordCombat/Animations/Polygon/Hit/KnockDown/A_KnockDown_Begin_Sword.fbx";

        [MenuItem("Tools/Legends/Regenerate Athlete Controller")]
        public static void Generate()
        {
            System.IO.Directory.CreateDirectory("Assets/Resources/Animators");

            var controller = AnimatorController.CreateAnimatorControllerAtPath(OutputPath);
            var root       = controller.layers[0].stateMachine;

            // States
            var idle      = root.AddState("Idle");
            var run       = root.AddState("Run");
            var carryRun  = root.AddState("CarryRun");
            var stumble   = root.AddState("Stumble");
            var celebrate = root.AddState("Celebrate");
            var fail      = root.AddState("Fail");

            root.defaultState = idle;

            // Trigger parameters (one per state — matches AthleteAnimator.StringToHash calls)
            controller.AddParameter("Idle",      AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Run",       AnimatorControllerParameterType.Trigger);
            controller.AddParameter("CarryRun",  AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Stumble",   AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Celebrate", AnimatorControllerParameterType.Trigger);
            controller.AddParameter("Fail",      AnimatorControllerParameterType.Trigger);

            // AnyState → each state via its trigger
            AddAnyTransition(root, idle,      "Idle");
            AddAnyTransition(root, run,       "Run");
            AddAnyTransition(root, carryRun,  "CarryRun");
            AddAnyTransition(root, stumble,   "Stumble");
            AddAnyTransition(root, celebrate, "Celebrate");
            AddAnyTransition(root, fail,      "Fail");

            // Assign animation clips from Synty packs
            AssignClip(idle,      ClipIdle);
            AssignClip(run,       ClipRun);
            AssignClip(carryRun,  ClipCarryRun);
            AssignClip(stumble,   ClipStumble);
            AssignClip(celebrate, ClipCelebrate);
            AssignClip(fail,      ClipFail);

            AssetDatabase.SaveAssets();
            Debug.Log($"[AthleteControllerGenerator] Generated at {OutputPath}");
        }

        static void AddAnyTransition(AnimatorStateMachine sm, AnimatorState to, string trigger)
        {
            var t = sm.AddAnyStateTransition(to);
            t.hasExitTime    = false;
            t.duration       = 0.1f;
            t.canTransitionToSelf = false;
            t.AddCondition(AnimatorConditionMode.If, 0f, trigger);
        }

        static void AssignClip(AnimatorState state, string fbxPath)
        {
            var clip = AssetDatabase.LoadAllAssetsAtPath(fbxPath)
                .OfType<AnimationClip>()
                .FirstOrDefault(c => !c.name.StartsWith("__preview__"));

            if (clip == null)
            {
                Debug.LogWarning($"[AthleteControllerGenerator] Clip not found at {fbxPath} — state '{state.name}' will have no motion.");
                return;
            }

            state.motion = clip;
        }
    }
}
