# GroveGames.Audio

[![Build Status](https://github.com/grovegs/Audio/actions/workflows/release.yml/badge.svg)](https://github.com/grovegs/Audio/actions/workflows/release.yml)
[![Tests](https://github.com/grovegs/Audio/actions/workflows/tests.yml/badge.svg)](https://github.com/grovegs/Audio/actions/workflows/tests.yml)
[![NuGet](https://img.shields.io/nuget/v/GroveGames.Audio)](https://www.nuget.org/packages/GroveGames.Audio)
[![Latest Release](https://img.shields.io/github/v/release/grovegs/Audio)](https://github.com/grovegs/Audio/releases/latest)

Music and sound effect playback with playlists and fades for .NET and Unity

## Features

- `IAudioPlayer<TClip>` and `AudioPlayer<TClip>`: one music track at a time with a fade between tracks, shuffled playlists, one-shot sound effects, and separate music and sound effect volumes
- `IAudioOutput<TClip>`: the one thing an engine implements. `TClip` is the engine's clip type
- `IPlaylist<TClip>` and `Playlist<TClip>`: a set of clips played in random order without repeating the current one
- `IAudioSettings` and `AudioSettings`: the music fade duration
- Unity: `UnityAudioPlayer`, `UnityAudioOutput`, a `UnityPlaylist` asset and a `UnityAudioSettings` asset in Project Settings
- Test doubles for your own tests: `FakeAudioPlayer` in the editor-only `GroveGames.Audio.Unity.Testing` assembly

## .NET

### Installation

```bash
dotnet add package GroveGames.Audio
```

### Usage

```csharp
using GroveGames.Audio;

IAudioPlayer<MyClip> player = new AudioPlayer<MyClip>(output, new AudioSettings(0.5f), scheduler, tweener, random);

player.PlayMusic(track);
player.PlayMusic(new Playlist<MyClip>(first, second, third));
player.PlaySfx(click);
player.PlaySfx(hit, 0.6f);
player.SetMusicVolume(0.8f);
player.SetSfxVolume(0f);
```

- `output` is your `IAudioOutput<MyClip>`. The scheduler, tweener and random number generator come from [GroveGames.Scheduling](https://github.com/grovegs/Scheduling), [GroveGames.Tween](https://github.com/grovegs/Tween) and [GroveGames.Mathematics](https://github.com/grovegs/Mathematics).
- `PlayMusic(clip)` loops one clip. `PlayMusic(playlist)` plays the playlist's clips in random order and moves on shortly before each one ends. Playing the clip or playlist that is already playing does nothing.
- A change of track fades the old one out and the new one in over `MusicFadeDuration`, in unscaled time.
- Volumes are clamped to 0 to 1. A music volume of 0 stops the music and remembers it; raising the volume starts it again. A sound effect volume of 0 stops playing effects and ignores new ones.
- `PlayMusic(null)` stops the music. `PlaySfx(null)` does nothing.

## Unity

### Installation

1. Install the core package via [NuGetForUnity](https://github.com/GlitchEnzo/NuGetForUnity):
   - Open Window > NuGet > Manage NuGet Packages
   - Search for `GroveGames.Audio` and install

2. Add the Unity package via Git URL:
   - Open Package Manager (Window > Package Manager)
   - Click "+" > "Add package from git URL"
   - Enter: `https://github.com/grovegs/Audio.git?path=src/GroveGames.Audio.Unity/Packages/com.grovegames.audio`

3. Create a `csc.rsp` file in your `Assets/` directory with the following content to enable C# 10 features:

   ```text
   -langversion:10
   -nullable:enable
   ```

### Usage

With [GroveGames.DependencyInjection](https://github.com/grovegs/DependencyInjection) installed, `AddAudio()` registers `UnityAudioPlayer` as `IAudioPlayer<AudioClip>`, `UnityAudioOutput` as `IAudioOutput<AudioClip>` and the settings, and creates the audio listener. The container must also have `IScheduler`, `ITweener` and `IRandomNumberGenerator`.

```csharp
using GroveGames.Audio;
using UnityEngine;

public sealed class ShopController
{
    private readonly IAudioPlayer<AudioClip> _audio;

    public ShopController(IAudioPlayer<AudioClip> audio)
    {
        _audio = audio;
    }

    public void Buy(AudioClip purchaseSound)
    {
        _audio.PlaySfx(purchaseSound);
    }
}
```

- Create a playlist with Assets > Create > GroveGames > Audio > Playlist and pass the `UnityPlaylist` to `PlayMusic`.
- The music fade duration is in Project Settings > GroveGames > Audio. It is stored in `Assets/Settings/GroveGames/AudioSettings.asset`, a `UnityAudioSettings` that ships as a preloaded asset.
- `UnityAudioOutput` is a prefab with one audio source for music and one for sound effects. The settings asset points at it and at the listener prefab.

## Development

### Project Structure

- `src/GroveGames.Audio/` - Core .NET library (`net10.0`, `netstandard2.1`)
- `src/GroveGames.Audio.Unity/` - Unity package
- `tests/` - xUnit tests
- `sandbox/` - Sample applications for testing

### Commands

```bash
dotnet build                        # Build all projects
dotnet format                       # Format code
dotnet test                         # Run tests
dotnet pack -c Release              # Create NuGet packages
```

### Requirements

- .NET 10.0 SDK

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
