using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DoubTech.Builds
{
    [CreateAssetMenu(fileName = "BuildSettings", menuName = "Build Settings", order = 0)]
    public class BuildSettingsSO : ScriptableObject
    {
        public string binaryName;
        public string destinationPath = "Dist";
        public string buildPath = "Dist";
        public BuildTarget buildTarget = BuildTarget.StandaloneWindows64;
        public List<SceneAsset> scenes = new List<SceneAsset>();
        public int majorVersion = 0;
        public int minorVersion = 0;
        public int buildNumber = 1;
        public string lastBuild;
    }
}