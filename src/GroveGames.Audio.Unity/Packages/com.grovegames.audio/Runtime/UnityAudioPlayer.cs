using GroveGames.Mathematics;
using GroveGames.Scheduling;
using GroveGames.Tween;
using UnityEngine;

namespace GroveGames.Audio.Unity
{
    public sealed class UnityAudioPlayer : AudioPlayer<AudioClip>
    {
        public UnityAudioPlayer(IAudioOutput<AudioClip> output, IAudioSettings settings, IScheduler scheduler, ITweener tweener, IRandomNumberGenerator random) : base(output, settings, scheduler, tweener, random)
        {
        }
    }
}
