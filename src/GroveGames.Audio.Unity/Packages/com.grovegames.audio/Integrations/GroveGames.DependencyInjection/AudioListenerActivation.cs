using GroveGames.DependencyInjection;

using UnityEngine;

namespace GroveGames.Audio.Unity
{
    internal sealed class AudioListenerActivation : IInitializable
    {
        private readonly AudioListener _audioListener;

        public AudioListenerActivation(AudioListener audioListener)
        {
            _audioListener = audioListener;
        }

        public void Initialize()
        {
            _audioListener.enabled = true;
        }
    }
}
