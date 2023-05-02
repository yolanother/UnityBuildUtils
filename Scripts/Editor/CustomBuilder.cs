using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace DoubTech.Builds
{
    public class CustomBuilder
    {
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
    }
}