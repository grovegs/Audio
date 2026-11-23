using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace GroveGames.Audio.Unity.Editor
{
    internal sealed class AudioSettingsBuildProcessor : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            AudioSettingsAsset.GetOrCreate();
        }
    }
}
