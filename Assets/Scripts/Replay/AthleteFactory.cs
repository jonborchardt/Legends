using System.Collections;
using UnityEngine;
using Legends.Data;

namespace Legends.Replay
{
    public static class AthleteFactory
    {
        static GameObject _cachedMeshPrefab;
        static RuntimeAnimatorController _cachedController;
        static Material _cachedMaterial;

        // In editor: AssetDatabase loads synchronously from source paths.
        // In builds: Resources.Load from Assets/Resources/Athletes/ (populated by AthleteResourceBuilder).
        public static IEnumerator PreloadAsync(AthleteVisualConfig config = null)
        {
            config ??= AthleteVisualConfig.Default();

#if UNITY_EDITOR
            _cachedMeshPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(config.MeshEditorPath);
            _cachedController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(config.ControllerEditorPath);
            _cachedMaterial   = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(config.MatEditorPath);
#else
            _cachedMeshPrefab = Resources.Load<GameObject>(ResourcePaths.AthleteCharacter);
            _cachedController = Resources.Load<RuntimeAnimatorController>(ResourcePaths.AthleteController);
            _cachedMaterial   = Resources.Load<Material>(ResourcePaths.AthleteMaterial);

            if (_cachedMeshPrefab == null)
                Debug.LogWarning($"[AthleteFactory] '{ResourcePaths.AthleteCharacter}' not found in Resources — capsule fallback will be used.");
            if (_cachedController == null)
                Debug.LogWarning($"[AthleteFactory] '{ResourcePaths.AthleteController}' not found in Resources — athletes will have no animations.");
            if (_cachedMaterial == null)
                Debug.LogWarning($"[AthleteFactory] '{ResourcePaths.AthleteMaterial}' not found in Resources — flat colour fallback will be used.");
#endif
            yield break;
        }

        public static void ReleaseHandles()
        {
            _cachedMeshPrefab = null;
            _cachedController = null;
            _cachedMaterial   = null;
        }

        public static GameObject Spawn(AthleteState athlete, int laneIndex,
                                       Transform parent = null,
                                       AthleteVisualConfig config = null)
        {
            config ??= AthleteVisualConfig.Default();

            var go = new GameObject($"Athlete_{athlete.Name}");
            if (parent != null) go.transform.SetParent(parent, false);
            go.transform.position = new Vector3(0f, 0f, laneIndex * config.LaneSpacing);

            var animator = go.AddComponent<Animator>();
            if (_cachedController != null)
                animator.runtimeAnimatorController = _cachedController;

            var meshGo = SpawnMesh(go.transform)
                      ?? CreatePrimitiveMesh(go.transform);

            ConfigureAvatar(animator, meshGo, config);
            ApplyMaterial(meshGo, laneIndex);

            go.AddComponent<AthleteAnimator>();
            return go;
        }

        static GameObject SpawnMesh(Transform parent)
        {
            if (_cachedMeshPrefab == null) return null;
            return Object.Instantiate(_cachedMeshPrefab, parent);
        }

        static GameObject CreatePrimitiveMesh(Transform parent)
        {
            var capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.transform.SetParent(parent, false);
            capsule.transform.localPosition = new Vector3(0f, 1f, 0f);
            Object.Destroy(capsule.GetComponent<CapsuleCollider>());
            return capsule;
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

        static void ApplyMaterial(GameObject meshGo, int laneIndex)
        {
            Material baseMat = _cachedMaterial;
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
    }
}
