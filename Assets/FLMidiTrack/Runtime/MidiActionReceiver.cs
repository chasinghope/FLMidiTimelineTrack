using UnityEngine;

namespace Chasing.Midi.Timeline
{
    [DisallowMultipleComponent]
    public class MidiActionReceiver : MonoBehaviour
    {
        public MidiAction action;

        private void OnEnable()
        {
            action?.AddListener(MidiActionTrigger);
        }

        private void OnDisable()
        {
            action?.RemoveListener(MidiActionTrigger);
        }

        protected virtual void MidiActionTrigger(float value)
        {
            Debug.Log($"MidiActionTrigger {value}");
        }
    }
}
