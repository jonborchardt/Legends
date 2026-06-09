using UnityEngine;
using Legends.Data;

namespace Legends.Replay
{
    // Creates a 3D athlete GameObject entirely in code — no prefab required.
    public static class AthleteFactory
    {
        static RuntimeAnimatorController _controller;

        public static GameObject Spawn(AthleteState athlete, int laneIndex, Transform parent = null)
        {
            var go = new GameObject($"Athlete_{athlete.Name}");
            if (parent != null) go.transform.SetParent(parent, false);

            go.transform.position = new Vector3(0f, 0f, laneIndex * 1.2f);

            var animator = go.AddComponent<Animator>();
            var controller = LoadController();
            if (controller != null)
                animator.runtimeAnimatorController = controller;

            // Try Synty mesh; fall back to capsule primitive
            var meshGo = LoadSyntyMesh(go.transform)
                      ?? CreatePrimitiveMesh(go.transform);

            var renderer = meshGo.GetComponentInChildren<Renderer>();
            if (renderer != null)
                renderer.material = BuildMaterial(laneIndex);

            go.AddComponent<AthleteAnimator>();

            return go;
        }

        static RuntimeAnimatorController LoadController()
        {
            if (_controller != null) return _controller;
            _controller = Resources.Load<RuntimeAnimatorController>("Animators/Athlete");
            return _controller;
        }

        // Attempts to load a Synty SidekickCharacter mesh from Resources.
        // Returns null if the asset is not present.
        static GameObject LoadSyntyMesh(Transform parent)
        {
            var prefab = Resources.Load<GameObject>("Meshes/SK_HUMN_BASE_01");
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

        static Material BuildMaterial(int laneIndex)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit")
                      ?? Shader.Find("Standard");
            var mat   = new Material(shader);
            mat.color = Color.HSVToRGB((laneIndex * 0.22f) % 1f, 0.75f, 0.9f);
            return mat;
        }
    }
}
