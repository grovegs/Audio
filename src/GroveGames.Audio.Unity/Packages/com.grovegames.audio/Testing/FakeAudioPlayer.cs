using GroveGames.Audio;
using GroveGames.Audio.Unity;
using UnityEngine;

namespace GroveGames.Audio.Unity.Testing
{
    public sealed class FakeAudioPlayer : IAudioPlayer<AudioClip>
    {
        private float _musicVolume;
        private float _sfxVolume;
        private AudioClip? _music;
        private IPlaylist<AudioClip>? _playlist;
        private AudioClip? _lastSfx;
        private float _lastSfxVolume;
        private int _sfxCount;
        private int _nextMusicCount;
        private int _stopMusicCount;

        public FakeAudioPlayer()
        {
            _musicVolume = -1f;
            _sfxVolume = -1f;
            _music = null;
            _playlist = null;
            _lastSfx = null;
            _lastSfxVolume = 0f;
            _sfxCount = 0;
            _nextMusicCount = 0;
            _stopMusicCount = 0;
        }

        public float MusicVolume => _musicVolume;
        public float SfxVolume => _sfxVolume;
        public AudioClip? Music => _music;
        public IPlaylist<AudioClip>? Playlist => _playlist;
        public AudioClip? LastSfx => _lastSfx;
        public float LastSfxVolume => _lastSfxVolume;
        public int SfxCount => _sfxCount;
        public int NextMusicCount => _nextMusicCount;
        public int StopMusicCount => _stopMusicCount;

        public void PlayMusic(AudioClip clip)
        {
            _music = clip;
        }

        public void PlayMusic(IPlaylist<AudioClip> playlist)
        {
            _playlist = playlist;
        }

        public void NextMusic()
        {
            _nextMusicCount++;
        }

        public void StopMusic()
        {
            _stopMusicCount++;
        }

        public void PlaySfx(AudioClip clip)
        {
            if (clip == null)
            {
                return;
            }

            _lastSfx = clip;
            _lastSfxVolume = 1f;
            _sfxCount++;
        }

        public void PlaySfx(AudioClip clip, float volume)
        {
            if (clip == null)
            {
                return;
            }

            _lastSfx = clip;
            _lastSfxVolume = volume;
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
