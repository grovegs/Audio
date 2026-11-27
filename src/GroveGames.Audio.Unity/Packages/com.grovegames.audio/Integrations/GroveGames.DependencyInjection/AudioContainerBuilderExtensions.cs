using GroveGames.DependencyInjection;
using GroveGames.DependencyInjection.Unity;
using UnityEngine;

namespace GroveGames.Audio.Unity
{
    public static class AudioContainerBuilderExtensions
    {
        public static IContainerBuilder AddAudio(this IContainerBuilder builder)
        {
            var settings = UnityAudioSettings.GetOrCreate();
            builder.AddSingleton<IAudioOutput<AudioClip>>(resolver => CreateOutput(resolver, settings));
            builder.AddSingleton(settings);
            builder.AddSingleton<IAudioSettings>(settings);
            builder.AddSingleton<IAudioPlayer<AudioClip>, UnityAudioPlayer>();
            return builder.AddSingleton(resolver => new AudioListenerActivation(CreateListener(resolver, settings)));
        }

        private static UnityAudioOutput CreateOutput(IObjectResolver resolver, UnityAudioSettings settings)
        {
            if (settings.OutputPrefab != null)
            {
                return resolver.Instantiate(settings.OutputPrefab);
            }

            Debug.LogError("The audio settings have no output prefab, so a default audio output is created.");
            var output = resolver.Instantiate<UnityAudioOutput>();

            if (output == null)
            {
                return null;
            }

            output.SetSources(CreateSource(output.gameObject), CreateSource(output.gameObject));
            return output;
        }

        private static AudioListener CreateListener(IObjectResolver resolver, UnityAudioSettings settings)
        {
            if (settings.ListenerPrefab != null)
            {
                return resolver.Instantiate(settings.ListenerPrefab);
            }

            Debug.LogError("The audio settings have no listener prefab, so a default audio listener is created.");
            return resolver.Instantiate<AudioListener>();
        }

        private static AudioSource CreateSource(GameObject gameObject)
        {
            var source = gameObject.AddComponent<AudioSource>();
            source.playOnAwake = false;
            return source;
        }
    }
}
