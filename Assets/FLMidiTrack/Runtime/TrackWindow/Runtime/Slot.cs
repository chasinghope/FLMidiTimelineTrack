using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Chasing.Midi.Timeline;

namespace Chasing.TrackWindow
{
    public class Slot : MonoBehaviour
    {
        public RectTransform realRect;
        [SerializeField] Image typeImg;
        [SerializeField] TextMeshProUGUI des;
        [SerializeField] Image selectImg;
        [SerializeField] Button btn;

        private MidiEvent onMidiEvent;
        private MidiEvent offMidiEvent;
        private bool isSelect;

        private void Awake()
        {
            selectImg.enabled = false;
            btn.onClick.AddListener(()=> TrackWindowsController.Instance.selectSlot.SelectSlotUnit(this));
        }

        public void Refresh(MidiEvent onMidiEvent, MidiEvent offMidiEvent)
        {
            this.onMidiEvent = onMidiEvent;
            this.offMidiEvent = offMidiEvent;
            typeImg.color = TrackWindowsController.Instance.octaveDict[this.onMidiEvent.Octave].color;
            des.text = TrackWindowsController.Instance.noteDict[this.onMidiEvent.Note].name;
        }


        public CustomExpandData GetCustomData()
        {
            return this.onMidiEvent.customData;
        }

        public void SetNewCustomData(CustomExpandData data)
        {
            this.offMidiEvent.customData = data;
            this.onMidiEvent.customData = data;
        }

        public Slot SelecteSlot()
        {
            isSelect = !isSelect;
            selectImg.enabled = isSelect;

            return isSelect ? this : null;
        }



    }
}
