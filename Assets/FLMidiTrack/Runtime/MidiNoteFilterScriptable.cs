using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chasing.Midi.Timeline
{
    [CreateAssetMenu(fileName = "MidiNoteConfig", menuName = "MidiNoteConfig")]
    public class MidiNoteFilterScriptable : ScriptableObject
    {
        public OctaveSetting[] octaveSettings = new OctaveSetting[12];
        public NoteSetting[] noteSettings = new NoteSetting[12];

        //[System.NonSerialized]
        //public Dictionary<MidiOctave, OctaveSetting> octaveDict = new Dictionary<MidiOctave, OctaveSetting>();
        //[System.NonSerialized]
        //public Dictionary<MidiNote, NoteSetting> noteDict = new Dictionary<MidiNote, NoteSetting>();

        //public void Awake()
        //{
        //    foreach (OctaveSetting item in octaveSettings)
        //    {
        //        if (!this.octaveDict.ContainsKey(item.octave))
        //        {
        //            octaveDict.Add(item.octave, item);
        //        }
        //        else
        //        {
        //            Debug.LogError("OctaveSetting ≈‰÷√÷ÿ∏¥");
        //        }
        //    }

        //    foreach (NoteSetting item in noteSettings)
        //    {
        //        if (!this.noteDict.ContainsKey(item.note))
        //        {
        //            noteDict.Add(item.note, item);
        //        }
        //        else
        //        {
        //            Debug.LogError("OctaveSetting ≈‰÷√÷ÿ∏¥");
        //        }
        //    }
        //}
    }

    [System.Serializable]
    public class OctaveSetting
    {
        public MidiOctave octave;
        public Color color = Color.white;
    }

    [System.Serializable]
    public class NoteSetting
    {
        public MidiNote note = MidiNote.C;
        public Color color = Color.white;
        public string name = "C";
    }

}