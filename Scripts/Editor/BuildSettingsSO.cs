using UnityEngine;

namespace DoubTech.Builds
{
    [CreateAssetMenu(fileName = "BuildSettings", menuName = "Build Settings", order = 0)]
    public class BuildSettingsSO : ScriptableObject
    {
        public string destinationPath = "Dist";
        public string buildPath = "Dist";
    }
}