namespace GroveGames.Audio;

public interface IAudioPlayer<TClip> where TClip : class
{
    public float MusicVolume { get; }
    public float SfxVolume { get; }
    public void PlayMusic(TClip? clip);
    public void PlayMusic(IPlaylist<TClip>? playlist);
    public void NextMusic();
    public void StopMusic();
    public void PlaySfx(TClip? clip);
    public void PlaySfx(TClip? clip, float volume);
    public void SetMusicVolume(float volume);
    public void SetSfxVolume(float volume);
}
