using UnityEngine;

namespace GroveGames.Audio.Unity
{
    public sealed class UnityAudioOutput : MonoBehaviour, IAudioOutput<AudioClip>
    {
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;

        public float MusicVolume => _musicSource.volume;
        public bool MusicLoop => _musicSource.loop;
        public bool IsMusicPlaying => _musicSource.isPlaying;
        public float SfxVolume => _sfxSource.volume;

        public float GetDuration(AudioClip clip)
        {
            return clip != null ? clip.length : 0f;
        }

        public void SetMusicVolume(float volume)
        {
            _musicSource.volume = volume;
        }

        public void SetMusicLoop(bool loop)
        {
            _musicSource.loop = loop;
        }

        public void SetSfxVolume(float volume)
        {
            _sfxSource.volume = volume;
        }

        public void PlayMusic(AudioClip clip, bool loop)
        {
            if (clip == null)
            {
                return;
            }

            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.Play();
        }

        public void StopMusic()
        {
            _musicSource.Stop();
        }

        public void PlaySfx(AudioClip clip, float volume)
        {
            if (clip == null)
            {
                return;
            }

            _sfxSource.PlayOneShot(clip, volume);
        }

        public void StopSfx()
        {
            _sfxSource.Stop();
        }
    }
}
