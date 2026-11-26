using GroveGames.Mathematics;
using GroveGames.Scheduling;
using GroveGames.Tween;

namespace GroveGames.Audio.Tests;

public sealed class AudioPlayerTests
{
    private const float FadeDuration = 0.5f;
    private const float ClipDuration = 10f;
    private const float Step = 0.1f;

    private sealed class TestClip
    {
        private readonly float _duration;

        public float Duration => _duration;

        public TestClip(float duration)
        {
            _duration = duration;
        }
    }

    private sealed class FakeAudioOutput : IAudioOutput<TestClip>
    {
        private readonly List<TestClip> _playedSfx;
        private readonly List<float> _sfxVolumes;
        private float _musicVolume;
        private float _sfxVolume;
        private bool _musicLoop;
        private TestClip? _music;
        private int _playMusicCallCount;
        private int _stopSfxCallCount;

        public float MusicVolume => _musicVolume;
        public bool MusicLoop => _musicLoop;
        public bool IsMusicPlaying => _music != null;
        public float SfxVolume => _sfxVolume;
        public TestClip? Music => _music;
        public int PlayMusicCallCount => _playMusicCallCount;
        public int StopSfxCallCount => _stopSfxCallCount;
        public IReadOnlyList<TestClip> PlayedSfx => _playedSfx;
        public IReadOnlyList<float> SfxVolumes => _sfxVolumes;

        public FakeAudioOutput()
        {
            _playedSfx = [];
            _sfxVolumes = [];
        }

        public float GetDuration(TestClip clip)
        {
            return clip.Duration;
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = volume;
        }

        public void SetMusicLoop(bool loop)
        {
            _musicLoop = loop;
        }

        public void SetSfxVolume(float volume)
        {
            _sfxVolume = volume;
        }

        public void PlayMusic(TestClip clip, bool loop)
        {
            _music = clip;
            _musicLoop = loop;
            _playMusicCallCount++;
        }

        public void StopMusic()
        {
            _music = null;
        }

        public void PlaySfx(TestClip clip, float volume)
        {
            _playedSfx.Add(clip);
            _sfxVolumes.Add(volume);
        }

        public void StopSfx()
        {
            _stopSfxCallCount++;
        }
    }

    private sealed class FakePlaylist : IPlaylist<TestClip>
    {
        private readonly List<TestClip> _clips;

        public IReadOnlyList<TestClip> Clips => _clips;

        public FakePlaylist(params TestClip[] clips)
        {
            _clips = [.. clips];
        }

        public void Clear()
        {
            _clips.Clear();
        }

        public void KeepFirst()
        {
            _clips.RemoveRange(1, _clips.Count - 1);
        }
    }

    private readonly FakeAudioOutput _output;
    private readonly Scheduler _scheduler;
    private readonly Tweener _tweener;
    private readonly AudioPlayer<TestClip> _player;

    public AudioPlayerTests()
    {
        _output = new FakeAudioOutput();
        _scheduler = new Scheduler();
        _tweener = new Tweener(static exception => throw exception);
        _player = new AudioPlayer<TestClip>(_output, new AudioSettings(FadeDuration), _scheduler, _tweener, new RandomNumberGenerator(1));
    }

    private void Advance(float seconds)
    {
        var steps = (int)MathF.Round(seconds / Step);

        for (var i = 0; i < steps; i++)
        {
            _scheduler.Update(Step, Step, Step);
            _tweener.Update(Step, Step, Step);
        }
    }

    [Fact]
    public void Constructor_NullOutput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new AudioPlayer<TestClip>(null!, new AudioSettings(FadeDuration), _scheduler, _tweener, new RandomNumberGenerator(1)));
    }

    [Fact]
    public void Constructor_NullSettings_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new AudioPlayer<TestClip>(_output, null!, _scheduler, _tweener, new RandomNumberGenerator(1)));
    }

    [Fact]
    public void Constructor_Always_StartsAtFullVolume()
    {
        Assert.Equal(1f, _player.MusicVolume);
        Assert.Equal(1f, _player.SfxVolume);
        Assert.Equal(1f, _output.MusicVolume);
        Assert.Equal(1f, _output.SfxVolume);
    }

    [Fact]
    public void PlayMusic_Clip_StartsItLoopingFromSilence()
    {
        var clip = new TestClip(ClipDuration);

        _player.PlayMusic(clip);

        Assert.Same(clip, _output.Music);
        Assert.True(_output.MusicLoop);
        Assert.Equal(0f, _output.MusicVolume);
    }

    [Fact]
    public void PlayMusic_Clip_FadesInToMusicVolume()
    {
        _player.SetMusicVolume(0.6f);
        _player.PlayMusic(new TestClip(ClipDuration));

        Advance(FadeDuration + Step);

        Assert.Equal(0.6f, _output.MusicVolume, 3);
    }

    [Fact]
    public void PlayMusic_SameClip_DoesNotRestartIt()
    {
        var clip = new TestClip(ClipDuration);
        _player.PlayMusic(clip);

        _player.PlayMusic(clip);

        Assert.Equal(1, _output.PlayMusicCallCount);
    }

    [Fact]
    public void PlayMusic_OtherClipWhilePlaying_FadesOutThenStartsIt()
    {
        var first = new TestClip(ClipDuration);
        var second = new TestClip(ClipDuration);
        _player.PlayMusic(first);
        Advance(FadeDuration + Step);

        _player.PlayMusic(second);

        Assert.Same(first, _output.Music);
        Advance(FadeDuration + Step);
        Assert.Same(second, _output.Music);
    }

    [Fact]
    public void PlayMusic_NullClip_StopsTheMusic()
    {
        _player.PlayMusic(new TestClip(ClipDuration));
        Advance(FadeDuration + Step);

        _player.PlayMusic((TestClip?)null);
        Advance(FadeDuration + Step);

        Assert.False(_output.IsMusicPlaying);
    }

    [Fact]
    public void PlayMusic_Playlist_StartsOneOfItsClipsWithoutLoop()
    {
        var playlist = new Playlist<TestClip>(new TestClip(ClipDuration), new TestClip(ClipDuration), new TestClip(ClipDuration));

        _player.PlayMusic(playlist);

        Assert.Contains(_output.Music, playlist.Clips);
        Assert.False(_output.MusicLoop);
    }

    [Fact]
    public void PlayMusic_SamePlaylist_DoesNotRestartIt()
    {
        var playlist = new Playlist<TestClip>(new TestClip(ClipDuration), new TestClip(ClipDuration));
        _player.PlayMusic(playlist);

        _player.PlayMusic(playlist);

        Assert.Equal(1, _output.PlayMusicCallCount);
    }

    [Fact]
    public void PlayMusic_EmptyPlaylist_DoesNothing()
    {
        _player.PlayMusic(new Playlist<TestClip>());

        Assert.False(_output.IsMusicPlaying);
    }

    [Fact]
    public void PlayMusic_NullPlaylist_DoesNothing()
    {
        _player.PlayMusic((IPlaylist<TestClip>?)null);

        Assert.False(_output.IsMusicPlaying);
    }

    [Fact]
    public void PlayMusic_Playlist_AdvancesToAnotherClipBeforeTheCurrentOneEnds()
    {
        var playlist = new Playlist<TestClip>(new TestClip(ClipDuration), new TestClip(ClipDuration), new TestClip(ClipDuration));
        _player.PlayMusic(playlist);
        var first = _output.Music;

        Advance(ClipDuration + Step);

        Assert.NotNull(_output.Music);
        Assert.NotSame(first, _output.Music);
        Assert.Contains(_output.Music, playlist.Clips);
    }

    [Fact]
    public void NextMusic_WithPlaylist_NeverRepeatsTheCurrentClip()
    {
        var playlist = new Playlist<TestClip>(new TestClip(ClipDuration), new TestClip(ClipDuration), new TestClip(ClipDuration));
        _player.PlayMusic(playlist);
        Advance(FadeDuration + Step);

        for (var i = 0; i < 20; i++)
        {
            var previous = _output.Music;

            _player.NextMusic();
            Advance(FadeDuration + Step);

            Assert.NotSame(previous, _output.Music);
        }
    }

    [Fact]
    public void NextMusic_WithoutPlaylist_DoesNothing()
    {
        var clip = new TestClip(ClipDuration);
        _player.PlayMusic(clip);

        _player.NextMusic();
        Advance(FadeDuration + Step);

        Assert.Same(clip, _output.Music);
        Assert.Equal(1, _output.PlayMusicCallCount);
    }

    [Fact]
    public void StopMusic_WhilePlaylistPlays_StopsAndDoesNotAdvance()
    {
        _player.PlayMusic(new Playlist<TestClip>(new TestClip(ClipDuration), new TestClip(ClipDuration)));
        Advance(FadeDuration + Step);

        _player.StopMusic();
        Advance(ClipDuration * 2f);

        Assert.False(_output.IsMusicPlaying);
    }

    [Fact]
    public void PlaySfx_Clip_PlaysAtFullVolume()
    {
        var clip = new TestClip(1f);

        _player.PlaySfx(clip);

        Assert.Same(clip, Assert.Single(_output.PlayedSfx));
        Assert.Equal(1f, Assert.Single(_output.SfxVolumes));
    }

    [Theory]
    [InlineData(-1f, 0f)]
    [InlineData(0.4f, 0.4f)]
    [InlineData(3f, 1f)]
    public void PlaySfx_Volume_IsClamped(float volume, float expected)
    {
        _player.PlaySfx(new TestClip(1f), volume);

        Assert.Equal(expected, Assert.Single(_output.SfxVolumes));
    }

    [Fact]
    public void PlaySfx_NullClip_DoesNothing()
    {
        _player.PlaySfx(null);

        Assert.Empty(_output.PlayedSfx);
    }

    [Fact]
    public void PlaySfx_Muted_DoesNothing()
    {
        _player.SetSfxVolume(0f);

        _player.PlaySfx(new TestClip(1f));

        Assert.Empty(_output.PlayedSfx);
    }

    [Fact]
    public void SetSfxVolume_Zero_StopsPlayingSfx()
    {
        _player.SetSfxVolume(0f);

        Assert.Equal(0f, _player.SfxVolume);
        Assert.Equal(0f, _output.SfxVolume);
        Assert.Equal(1, _output.StopSfxCallCount);
    }

    [Theory]
    [InlineData(-1f, 0f)]
    [InlineData(0.3f, 0.3f)]
    [InlineData(2f, 1f)]
    public void SetMusicVolume_Value_IsClamped(float volume, float expected)
    {
        _player.SetMusicVolume(0.5f);

        _player.SetMusicVolume(volume);

        Assert.Equal(expected, _player.MusicVolume);
    }

    [Fact]
    public void SetMusicVolume_Zero_StopsTheMusic()
    {
        _player.PlayMusic(new TestClip(ClipDuration));
        Advance(FadeDuration + Step);

        _player.SetMusicVolume(0f);

        Assert.False(_output.IsMusicPlaying);
    }

    [Fact]
    public void SetMusicVolume_FromZero_RestartsTheMusic()
    {
        var clip = new TestClip(ClipDuration);
        _player.PlayMusic(clip);
        Advance(FadeDuration + Step);
        _player.SetMusicVolume(0f);

        _player.SetMusicVolume(0.8f);
        Advance(FadeDuration + Step);

        Assert.Same(clip, _output.Music);
        Assert.Equal(0.8f, _output.MusicVolume, 3);
    }

    [Fact]
    public void SetMusicVolume_WhileMuted_KeepsRequestedMusicForLater()
    {
        var clip = new TestClip(ClipDuration);
        _player.SetMusicVolume(0f);

        _player.PlayMusic(clip);
        Assert.False(_output.IsMusicPlaying);
        _player.SetMusicVolume(1f);

        Assert.Same(clip, _output.Music);
    }

    [Fact]
    public void SetMusicVolume_AfterFade_AppliesImmediately()
    {
        _player.PlayMusic(new TestClip(ClipDuration));
        Advance(FadeDuration + Step);

        _player.SetMusicVolume(0.25f);

        Assert.Equal(0.25f, _output.MusicVolume);
    }

    [Fact]
    public void Dispose_WhilePlaylistPlays_StopsAdvancing()
    {
        _player.PlayMusic(new Playlist<TestClip>(new TestClip(ClipDuration), new TestClip(ClipDuration)));
        Advance(FadeDuration + Step);
        var current = _output.Music;

        _player.Dispose();
        Advance(ClipDuration * 2f);

        Assert.Same(current, _output.Music);
        Assert.Equal(1, _output.PlayMusicCallCount);
    }

    [Fact]
    public void NextMusic_PlaylistLostClips_PlaysAClipItStillHas()
    {
        var playlist = new FakePlaylist(new TestClip(ClipDuration), new TestClip(ClipDuration), new TestClip(ClipDuration));
        _player.PlayMusic(playlist);
        Advance(FadeDuration + Step);

        while (_output.Music == playlist.Clips[0])
        {
            _player.NextMusic();
            Advance(FadeDuration + Step);
        }

        playlist.KeepFirst();

        _player.NextMusic();
        Advance(FadeDuration + Step);

        Assert.Same(playlist.Clips[0], _output.Music);
    }

    [Fact]
    public void NextMusic_PlaylistEmptied_StopsTheMusic()
    {
        var playlist = new FakePlaylist(new TestClip(ClipDuration), new TestClip(ClipDuration));
        _player.PlayMusic(playlist);
        Advance(FadeDuration + Step);
        playlist.Clear();

        _player.NextMusic();
        Advance(FadeDuration + Step);

        Assert.False(_output.IsMusicPlaying);
    }

    [Fact]
    public void Play_AfterWarmUp_DoesNotAllocate()
    {
        var first = new TestClip(ClipDuration);
        var second = new TestClip(ClipDuration);
        var sound = new TestClip(1f);
        var playlist = new Playlist<TestClip>(first, second);
        Play(first, second, sound, playlist);

        var before = GC.GetAllocatedBytesForCurrentThread();
        Play(first, second, sound, playlist);
        var allocated = GC.GetAllocatedBytesForCurrentThread() - before;

        Assert.Equal(0, allocated);
    }

    private void Play(TestClip first, TestClip second, TestClip sound, Playlist<TestClip> playlist)
    {
        _player.PlayMusic(first);
        Advance(FadeDuration + Step);
        _player.PlayMusic(second);
        _player.PlaySfx(sound);
        _player.PlaySfx(sound, 0.5f);
        _player.SetMusicVolume(0.5f);
        Advance(FadeDuration * 2f + Step);
        _player.PlayMusic(playlist);
        Advance(ClipDuration + Step);
        _player.NextMusic();
        _player.SetMusicVolume(0f);
        _player.SetMusicVolume(1f);
        Advance(FadeDuration + Step);
        _player.StopMusic();
        Advance(FadeDuration + Step);
    }
}
