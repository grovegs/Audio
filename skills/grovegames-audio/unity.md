## Unity

- `IAudioPlayer<TClip>`, `IAudioOutput<TClip>` and `IPlaylist<TClip>` are in the `GroveGames.Audio` namespace. In Unity the clip type is `AudioClip`, so inject `IAudioPlayer<AudioClip>`. `GroveGames.Audio.Unity` holds the implementations: `UnityAudioPlayer`, `UnityAudioOutput`, `UnityPlaylist` and `UnityAudioSettings`. Never inject `UnityAudioPlayer`; inject the interface.
- A settings class declares `[SerializeField] private AudioClip _xSound = null!;` and exposes `public AudioClip XSound => _xSound;`. A playlist field is a `UnityPlaylist`.
- Register with `builder.AddAudio()` in the root installer. It also creates the audio listener, so never add another `AudioListener` to a scene or prefab.
- A test double implements `IAudioPlayer<AudioClip>`.
- An asmdef that overrides references lists `GroveGames.Audio.dll` in its precompiled references.
