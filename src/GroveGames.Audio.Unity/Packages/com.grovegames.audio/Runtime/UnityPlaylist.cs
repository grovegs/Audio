using System.Collections.Generic;
using UnityEngine;

namespace GroveGames.Audio.Unity
{
    [CreateAssetMenu(fileName = "Playlist", menuName = "GroveGames/Audio/Playlist")]
    public sealed class UnityPlaylist : ScriptableObject, IPlaylist<AudioClip>
    {
        [SerializeField] private AudioClip[] _clips;

        public IReadOnlyList<AudioClip> Clips => _clips;
    }
}
