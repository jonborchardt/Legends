using UnityEngine;
using Legends.Data;

namespace Legends.Replay
{
    public static class AthleteFactory
    {
        static RuntimeAnimatorController _controller;

        public static GameObject Spawn(AthleteState athlete, int laneIndex,
                                       Transform parent = null,
                                       AthleteVisualConfig config = null)
        {
            config ??= AthleteVisualConfig.Default();

            var go = new GameObject($"Athlete_{athlete.Name}");
            if (parent != null) go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(0f, 0f, laneIndex * config.LaneSpacing);

            var animator = go.AddComponent<Animator>();
            var ctl = LoadController();
            if (ctl != null)
                animator.runtimeAnimatorController = ctl;

            var meshGo = LoadMesh(go.transform, config)
                      ?? CreatePrimitiveMesh(go.transform);

            ConfigureAvatar(animator, meshGo, config);
            ApplyMaterial(meshGo, laneIndex, config);

            go.AddComponent<AthleteAnimator>();
            return go;
        }

        static void ConfigureAvatar(Animator animator, GameObject meshGo, AthleteVisualConfig config)
        {
#if UNITY_EDITOR
            var avatar = UnityEditor.AssetDatabase.LoadAssetAtPath<Avatar>(config.MeshEditorPath);
            if (avatar != null) animator.avatar = avatar;
#else
            var src = meshGo.GetComponent<Animator>()
                   ?? meshGo.GetComponentInChildren<Animator>();
            if (src != null)
            {
                animator.avatar = src.avatar;
                Object.Destroy(src);
            }
#endif
        }

        static void ApplyMaterial(GameObject meshGo, int laneIndex, AthleteVisualConfig config)
        {
            Material baseMat = null;
#if UNITY_EDITOR
            baseMat = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(config.MatEditorPath);
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

        static GameObject LoadMesh(Transform parent, AthleteVisualConfig config)
        {
            GameObject prefab = null;
#if UNITY_EDITOR
            prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(config.MeshEditorPath);
#endif
            prefab ??= Resources.Load<GameObject>(config.MeshRuntimePath);
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
