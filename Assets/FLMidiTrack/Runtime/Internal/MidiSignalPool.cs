using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chasing.Midi.Timeline
{
    /// <summary>
    /// Object Pool class for Midi signals
    /// </summary>
    sealed class MidiSignalPool
    {
        Stack<MidiSignal> mUseSignals = new Stack<MidiSignal>();
        Stack<MidiSignal> mFreeSignals = new Stack<MidiSignal>();

        public MidiSignal Allocate(in MidiEvent data)
        {
            MidiSignal signal = mFreeSignals.Count > 0 ? mFreeSignals.Pop() : new MidiSignal();
            signal.Event = data;
            mUseSignals.Push(signal);
            return signal;
        }

        public void ResetFrame()
        {
            while(mUseSignals.Count > 0)
            {
                mFreeSignals.Push(mUseSignals.Pop());
            }
        }

    }

}