using UnityEngine;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace AetherSurge.Editor
{
    public static class iOSBuildExporter
    {
        [MenuItem("Aether Surge/Export iOS Xcode Project")]
        public static void ExportXcodeProject()
        {
            Debug.Log("[iOSBuildExporter] Starting Xcode project export verification...");

            // 1. Configure iOS Player Settings
            PlayerSettings.companyName = "AetherSurge";
            PlayerSettings.productName = "Dragon-Ball-Adventure";
            PlayerSettings.SetApplicationIdentifier(UnityEditor.Build.NamedBuildTarget.iOS, "com.aethersurge.dragonballadventure");
            PlayerSettings.iOS.targetOSVersionString = "16.0";
            PlayerSettings.iOS.targetDevice = iOSTargetDevice.iPhoneAndiPad;
            PlayerSettings.iOS.requiresFullScreen = true;

            // 2. Configure Build Options
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/WhiteboxArena.unity" },
                locationPathName = "Builds/iOS",
                target = BuildTarget.iOS,
                options = BuildOptions.None
            };

            // 3. Execute Build
            BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            BuildSummary summary = report.summary;

            if (summary.result == BuildResult.Succeeded)
            {
                Debug.LogFormat("[iOSBuildExporter] Xcode project export SUCCEEDED! Total time: {0:F1}s, Output size: {1} MB",
                    summary.totalTime.TotalSeconds,
                    (summary.totalSize / (1024f * 1024f)).ToString("F2"));
            }
            else
            {
                Debug.LogErrorFormat("[iOSBuildExporter] Xcode project export FAILED with result: {0}, errors: {1}",
                    summary.result,
                    summary.totalErrors);
            }
        }
    }
}
