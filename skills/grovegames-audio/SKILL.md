---
name: grovegames-audio
description: Rules for music and sound effects in game code with GroveGames.Audio. Use when writing or reviewing code that plays a sound effect or music, adds a clip or playlist to a settings class, or changes the music or sound effect volume.
---

# Using GroveGames.Audio

Rules for game code that uses this package. They hold in every game. The API is in the package's `README.md`.

- Take `IAudioPlayer<TClip>` through the constructor, with the engine's clip type. Never play a clip through an engine audio API.
- A clip is a tunable: it lives in the feature's settings, not in a controller. Play it with `_audio.PlaySfx(_settings.XSound)`.
- Play a sound effect from the controller, at the moment the player sees the result. Never from a service, and never per frame.
- Music is one track or one playlist at a time. Call `PlayMusic` with the track you want; the player fades from the old one. Do not stop the music first, and do not fade volumes yourself.
- The player's music and sound preferences are `SetMusicVolume` and `SetSfxVolume`. Do not check a preference before `PlaySfx` or `PlayMusic`; a muted player ignores them and remembers the music, and unmuting restarts the current track.
- Do not null-check a clip before playing it; a missing clip plays nothing.
- You cannot hear the result. Say so and ask the user to listen to it.
