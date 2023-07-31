using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chasing.Midi.Timeline
{
    public class GenerateBlockArea : MonoBehaviour
    {
        [Header("成员")]
        public MidiNoteFilterScriptable mSOnoteFilter;
        public BlockNoteFilter mBlockNoteFilter;
        public Transform mBlockParent;
        public MidiGlobalSignalReceiver midiSignalReceive;

        [Header("Gizoms")]
        public Transform[] mAchorArray = new Transform[4];
        public Transform mStart;
        public float mSpacing = 30f;
        public int columnNumber = 8;

        private int nextColum = 0;
        public int NextColumn
        {
            get
            {
                nextColum++;
                if (nextColum == columnNumber)
                    nextColum = 0;

                return nextColum;
            }
        }

        public Dictionary<MidiNoteFilter, BlockNoteFilter> mUINoteDict = new Dictionary<MidiNoteFilter, BlockNoteFilter>();

        private Dictionary<MidiOctave, OctaveSetting> octaveDict = new Dictionary<MidiOctave, OctaveSetting>();
        private Dictionary<MidiNote, NoteSetting> noteDict = new Dictionary<MidiNote, NoteSetting>();

        private void Awake()
        {
            mBlockNoteFilter.gameObject.SetActive(false);

            foreach (OctaveSetting item in mSOnoteFilter.octaveSettings)
            {
                if (!this.octaveDict.ContainsKey(item.octave))
                {
                    octaveDict.Add(item.octave, item);
                }
                else
                {
                    Debug.LogError("OctaveSetting 配置重复");
                }
            }

            foreach (NoteSetting item in mSOnoteFilter.noteSettings)
            {
                if (!this.noteDict.ContainsKey(item.note))
                {
                    noteDict.Add(item.note, item);
                }
                else
                {
                    Debug.LogError("OctaveSetting 配置重复");
                }
            }
        }

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


        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(mAchorArray[0].localPosition, mAchorArray[1].localPosition);
            Gizmos.DrawLine(mAchorArray[1].localPosition, mAchorArray[2].localPosition);
            Gizmos.DrawLine(mAchorArray[2].localPosition, mAchorArray[3].localPosition);
            Gizmos.DrawLine(mAchorArray[3].localPosition, mAchorArray[0].localPosition);
            for (int i = 0; i < columnNumber; i++)
            {
                Gizmos.DrawSphere(mStart.localPosition + new Vector3(1, 0, 0) * i * mSpacing, 0.2f);
            }
        }


        private void NoteFilterOnEvent(MidiNoteFilter arg0)
        {
            SetBlock(arg0);
        }

        private void NoteFilterOffEvent(MidiNoteFilter arg0)
        {
            HideBlock(arg0);
        }


        private void SetBlock(MidiNoteFilter noteFilter, float alpha = 1f)
        {
            BlockNoteFilter block = null;
            if (mUINoteDict.ContainsKey(noteFilter))
            {
                block = mUINoteDict[noteFilter];
            }
            else
            {
                GameObject obj = GameObject.Instantiate(mBlockNoteFilter.gameObject, mBlockParent);
                if (obj == null)
                    Debug.Log("obj == null");
                block = obj.GetComponent<BlockNoteFilter>();
                if (block == null)
                    Debug.Log("block == null");
                mUINoteDict.Add(noteFilter, block);
            }

            NoteSetting noteSetting = noteDict[noteFilter.note];
            OctaveSetting octaveSetting = octaveDict[noteFilter.octave];
            block.transform.localPosition = mStart.localPosition + new Vector3(1, 0, 0) * NextColumn * mSpacing;
            block.Set(noteSetting.name, noteSetting.color, octaveSetting.color, alpha);
        }


        private void HideBlock(MidiNoteFilter noteFilter)
        {
            if(mUINoteDict.TryGetValue(noteFilter, out BlockNoteFilter value))
            {
                value.Hide();
            }
        }

    }
}
