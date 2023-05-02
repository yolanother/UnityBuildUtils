using System.IO;
using UnityEditor;

namespace DoubTech.Builds
{
    public class BuildSettingsSOImporter : AssetPostprocessor
    {
        public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string assetPath in importedAssets)
            {
                if (assetPath.EndsWith(".asset") &&
                    Path.GetFileNameWithoutExtension(assetPath).StartsWith("BuildSettings"))
                {
                    BuildSettingsSO buildSettings = AssetDatabase.LoadAssetAtPath<BuildSettingsSO>(assetPath);
                    if (buildSettings != null)
                    {
                        GenerateBuilderScript(buildSettings);
                    }
                }
            }
        }

        public static void GenerateBuilderScript(BuildSettingsSO buildSettings)
        {
            // Get Asset Path from buildSettings
            string assetPath = AssetDatabase.GetAssetPath(buildSettings);
            string guid = AssetDatabase.GUIDFromAssetPath(assetPath).ToString();

            string scriptName = "Build_" + buildSettings.name + ".cs";
            string scriptPath = Path.Combine("Assets/Editor", scriptName);
            
            if(!Directory.Exists("Assets/Editor"))
            {
                Directory.CreateDirectory("Assets/Editor");
            }

            if(File.Exists(scriptPath)) return;

            using (StreamWriter writer = new StreamWriter(scriptPath, false))
            {
                writer.WriteLine("using UnityEditor;");
                writer.WriteLine("using UnityEngine;");
                writer.WriteLine("using DoubTech.Builds;");
                writer.WriteLine();
                writer.WriteLine("public class Build_" + buildSettings.name);
                writer.WriteLine("{");
                writer.WriteLine("    [MenuItem(\"Build/" + buildSettings.name + "/Build\", false, 1)]");
                writer.WriteLine("    public static void Build()");
                writer.WriteLine("    {");
                writer.WriteLine($"        CustomBuilder.BuildGUID(\"{guid}\");");
                writer.WriteLine("    }");
                writer.WriteLine("}");
                writer.WriteLine();
                writer.WriteLine("public class Build_" + buildSettings.name);
                writer.WriteLine("{");
                writer.WriteLine("    [MenuItem(\"Build/" + buildSettings.name + "/Build and Run\", false, 1)]");
                writer.WriteLine("    public static void BuildAndRun()");
                writer.WriteLine("    {");
                writer.WriteLine($"        CustomBuilder.BuildAndRunGUID(\"{guid}\");");
                writer.WriteLine("    }");
                writer.WriteLine("}");
                writer.WriteLine();
                writer.WriteLine("public class Build_" + buildSettings.name);
                writer.WriteLine("{");
                writer.WriteLine("    [MenuItem(\"Build/" + buildSettings.name + "/Build and Run\", false, 2)]");
                writer.WriteLine("    public static void Run()");
                writer.WriteLine("    {");
                writer.WriteLine($"        CustomBuilder.RunGUID(\"{guid}\");");
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            AssetDatabase.Refresh();
        }
    }
}