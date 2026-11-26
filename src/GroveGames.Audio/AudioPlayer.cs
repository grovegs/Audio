using GroveGames.Mathematics;
using GroveGames.Scheduling;
using GroveGames.Tween;

namespace GroveGames.Audio;

public sealed class AudioPlayer<TClip> : IAudioPlayer<TClip>, IDisposable where TClip : class
{
    private const float RelativeTolerance = 1e-6f;
    private const float AbsoluteTolerance = float.Epsilon * 8f;

    private readonly IScheduler _scheduler;
    private readonly ITweener _tweener;
    private readonly IRandomNumberGenerator _random;
    private readonly float _musicFadeDuration;
    private readonly IAudioOutput<TClip> _output;
    private readonly Action _nextMusic;
    private readonly Action _startMusic;
    private readonly ScheduleToken _playlistToken;
    private float _musicVolume;
    private float _sfxVolume;
    private TClip? _musicClip;
    private bool _musicLoop;
    private IPlaylist<TClip>? _playlist;
    private int _playlistIndex;
    private TweenHandle _musicFade;

    public float MusicVolume => _musicVolume;
    public float SfxVolume => _sfxVolume;

    public AudioPlayer(IAudioOutput<TClip> output, IAudioSettings settings, IScheduler scheduler, ITweener tweener, IRandomNumberGenerator random)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(scheduler);
        ArgumentNullException.ThrowIfNull(tweener);
        ArgumentNullException.ThrowIfNull(random);

        _output = output;
        _scheduler = scheduler;
        _tweener = tweener;
        _random = random;
        _musicFadeDuration = settings.MusicFadeDuration;
        _nextMusic = NextMusic;
        _startMusic = StartMusic;
        _playlistToken = new ScheduleToken();
        _musicVolume = 1f;
        _sfxVolume = 1f;
        _output.SetMusicVolume(_musicVolume);
        _output.SetSfxVolume(_sfxVolume);
    }

    public void PlayMusic(TClip? clip)
    {
        if (clip == null)
        {
            StopMusic();
            return;
        }

        StopPlaylist();

        if (clip == _musicClip)
        {
            _musicLoop = true;
            _output.SetMusicLoop(true);
            return;
        }

        ChangeMusic(clip, true);
    }

    public void PlayMusic(IPlaylist<TClip>? playlist)
    {
        if (playlist == _playlist)
        {
            return;
        }

        var clips = playlist?.Clips;

        if (clips == null || clips.Count == 0)
        {
            return;
        }

        StopPlaylist();
        _playlist = playlist;
        _playlistIndex = _random.NextInt(clips.Count - 1);
        ChangeMusic(clips[_playlistIndex], false);
    }

    public void NextMusic()
    {
        if (_playlist == null)
        {
            return;
        }

        var clips = _playlist.Clips;

        if (clips.Count == 0)
        {
            StopMusic();
            return;
        }

        if (clips.Count > 1)
        {
            var candidate = _random.NextInt(clips.Count - 2);
            _playlistIndex = candidate >= _playlistIndex ? candidate + 1 : candidate;
        }
        else
        {
            _playlistIndex = 0;
        }

        ChangeMusic(clips[_playlistIndex], false);
    }

    public void StopMusic()
    {
        StopPlaylist();
        ChangeMusic(null, false);
    }

    public void PlaySfx(TClip? clip)
    {
        if (clip == null || IsMuted(_sfxVolume))
        {
            return;
        }

        _output.PlaySfx(clip, 1f);
    }

    public void PlaySfx(TClip? clip, float volume)
    {
        if (clip == null || IsMuted(_sfxVolume))
        {
            return;
        }

        _output.PlaySfx(clip, Math.Clamp(volume, 0f, 1f));
    }

    public void SetMusicVolume(float volume)
    {
        volume = Math.Clamp(volume, 0f, 1f);

        if (IsApproximately(_musicVolume, volume))
        {
            return;
        }

        var wasMuted = IsMuted(_musicVolume);
        _musicVolume = volume;

        if (IsMuted(volume))
        {
            _musicFade.Cancel();
            _playlistToken.Cancel();
            _output.StopMusic();
            return;
        }

        if (wasMuted)
        {
            StartMusic();
            return;
        }

        if (!_musicFade.IsActive)
        {
            _output.SetMusicVolume(volume);
        }
    }

    public void SetSfxVolume(float volume)
    {
        volume = Math.Clamp(volume, 0f, 1f);

        if (IsApproximately(_sfxVolume, volume))
        {
            return;
        }

        _sfxVolume = volume;
        _output.SetSfxVolume(volume);

        if (IsMuted(volume))
        {
            _output.StopSfx();
        }
    }

    public void Dispose()
    {
        _musicFade.Cancel();
        _playlistToken.Cancel();
    }

    private void ChangeMusic(TClip? clip, bool loop)
    {
        _playlistToken.Cancel();
        _musicClip = clip;
        _musicLoop = loop;
        _musicFade.Cancel();

        if (!_output.IsMusicPlaying || _output.MusicVolume <= 0f)
        {
            StartMusic();
            return;
        }

        _musicFade = _tweener.Create(_output.MusicVolume, 0f, _musicFadeDuration)
            .WithTimeMode(TimeMode.Unscaled)
            .WithCompletionCallback(_startMusic)
            .Bind(_output, static (volume, output) => output.SetMusicVolume(volume));
    }

    private void StartMusic()
    {
        _output.StopMusic();

        if (_musicClip == null || IsMuted(_musicVolume))
        {
            return;
        }

        _output.SetMusicVolume(0f);
        _output.PlayMusic(_musicClip, _musicLoop);
        _musicFade = _tweener.Create(0f, 1f, _musicFadeDuration)
            .WithTimeMode(TimeMode.Unscaled)
            .Bind(this, static (progress, player) => player._output.SetMusicVolume(progress * player._musicVolume));

        if (_musicLoop)
        {
            return;
        }

        _playlistToken.Cancel();
        _playlistToken.Reset();
        _scheduler.Schedule(_nextMusic, Math.Max(0d, _output.GetDuration(_musicClip) - _musicFadeDuration), _playlistToken, TimeMode.Unscaled);
    }

    private void StopPlaylist()
    {
        _playlistToken.Cancel();
        _playlist = null;
    }

    private static bool IsMuted(float volume)
    {
        return IsApproximately(volume, 0f);
    }

    private static bool IsApproximately(float a, float b)
    {
        return MathF.Abs(b - a) < MathF.Max(RelativeTolerance * MathF.Max(MathF.Abs(a), MathF.Abs(b)), AbsoluteTolerance);
    }
}
