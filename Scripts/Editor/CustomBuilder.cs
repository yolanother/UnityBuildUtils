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
                scenes = new[] { "Assets/Scenes/SampleScene.unity" },
                locationPathName = "Build/Windows",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };

            string sourcePath = Path.Combine(Directory.GetCurrentDirectory(), buildSettings.buildPath);
            string destinationPath = Path.Combine(Directory.GetCurrentDirectory(), buildSettings.destinationPath);

            if (Directory.Exists(destinationPath))
            {
                var remove = EditorUtility.DisplayDialog("Replace Existing Build?",
                    "The destination path already exists. Do you want to remove it?", "Yes", "No");
                if (!remove) return;
            }
            
            // Build the project
            BuildPipeline.BuildPlayer(buildPlayerOptions);
                
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

            Debug.Log($"Build completed and copied to {destinationPath}");
            EditorUtility.RevealInFinder(destinationPath);
        }
    }
}