namespace GroveGames.Audio.Tests;

public sealed class AudioSettingsTests
{
    [Fact]
    public void Constructor_NegativeFadeDuration_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new AudioSettings(-0.1f));
    }

    [Fact]
    public void Constructor_ValidFadeDuration_ExposesIt()
    {
        var settings = new AudioSettings(0.5f);

        Assert.Equal(0.5f, settings.MusicFadeDuration);
    }
}
