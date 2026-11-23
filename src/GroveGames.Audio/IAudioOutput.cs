namespace GroveGames.Audio;

public interface IAudioOutput<TClip> where TClip : class
{
    public float MusicVolume { get; }
    public bool MusicLoop { get; }
    public bool IsMusicPlaying { get; }
    public float SfxVolume { get; }
    public float GetDuration(TClip clip);
    public void SetMusicVolume(float volume);
    public void SetMusicLoop(bool loop);
    public void SetSfxVolume(float volume);
    public void PlayMusic(TClip clip, bool loop);
    public void StopMusic();
    public void PlaySfx(TClip clip, float volume);
    public void StopSfx();
}
