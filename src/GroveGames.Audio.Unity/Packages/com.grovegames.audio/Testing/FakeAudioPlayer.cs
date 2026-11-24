using GroveGames.Audio;
using GroveGames.Audio.Unity;
using UnityEngine;

namespace GroveGames.Audio.Unity.Testing
{
    public sealed class FakeAudioPlayer : IAudioPlayer<AudioClip>
    {
        private float _musicVolume = -1f;
        private float _sfxVolume = -1f;
        private AudioClip? _music;
        private AudioClip? _lastSfx;
        private int _sfxCount;

        public float MusicVolume => _musicVolume;
        public float SfxVolume => _sfxVolume;
        public AudioClip? Music => _music;
        public AudioClip? LastSfx => _lastSfx;
        public int SfxCount => _sfxCount;

        public void PlayMusic(AudioClip clip)
        {
            _music = clip;
        }

        public void PlayMusic(IPlaylist<AudioClip> playlist)
        {
        }

        public void NextMusic()
        {
        }

        public void StopMusic()
        {
        }

        public void PlaySfx(AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }

            _lastSfx = clip;
            _sfxCount++;
        }

        public void PlaySfx(AudioClip clip, float volume)
        {
            if (clip == null)
            {
                return;
            }

            _lastSfx = clip;
            _sfxCount++;
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = volume;
        }

        public void SetSfxVolume(float volume)
        {
            _sfxVolume = volume;
        }
    }
}
