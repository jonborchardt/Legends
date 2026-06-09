using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Legends.Data;

namespace Legends.Replay
{
    public static class AthleteFactory
    {
        static GameObject _cachedMeshPrefab;
        static RuntimeAnimatorController _cachedController;
        static Material _cachedMaterial;

#if !UNITY_EDITOR
        static AsyncOperationHandle<GameObject> _meshHandle;
        static AsyncOperationHandle<RuntimeAnimatorController> _controllerHandle;
        static AsyncOperationHandle<Material> _materialHandle;
#endif

        // Preload both assets before calling Spawn. Call from ReplaySceneController via
        // yield return StartCoroutine(AthleteFactory.PreloadAsync()).
        // In editor: AssetDatabase paths load synchronously — no async work needed.
        // In builds: Addressables async load; WaitForCompletion is forbidden on WebGL.
        public static IEnumerator PreloadAsync(AthleteVisualConfig config = null)
        {
            config ??= AthleteVisualConfig.Default();

#if UNITY_EDITOR
            _cachedMeshPrefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(config.MeshEditorPath);
            _cachedController = UnityEditor.AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(config.ControllerEditorPath);
            _cachedMaterial   = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(config.MatEditorPath);
            yield break;
#else
            _meshHandle = Addressables.LoadAssetAsync<GameObject>(AddressKeys.AthleteCharacter);
            yield return _meshHandle;
            if (_meshHandle.Status == AsyncOperationStatus.Succeeded)
                _cachedMeshPrefab = _meshHandle.Result;
            else
                Debug.LogWarning($"[AthleteFactory] Failed to load mesh '{AddressKeys.AthleteCharacter}' — capsule fallback will be used.");

            _controllerHandle = Addressables.LoadAssetAsync<RuntimeAnimatorController>(AddressKeys.AthleteController);
            yield return _controllerHandle;
            if (_controllerHandle.Status == AsyncOperationStatus.Succeeded)
                _cachedController = _controllerHandle.Result;
            else
                Debug.LogWarning($"[AthleteFactory] Failed to load controller '{AddressKeys.AthleteController}' — athletes will have no animations.");

            _materialHandle = Addressables.LoadAssetAsync<Material>(AddressKeys.AthleteMaterial);
            yield return _materialHandle;
            if (_materialHandle.Status == AsyncOperationStatus.Succeeded)
                _cachedMaterial = _materialHandle.Result;
            else
                Debug.LogWarning($"[AthleteFactory] Failed to load material '{AddressKeys.AthleteMaterial}' — flat colour fallback will be used.");
#endif
        }

        // Release Addressables handles and clear cached assets.
        // Call from ReplaySceneController.OnDestroy so handles are not leaked across scenes.
        public static void ReleaseHandles()
        {
#if !UNITY_EDITOR
            if (_meshHandle.IsValid())       Addressables.Release(_meshHandle);
            if (_controllerHandle.IsValid()) Addressables.Release(_controllerHandle);
            if (_materialHandle.IsValid())   Addressables.Release(_materialHandle);
#endif
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
            ApplyMaterial(meshGo, laneIndex, config);

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

        static void ApplyMaterial(GameObject meshGo, int laneIndex, AthleteVisualConfig config)
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
