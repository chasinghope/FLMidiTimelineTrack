using System;


namespace Chasing.Midi.Timeline
{
    [System.Serializable]
    public class CustomExpandData
    {
        public NoteType noteType;

        public override string ToString()
        {
            return $"noteType: {noteType}";
        }
    }


    public enum NoteType
    {
        None = -1,
        A = 0,
        B,
        C,
        D,
    }

}



