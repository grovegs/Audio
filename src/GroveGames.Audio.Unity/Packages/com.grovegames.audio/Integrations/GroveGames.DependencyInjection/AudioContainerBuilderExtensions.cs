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
            builder.AddSingleton<IAudioOutput<AudioClip>>(resolver => resolver.Instantiate(settings.OutputPrefab));
            builder.AddSingleton(settings);
            builder.AddSingleton<IAudioSettings>(settings);
            builder.AddSingleton<IAudioPlayer<AudioClip>, AudioPlayer<AudioClip>>();
            return builder.AddSingleton(resolver => new AudioListenerActivation(resolver.Instantiate(settings.ListenerPrefab)));
        }
    }
}
