using UnityEngine;

namespace GroveGames.Audio.Unity
{
    public sealed class UnityAudioSettings : ScriptableObject, IAudioSettings
    {
        public const string ConfigName = "com.grovegames.audio.settings";

        private static UnityAudioSettings s_instance;

        [SerializeField] private float _musicFadeDuration = 0.5f;

        [SerializeField, HideInInspector] private AudioListener _listenerPrefab;
        [SerializeField, HideInInspector] private UnityAudioOutput _outputPrefab;

        public float MusicFadeDuration => Mathf.Max(0f, _musicFadeDuration);
        public AudioListener ListenerPrefab => _listenerPrefab;
        public UnityAudioOutput OutputPrefab => _outputPrefab;

        public static UnityAudioSettings GetOrCreate()
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorBuildSettings.TryGetConfigObject(ConfigName, out UnityAudioSettings settings) && settings != null)
            {
                return settings;
            }
#else
            if (s_instance == null)
            {
                var preloaded = Resources.FindObjectsOfTypeAll<UnityAudioSettings>();
                s_instance = preloaded.Length > 0 ? preloaded[0] : null;
            }
#endif
            if (s_instance == null)
            {
                Debug.LogWarning("The audio settings asset was not found, so default audio settings are used.");
                s_instance = CreateInstance<UnityAudioSettings>();
                s_instance.hideFlags = HideFlags.DontSave;
            }

            return s_instance;
        }
    }
}
