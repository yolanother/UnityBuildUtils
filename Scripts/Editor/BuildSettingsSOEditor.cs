using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DoubTech.Builds
{
    [CustomEditor(typeof(BuildSettingsSO))]
    public class BuildSettingsSOEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            BuildSettingsSO buildSettings = (BuildSettingsSO)target;

            EditorGUILayout.LabelField("Build Settings", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            EditorGUILayout.BeginHorizontal();
            buildSettings.destinationPath = EditorGUILayout.TextField("Destination Path", buildSettings.destinationPath);

            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string folderPath = EditorUtility.OpenFolderPanel("Select Destination Folder", buildSettings.destinationPath, "");
                if (!string.IsNullOrEmpty(folderPath))
                {
                    string projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
                    buildSettings.destinationPath = folderPath.Replace(projectPath, string.Empty);
                }
            }
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            buildSettings.buildPath = EditorGUILayout.TextField("Build Temp Path", buildSettings.buildPath);

            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string folderPath = EditorUtility.OpenFolderPanel("Select Destination Folder", buildSettings.buildPath, "");
                if (!string.IsNullOrEmpty(folderPath))
                {
                    string projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
                    buildSettings.buildPath = folderPath.Replace(projectPath, string.Empty);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;

            if (GUILayout.Button("Build"))
            {
                CustomBuilder.Build(buildSettings);
            }
        }
    }
}
