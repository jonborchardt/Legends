namespace Legends.Replay
{
    // Plain C# config — no ScriptableObject, no drag-drop.
    // Pass to AthleteFactory.Spawn() or rely on the default.
    public class AthleteVisualConfig
    {
        // Editor (AssetDatabase) path to the character FBX.
        public string MeshEditorPath  { get; set; }

        // Resources-relative path used in WebGL builds.
        public string MeshRuntimePath { get; set; }

        // Editor path to the base material. Tinted per-lane at spawn time.
        public string MatEditorPath   { get; set; }

        // World-space gap between athlete lanes.
        public float  LaneSpacing     { get; set; }

        public static AthleteVisualConfig Default() => new()
        {
            MeshEditorPath  = "Assets/Synty/AnimationBaseLocomotion/Meshes/PolygonSyntyCharacter.fbx",
            MeshRuntimePath = "Meshes/PolygonSyntyCharacter",
            MatEditorPath   = "Assets/Synty/AnimationSwordCombat/Samples/Materials/M_Dummy.mat",
            LaneSpacing     = 1.2f,
        };
    }
}
