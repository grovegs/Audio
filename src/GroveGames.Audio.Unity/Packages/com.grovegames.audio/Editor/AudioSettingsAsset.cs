using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GroveGames.Audio.Unity.Editor
{
    internal static class AudioSettingsAsset
    {
        public const string AssetPath = "Assets/Settings/GroveGames/AudioSettings.asset";

        private const string LegacyAssetPath = "Assets/Settings/Resources/GroveGames/AudioSettings.asset";
        private const string ListenerPrefabPath = "Packages/com.grovegames.audio/Runtime/Prefabs/AudioListener.prefab";
        private const string OutputPrefabPath = "Packages/com.grovegames.audio/Runtime/Prefabs/AudioOutput.prefab";

        [InitializeOnLoadMethod]
        private static void MigrateLegacyAsset()
        {
            if (File.Exists(LegacyAssetPath) && !EditorBuildSettings.TryGetConfigObject(UnityAudioSettings.ConfigName, out UnityAudioSettings _))
            {
                EditorApplication.delayCall += () => GetOrCreate();
            }
        }

        public static UnityAudioSettings GetOrCreate()
        {
            if (!EditorBuildSettings.TryGetConfigObject(UnityAudioSettings.ConfigName, out UnityAudioSettings settings) || settings == null)
            {
                settings = LoadOrCreate();
                EditorBuildSettings.AddConfigObject(UnityAudioSettings.ConfigName, settings, true);
            }

            AssignPrefabs(settings);
            Preload(settings);
            return settings;
        }

        private static void AssignPrefabs(UnityAudioSettings settings)
        {
            var listenerPrefab = AssetDatabase.LoadAssetAtPath<AudioListener>(ListenerPrefabPath);
            var outputPrefab = AssetDatabase.LoadAssetAtPath<UnityAudioOutput>(OutputPrefabPath);

            if (settings.ListenerPrefab == listenerPrefab && settings.OutputPrefab == outputPrefab)
            {
                return;
            }

            var serialized = new SerializedObject(settings);
            serialized.FindProperty("_listenerPrefab").objectReferenceValue = listenerPrefab;
            serialized.FindProperty("_outputPrefab").objectReferenceValue = outputPrefab;
            serialized.ApplyModifiedPropertiesWithoutUndo();
            AssetDatabase.SaveAssetIfDirty(settings);
        }

        private static UnityAudioSettings LoadOrCreate()
        {
            CreateFolder();

            if (AssetDatabase.LoadAssetAtPath<UnityAudioSettings>(AssetPath) == null && AssetDatabase.LoadAssetAtPath<UnityAudioSettings>(LegacyAssetPath) != null)
            {
                AssetDatabase.MoveAsset(LegacyAssetPath, AssetPath);
            }

            var settings = AssetDatabase.LoadAssetAtPath<UnityAudioSettings>(AssetPath);

            if (settings != null)
            {
                return settings;
            }

            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<UnityAudioSettings>(), AssetPath);
            AssetDatabase.SaveAssets();
            return AssetDatabase.LoadAssetAtPath<UnityAudioSettings>(AssetPath);
        }

        private static void Preload(UnityAudioSettings settings)
        {
            var preloaded = PlayerSettings.GetPreloadedAssets();
            var assets = new List<Object>(preloaded.Length + 1);
            var isPreloaded = false;

            for (var i = 0; i < preloaded.Length; i++)
            {
                var asset = preloaded[i];

                if (asset is UnityAudioSettings && asset != settings)
                {
                    continue;
                }

                isPreloaded |= asset == settings;
                assets.Add(asset);
            }

            if (!isPreloaded)
            {
                assets.Add(settings);
            }

            if (isPreloaded && assets.Count == preloaded.Length)
            {
                return;
            }

            PlayerSettings.SetPreloadedAssets(assets.ToArray());
        }

        private static void CreateFolder()
        {
            var current = "Assets";

            foreach (var part in AssetPath.Substring("Assets/".Length).Split('/'))
            {
                if (part.EndsWith(".asset", System.StringComparison.Ordinal))
                {
                    return;
                }

                var next = current + "/" + part;

                if (!AssetDatabase.IsValidFolder(next))
                {
                    AssetDatabase.CreateFolder(current, part);
                }

                current = next;
            }
        }
    }
}
