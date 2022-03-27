using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    


    class MidiControl
    {
    }
}
