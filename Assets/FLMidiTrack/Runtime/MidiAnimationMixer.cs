using UnityEngine;
using UnityEngine.Playables;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Chasing.Midi.Timeline
{
    public class MidiAction
    {
        public UnityAction<float> action;


        public MidiAction(UnityAction<float> action)
        {
            this.action = action;
        }

        public MidiAction(MidiAction midiaction)
        {
            this.action = midiaction.action;
        }

        public void AddListener(UnityAction<float> action)
        {
            this.action += action;
        }

        public void RemoveListener(UnityAction<float> action)
        {
            this.action -= action;
        }

        public void RemoveAllListener()
        {
            this.action = null;
        }

        public void Invoke(float parameter)
        {
            action?.Invoke(parameter);
        }
    }



    [System.Serializable]
    public sealed class MidiAnimationMixer : PlayableBehaviour
    {

        public MidiControl[] controls = new MidiControl[0];

        //private MidiAction[] midiActions;

        public override void OnPlayableCreate(Playable playable)
        {
            IExposedPropertyTable resolver = playable.GetGraph().GetResolver();

            for (var i = 0; i < controls.Length; i++)
            {
                MidiControl midiControl = controls[i];
                Component component = midiControl.targetComponent.Resolve(resolver);
                if(component != null)
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

                float acc = 0f;

                for (int i = 0; i < playable.GetInputCount(); i++)
                {
                    ScriptPlayable<MidiAnimation> clip = (ScriptPlayable<MidiAnimation>)playable.GetInput(i);
                    acc += playable.GetInputWeight(i) * clip.GetBehaviour().GetValue(clip, control);
                }

                control.action?.Invoke(acc);
            }
        }
    }

}
