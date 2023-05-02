using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DoubTech.Builds
{
    public class CustomBuilder
    {
        [MenuItem("Build/Install and Run APK")]
        public static void InstallAndRunApk()
        {
            string apkPath = EditorUtility.OpenFilePanel("Select APK", "", "apk");
            if (string.IsNullOrEmpty(apkPath))
            {
                Debug.LogError("No APK file selected.");
                return;
            }

            InstallApk(apkPath);
            RunApk();
        }

        private static void InstallApk(string apkPath)
        {
            string installCommand = $"adb install -r \"{apkPath}\"";
            System.Diagnostics.Process.Start("cmd.exe", $"/C {installCommand}");
            Debug.Log("Installing APK...");
        }

        private static void RunApk()
        {
            string packageName = GetPackageName();
            if (string.IsNullOrEmpty(packageName))
            {
                Debug.LogError("Cannot get package name from the project's package setup.");
                return;
            }

            string launchCommand = $"adb shell monkey -p {packageName} -c android.intent.category.LAUNCHER 1";
            System.Diagnostics.Process.Start("cmd.exe", $"/C {launchCommand}");
            Debug.Log("Running APK...");
        }

        private static string GetPackageName()
        {
            return PlayerSettings.GetApplicationIdentifier(BuildTargetGroup.Android);
        }
        public static void Build(BuildSettingsSO buildSettings)
        {
            // Set the build options
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions
            {
                locationPathName = Path.Combine(buildSettings.buildPath, buildSettings.binaryName),
                target = buildSettings.buildTarget,
                options = BuildOptions.None
            };

            buildPlayerOptions.locationPathName = buildPlayerOptions.locationPathName
                .Replace("{build}", buildSettings.buildNumber.ToString());
            
            // Set the scenes to be included in the build.
            List<EditorBuildSettingsScene> buildScenes = new List<EditorBuildSettingsScene>();
            foreach (SceneAsset sceneAsset in buildSettings.scenes)
            {
                string scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                buildScenes.Add(new EditorBuildSettingsScene(scenePath, true));
            }

            EditorBuildSettings.scenes = buildScenes.ToArray();

            string sourcePath = Path.Combine(Directory.GetCurrentDirectory(), buildSettings.buildPath);
            string destinationPath = Path.Combine(Directory.GetCurrentDirectory(), buildSettings.destinationPath);

            if (destinationPath != sourcePath && Directory.Exists(destinationPath))
            {
                var remove = EditorUtility.DisplayDialog("Replace Existing Build?",
                    "The destination path already exists. Do you want to remove it?", "Yes", "No");
                if (!remove) return;
            }
            
            // Build the project
            BuildPipeline.BuildPlayer(buildPlayerOptions);

            buildSettings.buildNumber++;
            EditorUtility.SetDirty(buildSettings);

            if (destinationPath != sourcePath)
            {
                if (Directory.Exists(destinationPath)) Directory.Delete(destinationPath, true);

                Directory.CreateDirectory(destinationPath);

                foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                {
                    Directory.CreateDirectory(dirPath.Replace(sourcePath, destinationPath));
                }

                foreach (string filePath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                {
                    File.Copy(filePath, filePath.Replace(sourcePath, destinationPath), true);
                }
            }

            Debug.Log($"Build completed and copied to {destinationPath}");
            EditorUtility.RevealInFinder(destinationPath);
            buildSettings.lastBuild = buildPlayerOptions.locationPathName;
            EditorUtility.SetDirty(buildSettings);
        }

        public static void Run(BuildSettingsSO buildSettings)
        {
            string destinationPath = Path.Combine(Directory.GetCurrentDirectory(), buildSettings.destinationPath);
            if (Directory.Exists(destinationPath))
            {
                // Find first exe in the destination path and run it.
                foreach (string filePath in Directory.GetFiles(destinationPath, "*.exe", SearchOption.AllDirectories))
                {
                    System.Diagnostics.Process.Start(filePath);
                    return;
                }
            }
            else
            {
                Debug.LogError($"Unable to find build at {destinationPath}");
            }
        }

        public static void BuildGUID(string guid)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            BuildSettingsSO buildSettings = AssetDatabase.LoadAssetAtPath<BuildSettingsSO>(assetPath);
            if (buildSettings != null)
            {
                CustomBuilder.Build(buildSettings);
            }
            else
            {
                Debug.LogError("BuildSettingsSO not found.");
            }
        }

        public static void BuildAndRunGUID(string guid)
        {
            BuildGUID(guid);
            RunGUID(guid);
        }

        public static void RunGUID(string guid)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            BuildSettingsSO buildSettings = AssetDatabase.LoadAssetAtPath<BuildSettingsSO>(assetPath);
            switch(buildSettings.buildTarget)
            {
                case BuildTarget.Android:
                    InstallApk(buildSettings.lastBuild);
                    RunApk();
                    break;
                default:
                    Run(buildSettings);
                    break;
            }
        }
    }
}