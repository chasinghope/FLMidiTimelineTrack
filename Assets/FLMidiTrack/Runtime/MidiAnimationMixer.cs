using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;
using UnityEngine.Events;
using System;

namespace Chasing.Midi.Timeline
{
    public class MidiAction
    {
        public UnityAction<MidiNoteFilter, float> action;

        public MidiAction()
        {

        }


        public MidiAction(UnityAction<MidiNoteFilter, float> action)
        {
            this.action = action;
        }

        public MidiAction(MidiAction midiaction)
        {
            this.action = midiaction.action;
        }

        public void AddListener(UnityAction<MidiNoteFilter, float> action)
        {
            this.action += action;
        }

        public void RemoveListener(UnityAction<MidiNoteFilter, float> action)
        {
            this.action -= action;
        }

        public void RemoveAllListener()
        {
            this.action = null;
        }

        public void Invoke(MidiNoteFilter noteFilter,float parameter)
        {
            action?.Invoke(noteFilter, parameter);
        }
    }



    [System.Serializable]
    public sealed class MidiAnimationMixer : PlayableBehaviour
    {

        public MidiControl[] controls = new MidiControl[0];

        //private MidiAction[] midiActions;

        //public override void OnPlayableCreate(Playable playable)
        //{
        //    IExposedPropertyTable resolver = playable.GetGraph().GetResolver();

        //    for (var i = 0; i < controls.Length; i++)
        //    {
        //        MidiControl midiControl = controls[i];
        //        Component component = midiControl.targetComponent.Resolve(resolver);
        //        if(component != null)
        //        {
        //            MidiActionReceiver midiActionReceiver = component.gameObject.GetComponent<MidiActionReceiver>();
        //            if (midiActionReceiver != null)
        //                midiControl.action = new MidiAction(midiActionReceiver.action);
        //            else
        //                Debug.LogWarning($"[Midi Track] control element {i + 1} component is must be <MidiActionReceiver>");
        //        }
        //        else
        //        {
        //            Debug.LogWarning($"[Midi Track] control element {i + 1} component is null, you must load <MidiActionReceiver>");
        //        }

        //    }
                
        //}

        public override void OnGraphStart(Playable playable)
        {
            IExposedPropertyTable resolver = playable.GetGraph().GetResolver();

            for (var i = 0; i < controls.Length; i++)
            {
                MidiControl midiControl = controls[i];
                Component component = midiControl.targetComponent.Resolve(resolver);
                if (component != null)
                {
                    MidiActionReceiver midiActionReceiver = component.gameObject.GetComponent<MidiActionReceiver>();
                    if (midiActionReceiver != null)
                        midiControl.action = new MidiAction(midiActionReceiver.action);
                    else
                        Debug.LogWarning($"[Midi Track] control element {i + 1} component is must be <MidiActionReceiver>");
                }
                else
                {
                    Debug.LogWarning($"[Midi Track] control element {i + 1} component is null, you must load <MidiActionReceiver>");
                }

            }
        }



        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            foreach (MidiControl control in controls)
            {
                if (!control.enabled) continue;

                //// 单个具体音符
                //if (!control.noteFilter.IsAllNote())
                //{
                //    ProcessFrameValue(playable, control);
                //}
                //else
                //{
                //    // 一组音符
                //    if (control.noteFilter.IsGroupNote())
                //    {
                //        foreach (MidiNote note in Enum.GetValues(typeof(MidiNote)))
                //        {
                //            if (note == MidiNote.All) continue;
                //            MidiNoteFilter noteFilter = new MidiNoteFilter() { octave = control.noteFilter.octave, note = note };
                //            MidiControl midiControl = control.Copy();
                //            midiControl.noteFilter = noteFilter;
                //            ProcessFrameValue(playable, midiControl);
                //        }
                //    }
                //    else
                //    {

                //        // 全音符
                //        foreach (MidiOctave octave in Enum.GetValues(typeof(MidiOctave)))
                //        {
                //            if (octave == MidiOctave.All) continue;
                //            foreach (MidiNote note in Enum.GetValues(typeof(MidiNote)))
                //            {
                //                if (note == MidiNote.All) continue;
                //                MidiNoteFilter noteFilter = new MidiNoteFilter() { octave = octave, note = note };
                //                MidiControl midiControl = control.Copy();
                //                midiControl.noteFilter = noteFilter;
                //                ProcessFrameValue(playable, midiControl);
                //            }
                //        }
                //    }
                //}

                if (control.noteFilter.IsAllNote())
                {
                    // 全音符
                    foreach (MidiOctave octave in Enum.GetValues(typeof(MidiOctave)))
                    {
                        if (octave == MidiOctave.All) continue;
                        foreach (MidiNote note in Enum.GetValues(typeof(MidiNote)))
                        {
                            if (note == MidiNote.All) continue;
                            MidiNoteFilter noteFilter = new MidiNoteFilter() { octave = octave, note = note };
                            MidiControl midiControl = control.Copy();
                            midiControl.noteFilter = noteFilter;
                            ProcessFrameValue(playable, midiControl);
                        }
                    }
                }
                else if (control.noteFilter.IsGroupNote())
                {
                    // 一组音符
                    foreach (MidiNote note in Enum.GetValues(typeof(MidiNote)))
                    {
                        if (note == MidiNote.All) continue;
                        MidiNoteFilter noteFilter = new MidiNoteFilter() { octave = control.noteFilter.octave, note = note };
                        MidiControl midiControl = control.Copy();
                        midiControl.noteFilter = noteFilter;
                        ProcessFrameValue(playable, midiControl);
                    }
                }
                else
                {
                    // 单个具体音符
                    ProcessFrameValue(playable, control);
                }


            }
        }


        private void ProcessFrameValue(Playable playable, MidiControl midControl)
        {
            float acc = 0f;

            for (int i = 0; i < playable.GetInputCount(); i++)
            {
                ScriptPlayable<MidiAnimation> clip = (ScriptPlayable<MidiAnimation>)playable.GetInput(i);
                acc += playable.GetInputWeight(i) * clip.GetBehaviour().GetValue(clip, midControl);
            }

            midControl.action?.Invoke(midControl.noteFilter, acc);
        }
    }

}
