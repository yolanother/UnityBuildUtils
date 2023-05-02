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
            
            buildSettings.buildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build Target", buildSettings.buildTarget);
            
            buildSettings.binaryName = EditorGUILayout.TextField("Binary Name", buildSettings.binaryName);

            EditorGUILayout.BeginHorizontal();
            buildSettings.destinationPath = EditorGUILayout.TextField("Destination Path", buildSettings.destinationPath);

            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string folderPath = EditorUtility.OpenFolderPanel("Select Destination Folder", buildSettings.destinationPath, "");
                if (!string.IsNullOrEmpty(folderPath))
                {
                    string projectPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
                    buildSettings.destinationPath = folderPath.Replace(projectPath, string.Empty);
                    EditorUtility.SetDirty(buildSettings);
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
                    EditorUtility.SetDirty(buildSettings);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel--;
            var scenes = serializedObject.FindProperty("scenes");
            EditorGUILayout.PropertyField(scenes, new GUIContent("Scenes"), true);

            var majorVersion = EditorGUILayout.IntField("Major Version", buildSettings.majorVersion);
            if(majorVersion != buildSettings.majorVersion)
            {
                buildSettings.majorVersion = majorVersion;
                EditorUtility.SetDirty(buildSettings);
            }
            
            var minorVersion = EditorGUILayout.IntField("Minor Version", buildSettings.minorVersion);
            if(minorVersion != buildSettings.minorVersion)
            {
                buildSettings.minorVersion = minorVersion;
                EditorUtility.SetDirty(buildSettings);
            }
            
            var buildNumber = EditorGUILayout.IntField("Build Number", buildSettings.buildNumber);
            if(buildNumber != buildSettings.buildNumber)
            {
                buildSettings.buildNumber = buildNumber;
                EditorUtility.SetDirty(buildSettings);
            }
            
            GUILayout.Space(16);
            GUILayout.Label("Build", EditorStyles.boldLabel);

            if (GUILayout.Button("Build"))
            {
                CustomBuilder.Build(buildSettings);
            }
            
            if (GUILayout.Button("Build and Run"))
            {
                CustomBuilder.Build(buildSettings);
                CustomBuilder.Run(buildSettings);
            }
            
            GUILayout.Space(16);
            GUILayout.Label("Utilities", EditorStyles.boldLabel);

            if (GUILayout.Button("Create Build Menu"))
            {
                BuildSettingsSOImporter.GenerateBuilderScript(buildSettings);
            }
            
            // Apply properties
            serializedObject.ApplyModifiedProperties();
        }
    }
}
