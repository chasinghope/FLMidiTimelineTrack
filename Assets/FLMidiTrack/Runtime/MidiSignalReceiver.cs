using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Chasing.Midi.Timeline
{
    /// <summary>
    /// 从Timeline接受MIDI信号(MIDI event notifications) 并触发相应事件 
    /// </summary>
    public sealed class MidiSignalReceiver : MonoBehaviour, INotificationReceiver
    {
        public MidiNoteFilter noteFilter = new MidiNoteFilter
        {
            note = MidiNote.All, octave = MidiOctave.All
        };


        public UnityEvent noteOnEvent = new UnityEvent();
        public UnityEvent noteOffEvent = new UnityEvent();


        public void OnNotify(Playable origin, INotification notification, object context)
        {
            MidiSignal signal = (MidiSignal)notification;

            if (signal.Event.IsNote)
            {
                if (!noteFilter.Check(signal.Event))
                    return;

                (signal.Event.IsNoteOn ? noteOnEvent : noteOffEvent).Invoke();
            }
        }
    }
}
