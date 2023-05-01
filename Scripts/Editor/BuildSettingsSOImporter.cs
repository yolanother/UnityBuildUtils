using System.IO;
using UnityEditor;

namespace DoubTech.Builds
{
    public class BuildSettingsSOImporter : AssetPostprocessor
    {
        private void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
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
                        GenerateBuilderScript(buildSettings, assetPath);
                    }
                }
            }
        }

        private static void GenerateBuilderScript(BuildSettingsSO buildSettings, string assetPath)
        {
            string scriptName = "Build_" + buildSettings.name + ".cs";
            string scriptPath = Path.Combine("Assets/Editor", scriptName);
            
            if(!Directory.Exists("Assets/Editor"))
            {
                Directory.CreateDirectory("Assets/Editor");
            }

            using (StreamWriter writer = new StreamWriter(scriptPath, false))
            {
                writer.WriteLine("using UnityEditor;");
                writer.WriteLine("using UnityEngine;");
                writer.WriteLine();
                writer.WriteLine("public class Build_" + buildSettings.name);
                writer.WriteLine("{");
                writer.WriteLine("    [MenuItem(\"Build/Build " + buildSettings.name + "\")]");
                writer.WriteLine("    public static void Build()");
                writer.WriteLine("    {");
                writer.WriteLine(
                    "        BuildSettingsSO buildSettings = AssetDatabase.LoadAssetAtPath<BuildSettingsSO>(\"" +
                    assetPath + "\");");
                writer.WriteLine("        if (buildSettings != null)");
                writer.WriteLine("        {");
                writer.WriteLine("            CustomBuilder.BuildWindows(buildSettings);");
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