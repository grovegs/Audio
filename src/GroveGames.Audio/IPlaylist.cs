namespace GroveGames.Audio;

public interface IPlaylist<TClip> where TClip : class
{
    public IReadOnlyList<TClip> Clips { get; }
}
