using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Chasing.Midi.Timeline
{
    /// <summary>
    /// 从Timeline接受MIDI信号(MIDI event notifications) 并触发相应事件 
    /// </summary>
    public sealed class MidiGlobalSignalReceiver : MonoBehaviour, INotificationReceiver
    {
        public MidiNoteFilter noteFilter = new MidiNoteFilter
        {
            note = MidiNote.All,
            octave = MidiOctave.All
        };


        public UnityAction<MidiNoteFilter> noteFilterOnEvent;
        public UnityAction<MidiNoteFilter> noteFilterOffEvent;


        public void OnNotify(Playable origin, INotification notification, object context)
        {
            MidiSignal signal = (MidiSignal)notification;

            if (signal.Event.IsNote)
            {
                MidiOctave nOctave = (MidiOctave)(signal.Event.data1 / 12 + 1);
                MidiNote nNote = (MidiNote)(signal.Event.data1 % 12 + 1);
                (signal.Event.IsNoteOn ? noteFilterOnEvent : noteFilterOffEvent)?.Invoke(new MidiNoteFilter() { note = nNote, octave = nOctave });
            }
        }
    }
}
