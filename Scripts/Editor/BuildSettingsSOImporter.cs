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
                writer.WriteLine("    [MenuItem(\"Build/Build " + buildSettings.name + "\")]");
                writer.WriteLine("    public static void Build()");
                writer.WriteLine("    {");
                writer.WriteLine($"        var assetPath = AssetDatabase.GUIDToAssetPath(\"{guid}\");");
                writer.WriteLine(
                    "        BuildSettingsSO buildSettings = AssetDatabase.LoadAssetAtPath<BuildSettingsSO>(assetPath);");
                writer.WriteLine("        if (buildSettings != null)");
                writer.WriteLine("        {");
                writer.WriteLine("            CustomBuilder.Build(buildSettings);");
                writer.WriteLine("        }");
                writer.WriteLine("        else");
                writer.WriteLine("        {");
                writer.WriteLine("            Debug.LogError(\"BuildSettingsSO not found.\");");
                writer.WriteLine("        }");
                writer.WriteLine("    }");
                writer.WriteLine("}");
            }

            AssetDatabase.Refresh();
        }
    }
}