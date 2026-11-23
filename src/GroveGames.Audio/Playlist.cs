namespace GroveGames.Audio;

public sealed class Playlist<TClip> : IPlaylist<TClip> where TClip : class
{
    private readonly TClip[] _clips;

    public IReadOnlyList<TClip> Clips => _clips;

    public Playlist(params TClip[] clips)
    {
        ArgumentNullException.ThrowIfNull(clips);

        _clips = clips;
    }
}
