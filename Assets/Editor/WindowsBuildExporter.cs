using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace AetherSurge.Editor
{
    public static class WindowsBuildExporter
    {
        [MenuItem("Aether Surge/Build Windows x64 Standalone")]
        public static void BuildWindowsExecutable()
        {
            Debug.Log("[WindowsBuildExporter] Starting Windows x64 build...");

            // 1. Configure Windows Player Settings
            PlayerSettings.companyName = "DragonBallAdventure";
            PlayerSettings.productName = "Dragon Ball Adventure";
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.defaultScreenWidth = 1280;
            PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.runInBackground = true;

            // 2. Re-bake Arena Scene to ensure font references are bound
            WhiteboxSceneBuilder.BuildArenaScene();

            // 3. Configure Build Options
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/WhiteboxArena.unity" },
                locationPathName = "Builds/Windows/DragonBallAdventure.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            // 4. Execute Build
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.LogFormat("[WindowsBuildExporter] Windows build SUCCEEDED! Total time: {0:F1}s, Output size: {1} MB, Path: {2}",
                    summary.totalTime.TotalSeconds,
                    (summary.totalSize / (1024f * 1024f)).ToString("F2"),
                    summary.outputPath);
            }
            else
            {
                Debug.LogErrorFormat("[WindowsBuildExporter] Windows build FAILED with result: {0}, errors: {1}",
                    summary.result,
                    summary.totalErrors);
            }
        }
    }
}
