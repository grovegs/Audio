namespace GroveGames.Audio.Tests;

public sealed class PlaylistTests
{
    [Fact]
    public void Constructor_NullClips_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Playlist<string>(null!));
    }

    [Fact]
    public void Constructor_Clips_ExposesThemInOrder()
    {
        var playlist = new Playlist<string>("a", "b", "c");

        Assert.Equal(["a", "b", "c"], playlist.Clips);
    }
}
