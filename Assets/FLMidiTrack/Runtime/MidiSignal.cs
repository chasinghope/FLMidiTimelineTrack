using UnityEngine;
using UnityEngine.Playables;

namespace Chasing.Midi.Timeline
{
    /// <summary>
    /// 可接受 MIDI event notifications
    /// </summary>
    public sealed class MidiSignal : INotification
    {
        /// <summary>
        ///  Notification ID (not in use)
        /// </summary>
        PropertyName INotification.id { get { return default(PropertyName); } }

        /// <summary>
        ///  MIDI event
        /// </summary>
        public MidiEvent Event { get; set; }
    }
}
