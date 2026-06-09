using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Legends.Editor
{
    // Runs all code generators that produce regeneratable artifacts.
    // Triggers: script recompile (static ctor), every build (IPreprocessBuild),
    // and manual trigger (Tools > Legends > Build All).
    // To add a new generator: call it inside BuildAll().
    [InitializeOnLoad]
    public class LegendsBuildTools : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        // Runs on editor load and after every script recompile
        static LegendsBuildTools() => BuildAll();

        // Runs before every WebGL / standalone build
        public void OnPreprocessBuild(BuildReport _) => BuildAll();

        [MenuItem("Tools/Legends/Build All")]
        public static void BuildAll()
        {
            AthleteControllerGenerator.Generate();
            // Add future generators here, e.g.:
            // EnvironmentMeshGenerator.Generate();
        }
    }
}
