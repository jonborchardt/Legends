using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Legends.Editor
{
    public static class AthleteAnimatorControllerBuilder
    {
        const string ControllerPath = "Assets/ScriptableObjects/Athletes/AthleteAnimatorController.controller";
        const string ClipsPath      = "Assets/ScriptableObjects/Athletes/AnimClips";
        const string SyntyBase      = "Assets/Synty/AnimationBaseLocomotion/Animations/Polygon/Masculine";

        // Hardcoded Synty clip paths — locomotion pack only, so some states use nearest equivalent.
        // null → falls back to MakePlaceholderClip.
        static readonly (string state, bool loop, float duration, string fbxPath)[] StateDefs =
        {
            ("Idle",
                true,  2.0f,
                SyntyBase + "/Idle/A_Idle_Standing_Masc.fbx"),

            ("Run",          // surge: fastest available — no egg carried
                true,  0.4f,
                SyntyBase + "/Locomotion/Sprint/A_Sprint_F_Masc.fbx"),

            ("CarryRun",     // normal run pace — carrying egg
                true,  0.5f,
                SyntyBase + "/Locomotion/Run/A_Run_F_Masc.fbx"),

            ("Stumble",      // hard landing impact is the closest available
                false, 0.6f,
                SyntyBase + "/InAir/A_Land_IdleHard_Masc.fbx"),

            ("Celebrate",    // jump for joy — best available in a loco pack
                true,  1.0f,
                SyntyBase + "/InAir/A_Jump_Idle_Masc.fbx"),

            ("Fail",         // large fall — egg drop reaction
                false, 0.8f,
                SyntyBase + "/InAir/A_InAir_FallLarge_Masc.fbx"),
        };

        [MenuItem("Legends/Build Athlete Animator Controller")]
        public static void Build()
        {
            EnsureFolder("Assets/ScriptableObjects");
            EnsureFolder("Assets/ScriptableObjects/Athletes");
            EnsureFolder(ClipsPath);

            var controller = AnimatorController.CreateAnimatorControllerAtPath(ControllerPath);
            var sm = controller.layers[0].stateMachine;

            // All 6 triggers — names must match AthleteAnimator.cs StringToHash calls exactly
            foreach (var (name, _, _, _) in StateDefs)
                controller.AddParameter(name, AnimatorControllerParameterType.Trigger);

            AnimatorState idleState     = null;
            AnimatorState carryRunState = null;

            foreach (var (name, loop, duration, fbxPath) in StateDefs)
            {
                var clip  = LoadFbxClip(fbxPath) ?? MakePlaceholderClip(name, loop, duration);
                var state = sm.AddState(name);
                state.motion = clip;

                // Any State → this state on trigger, no exit time wait
                var t = sm.AddAnyStateTransition(state);
                t.AddCondition(AnimatorConditionMode.If, 0, name);
                t.hasExitTime         = false;
                t.duration            = 0.1f;
                t.canTransitionToSelf = false;

                if (name == "Idle")     idleState     = state;
                if (name == "CarryRun") carryRunState = state;
            }

            if (idleState != null)
                sm.defaultState = idleState;

            // One-shot states auto-return to CarryRun when their clip ends
            if (carryRunState != null)
            {
                AddExitReturn(sm, "Stumble", carryRunState);
                AddExitReturn(sm, "Fail",    carryRunState);
            }

            EditorUtility.SetDirty(controller);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[Legends] AthleteAnimatorController saved to {ControllerPath}");
        }

        // FBX files embed the clip as a sub-asset; LoadAssetAtPath<AnimationClip> returns it directly.
        static AnimationClip LoadFbxClip(string fbxPath)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(fbxPath);
            if (clip == null)
                Debug.LogWarning($"[Legends] Synty clip not found at '{fbxPath}' — using placeholder.");
            else
                Debug.Log($"[Legends] Loaded Synty clip: {fbxPath}");
            return clip;
        }

        static void AddExitReturn(AnimatorStateMachine sm, string fromName, AnimatorState to)
        {
            foreach (var cs in sm.states)
            {
                if (cs.state.name != fromName) continue;
                var t = cs.state.AddTransition(to);
                t.hasExitTime = true;
                t.exitTime    = 1.0f;
                t.duration    = 0.1f;
                return;
            }
        }

        static AnimationClip MakePlaceholderClip(string name, bool loop, float duration)
        {
            var clip = new AnimationClip { name = name };

            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = loop;
            AnimationUtility.SetAnimationClipSettings(clip, settings);

            float mid = duration * 0.5f;
            switch (name)
            {
                case "Run":
                    SetCurve(clip, "localPosition.y", K3(0, 0f, mid, 0.2f,  duration, 0f));
                    break;
                case "CarryRun":
                    SetCurve(clip, "localPosition.y", K3(0, 0f, mid, 0.15f, duration, 0f));
                    break;
                case "Stumble":
                    SetCurve(clip, "localPosition.x", K3(0, 0f, mid, 0.3f,  duration, 0f));
                    SetCurve(clip, "localPosition.y", K3(0, 0f, mid, -0.2f, duration, 0f));
                    break;
                case "Celebrate":
                    SetCurve(clip, "localScale.y",    K3(0, 1f, mid, 1.3f,  duration, 1f));
                    break;
                case "Fail":
                    SetCurve(clip, "localPosition.y", K3(0, 0f, mid, -0.5f, duration, 0f));
                    break;
                // Idle: no curves — static pose
            }

            AssetDatabase.CreateAsset(clip, $"{ClipsPath}/{name}.anim");
            return clip;
        }

        static void SetCurve(AnimationClip clip, string property, Keyframe[] keys)
            => clip.SetCurve("", typeof(Transform), property, new AnimationCurve(keys));

        static Keyframe[] K3(float t0, float v0, float t1, float v1, float t2, float v2)
            => new[] { new Keyframe(t0, v0), new Keyframe(t1, v1), new Keyframe(t2, v2) };

        static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;
            var parts  = path.Split('/');
            var parent = string.Join("/", parts[..^1]);
            EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, parts[^1]);
        }
    }
}
