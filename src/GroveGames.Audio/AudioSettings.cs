namespace GroveGames.Audio;

public sealed class AudioSettings : IAudioSettings
{
    private readonly float _musicFadeDuration;

    public float MusicFadeDuration => _musicFadeDuration;

    public AudioSettings(float musicFadeDuration)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(musicFadeDuration);

        _musicFadeDuration = musicFadeDuration;
    }
}
