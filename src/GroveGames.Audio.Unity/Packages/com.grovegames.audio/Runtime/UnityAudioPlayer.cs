using System;
using GroveGames.Mathematics;
using GroveGames.Scheduling;
using GroveGames.Tween;
using UnityEngine;

namespace GroveGames.Audio.Unity
{
    public sealed class UnityAudioPlayer : IAudioPlayer<AudioClip>, IDisposable
    {
        private readonly AudioPlayer<AudioClip> _player;

        public float MusicVolume => _player.MusicVolume;
        public float SfxVolume => _player.SfxVolume;

        public UnityAudioPlayer(IAudioOutput<AudioClip> output, IAudioSettings settings, IScheduler scheduler, ITweener tweener, IRandomNumberGenerator random)
        {
            _player = new AudioPlayer<AudioClip>(output, settings, scheduler, tweener, random);
        }

        public void PlayMusic(AudioClip clip)
        {
            _player.PlayMusic(clip);
        }

        public void PlayMusic(IPlaylist<AudioClip> playlist)
        {
            _player.PlayMusic(playlist);
        }

        public void NextMusic()
        {
            _player.NextMusic();
        }

        public void StopMusic()
        {
            _player.StopMusic();
        }

        public void PlaySfx(AudioClip clip)
        {
            _player.PlaySfx(clip);
        }

        public void PlaySfx(AudioClip clip, float volume)
        {
            _player.PlaySfx(clip, volume);
        }

        public void SetMusicVolume(float volume)
        {
            _player.SetMusicVolume(volume);
        }

        public void SetSfxVolume(float volume)
        {
            _player.SetSfxVolume(volume);
        }

        public void Dispose()
        {
            _player.Dispose();
        }
    }
}
