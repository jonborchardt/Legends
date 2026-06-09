using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using Legends.Replay;

namespace Legends.Editor
{
    // Programmatically registers all runtime assets in the Addressables default group
    // so that builds include them without any manual editor setup.
    // Called by LegendsBuildTools before every player build.
    public static class AddressablesSetup
    {
        static readonly (string path, string key)[] Entries =
        {
            ("Assets/Synty/AnimationBaseLocomotion/Meshes/PolygonSyntyCharacter.fbx", AddressKeys.AthleteCharacter),
            ("Assets/GeneratedArtifacts/Animators/Athlete.controller",                AddressKeys.AthleteController),
            ("Assets/Synty/AnimationSwordCombat/Samples/Materials/M_Dummy.mat",      AddressKeys.AthleteMaterial),
        };

        public static void Register()
        {
            var settings = AddressableAssetSettingsDefaultObject.GetSettings(true);
            var group    = settings.DefaultGroup;

            foreach (var (path, key) in Entries)
            {
                var guid = AssetDatabase.AssetPathToGUID(path);
                if (string.IsNullOrEmpty(guid))
                {
                    UnityEngine.Debug.LogWarning($"[AddressablesSetup] Asset not found at path: {path}");
                    continue;
                }
                var entry = settings.CreateOrMoveEntry(guid, group, readOnly: false, postEvent: false);
                entry.address = key;
            }

            EditorUtility.SetDirty(settings);
            AssetDatabase.SaveAssets();
        }

        public static void BuildContent()
        {
            AddressableAssetSettings.BuildPlayerContent();
        }
    }
}
