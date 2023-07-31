using PlasticGui.Configuration.CloudEdition.Welcome;

namespace Chasing.Midi.Timeline
{
    /// <summary>
    /// MIDI event raw data struct
    /// </summary>
    [System.Serializable]
    public class MidiEvent
    {
        public uint time;
        public byte status;

        public byte data1;
        public byte data2;

        public CustomExpandData customData;

        public bool IsCC => (status & 0xb0) == 0xb0;
        public bool IsNote => (status & 0xe0) == 0x80;
        public bool IsNoteOn => (status & 0xf0) == 0x90;
        public bool IsNoteOff => (status & 0xf0) == 0x80;

        public MidiOctave Octave => (MidiOctave)(data1 / 12 + 1);
        public MidiNote Note => (MidiNote)(data1 % 12 + 1);



        public override string ToString()
        {
            return string.Format("[{0}: {1},   IsNoteOn:{2}, IsNoteOff:{3}  IsNote: {4}  IsCC: {5}]  [CustomData: {6}]", time, Note, IsNoteOn, IsNoteOff, IsNote, IsCC, customData);
        }

        public static bool IsSameKey(MidiEvent a, MidiEvent b)
        {
            return a.data1 == b.data1;
        }

        public static bool IsOnOffComplete(MidiEvent a, MidiEvent b)
        {
            if (!IsSameKey(a, b))
                return false;

            return a.IsNoteOn == true && b.IsNoteOff == true;
        }
    }




}
