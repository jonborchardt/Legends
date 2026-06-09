using System.IO;
using UnityEditor;
using UnityEngine;

namespace Legends.Editor
{
    // Populates Assets/Resources/Athletes/ with the character prefab and base material
    // so Resources.Load can find them in player builds without Addressables.
    // Called from LegendsBuildTools.BuildAll() on editor load, recompile, and before builds.
    public static class AthleteResourceBuilder
    {
        const string ResourcesFolder = "Assets/Resources/Athletes";
        const string MeshFbxPath     = "Assets/Synty/AnimationBaseLocomotion/Meshes/PolygonSyntyCharacter.fbx";
        const string MaterialSrcPath = "Assets/Synty/AnimationSwordCombat/Samples/Materials/M_Dummy.mat";
        const string PrefabDstPath   = "Assets/Resources/Athletes/AthleteCharacter.prefab";
        const string MaterialDstPath = "Assets/Resources/Athletes/M_Dummy.mat";

        public static void Build()
        {
            Directory.CreateDirectory(ResourcesFolder);
            AssetDatabase.Refresh();
            BuildMeshPrefab();
            BuildMaterial();
        }

        static void BuildMeshPrefab()
        {
            var fbx = AssetDatabase.LoadAssetAtPath<GameObject>(MeshFbxPath);
            if (fbx == null)
            {
                Debug.LogWarning($"[AthleteResourceBuilder] FBX not found: {MeshFbxPath}");
                return;
            }
            var instance = Object.Instantiate(fbx);
            PrefabUtility.SaveAsPrefabAsset(instance, PrefabDstPath);
            Object.DestroyImmediate(instance);
        }

        static void BuildMaterial()
        {
            if (AssetDatabase.LoadAssetAtPath<Material>(MaterialDstPath) != null)
                return;
            if (!AssetDatabase.CopyAsset(MaterialSrcPath, MaterialDstPath))
                Debug.LogWarning($"[AthleteResourceBuilder] Failed to copy material from {MaterialSrcPath}");
        }
    }
}
