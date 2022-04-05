using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Chasing.Midi.Timeline
{
    /// <summary>
    /// Midi track 资源,将作为 Timeline中midi Animation的资源
    /// </summary>
    [System.Serializable]
    public sealed class MidiAnimationAsset : PlayableAsset, ITimelineClipAsset
    {
        public MidiAnimation template = new MidiAnimation();

        public override double duration => template.DurationInSecond;

        public ClipCaps clipCaps
        {
            get
            {
                return ClipCaps.Blending       |
                       ClipCaps.ClipIn         |
                       ClipCaps.Extrapolation  |
                       ClipCaps.Looping        |
                       ClipCaps.SpeedMultiplier;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            return ScriptPlayable<MidiAnimation>.Create(graph, template);
        }
    }

}
