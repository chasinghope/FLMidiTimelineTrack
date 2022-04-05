using UnityEngine;
using UnityEngine.Playables;

namespace Chasing.Midi.Timeline
{
    [System.Serializable]
    public sealed class MidiAnimationMixer : PlayableBehaviour
    {

        public MidiControl[] controls = new MidiControl[0];

        

        public override void OnPlayableCreate(Playable playable)
        {
            base.OnPlayableCreate(playable);
        }


        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);
        }
    }

}
