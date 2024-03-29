﻿using UnityEngine;

namespace Chasing.Midi.Timeline
{
    [DisallowMultipleComponent]
    public class MidiActionReceiver : MonoBehaviour
    {
        public MidiAction action = new MidiAction();

        private void OnEnable()
        {
            action?.AddListener(MidiActionTrigger);
        }

        private void OnDisable()
        {
            action?.RemoveListener(MidiActionTrigger);
        }

        protected virtual void MidiActionTrigger(MidiNoteFilter noteFilter, float value)
        {
            Debug.Log($"MidiActionTrigger {noteFilter} {value}");
        }
    }
}
