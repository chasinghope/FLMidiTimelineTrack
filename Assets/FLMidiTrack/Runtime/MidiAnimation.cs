using System.Collections;
using System.Collections.Generic;
using UnityEngine.Playables;

namespace Chasing.Midi.Timeline
{
    /// <summary>
    /// 运行时基于动画计算Midi
    /// </summary>
    [System.Serializable]
    public sealed class MidiAnimation : PlayableBehaviour
    {
        public float tempo = 120;
        public uint duration;
        public uint ticksPerQuarterNote = 96;
        public MidiEvent[] events;

        public float DurationInSecond
        {
            get { return duration / tempo * 60 / ticksPerQuarterNote; }
        }



    }

}
