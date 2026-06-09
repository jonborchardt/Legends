using UnityEngine;
using Legends.Data;

namespace Legends.Replay
{
    public static class AthleteFactory
    {
        const string CharFbxPath = "Assets/Synty/AnimationBaseLocomotion/Meshes/PolygonSyntyCharacter.fbx";
        const string MatPath     = "Assets/Synty/AnimationSwordCombat/Samples/Materials/M_Dummy.mat";

        static RuntimeAnimatorController _controller;

        public static GameObject Spawn(AthleteState athlete, int laneIndex, Transform parent = null)
        {
            var go = new GameObject($"Athlete_{athlete.Name}");
            if (parent != null) go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(0f, 0f, laneIndex * 1.2f);

            var animator = go.AddComponent<Animator>();
            var ctl = LoadController();
            if (ctl != null)
                animator.runtimeAnimatorController = ctl;

            var meshGo = LoadSyntyMesh(go.transform)
                      ?? CreatePrimitiveMesh(go.transform);

            ConfigureAvatar(animator, meshGo);
            ApplyMaterial(meshGo, laneIndex);

            go.AddComponent<AthleteAnimator>();
            return go;
        }

        // Assigns the Humanoid Avatar so retargeting works on the Animator sitting on the parent.
        static void ConfigureAvatar(Animator animator, GameObject meshGo)
        {
#if UNITY_EDITOR
            var avatar = UnityEditor.AssetDatabase.LoadAssetAtPath<Avatar>(CharFbxPath);
            if (avatar != null) animator.avatar = avatar;
#else
            // In builds the FBX is loaded from Resources; extract the Avatar from its Animator.
            var src = meshGo.GetComponent<Animator>()
                   ?? meshGo.GetComponentInChildren<Animator>();
            if (src != null)
            {
                animator.avatar = src.avatar;
                Object.Destroy(src);
            }
#endif
        }

        // Creates a per-lane tinted instance of the Synty Dummy material.
        static void ApplyMaterial(GameObject meshGo, int laneIndex)
        {
            Material baseMat = null;
#if UNITY_EDITOR
            baseMat = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(MatPath);
#endif
            if (baseMat == null)
            {
                var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
                baseMat = new Material(shader);
            }

            var mat  = new Material(baseMat);
            var tint = Color.HSVToRGB((laneIndex * 0.22f) % 1f, 0.60f, 0.95f);
            mat.SetColor("_BaseColor", tint);

            foreach (var r in meshGo.GetComponentsInChildren<Renderer>())
                r.sharedMaterial = mat;
        }

        static GameObject LoadSyntyMesh(Transform parent)
        {
            GameObject prefab = null;
#if UNITY_EDITOR
            prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(CharFbxPath);
#endif
            prefab ??= Resources.Load<GameObject>("Meshes/PolygonSyntyCharacter");
            if (prefab == null) return null;
            return Object.Instantiate(prefab, parent);
        }

        static GameObject CreatePrimitiveMesh(Transform parent)
        {
            var capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.transform.SetParent(parent, false);
            capsule.transform.localPosition = new Vector3(0f, 1f, 0f);
            Object.Destroy(capsule.GetComponent<CapsuleCollider>());
            return capsule;
        }

        static RuntimeAnimatorController LoadController()
        {
            if (_controller != null) return _controller;
            _controller = Resources.Load<RuntimeAnimatorController>("Animators/Athlete");
            return _controller;
        }
    }
}
