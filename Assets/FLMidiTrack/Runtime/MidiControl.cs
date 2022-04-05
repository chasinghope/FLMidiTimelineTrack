using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Chasing.Midi.Timeline
{
    /// <summary>
    /// 音符
    /// </summary>
    public enum MidiNote
    {
        All,
        C,
        CSharp,
        D,
        DSharp,
        E,
        F,
        FSharp,
        G,
        GSharp,
        A,
        ASharp,
        B
    }

    /// <summary>
    /// 八度音阶
    /// </summary>
    public enum MidiOctave
    {
        All,
        Minus2,
        Minus1,
        Zero,
        Plus1,
        Plus2,
        Plus3,
        Plus4,
        Plus5,
        Plus6,
        Plus7,
        Plus8
    }

    /// <summary>
    /// Midi音符过滤器
    /// </summary>
    [System.Serializable]
    public struct MidiNoteFilter
    {
        public MidiNote note;
        public MidiOctave octave;

        public bool Check(in MidiEvent e)
        {
            return e.IsNote && (octave == MidiOctave.All || e.data1 / 12 == (int)octave - 1) && (note == MidiNote.All || e.data1 % 12 == (int)note - 1);
        }
    }

    /// <summary>
    /// Midi ADSR封装
    /// </summary>
    [System.Serializable]
    public struct MidiEnvelope
    {
        /// <summary>
        /// The attack phase begins the moment a key is pressed. This phase determines how quickly a sound reaches full volume before entering the decay phase.
        /// </summary>
        public float attack;

        /// <summary>
        /// The decay phase determines the length of the drop from the peak level to the sustain level of a sound. 
        /// </summary>
        public float decay;

        /// <summary>
        /// The sustain phase does not specify a length of time. Instead, it determines the volume of a sound for the entire hold time between the decay and release phases.
        /// </summary>
        public float sustain;

        /// <summary>
        /// The final phase determines the speed at which a sound ends from the moment you release the key.
        /// </summary>
        public float release;

        public float AttackTime => Mathf.Max(1e-5f, attack / 10);
        public float DecayTime => Mathf.Max(1e-5f, decay / 10);
        public float ReleaseTime => Mathf.Max(1e-5f, release / 10);

        public float SustainLevel => Mathf.Clamp01(sustain);
    }


    /// <summary>
    /// Midi控制器
    /// </summary>
    public sealed class MidiControl
    {
        /// <summary>
        /// Midi控制模式
        /// </summary>
        public enum Mode
        {
            NoteEnvelope, NoteCurve, CC
        }


        public bool enabled = true;
        public Mode mode = Mode.NoteEnvelope;


        public MidiNoteFilter noteFilter = new MidiNoteFilter
        {
            note = MidiNote.All,
            octave = MidiOctave.All
        };

        public MidiEnvelope envelope = new MidiEnvelope
        {
            attack = 0,
            decay = 1,
            sustain = 0.5f,
            release = 1
        };

        public AnimationCurve curve = new AnimationCurve
        (
            new Keyframe(0f, 0f, 90f, 90f),
            new Keyframe(0.02f, 1f),
            new Keyframe(0.5f, 0f)
        );

        public int ccNumber = 1;
    }
}
