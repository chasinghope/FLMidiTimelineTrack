using Chasing.Midi.Timeline;
using UnityEngine;
using UnityEngine.Events;

public class NoteEventShow : MonoBehaviour
{
    public MidiGlobalSignalReceiver midiSignalReceive;

    public void OnEnable()
    {
        midiSignalReceive.noteFilterOnEvent += NoteFilterOnEvent;
        midiSignalReceive.noteFilterOffEvent += NoteFilterOffEvent;
    }

    public void OnDisable()
    {
        midiSignalReceive.noteFilterOnEvent -= NoteFilterOnEvent;
        midiSignalReceive.noteFilterOffEvent -= NoteFilterOffEvent;
    }

    private void NoteFilterOffEvent(MidiNoteFilter arg0)
    {
        Debug.Log($"[OFF] note: {arg0.note}, octave: {arg0.octave}");
    }

    private void NoteFilterOnEvent(MidiNoteFilter arg0)
    {
        Debug.Log($"[ON] note: {arg0.note}, octave: {arg0.octave}");
    }

}
