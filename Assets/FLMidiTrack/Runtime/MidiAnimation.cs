using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace Chasing.Midi.Timeline
{
    /// <summary>
    /// 运行时基于动画计算Midi
    /// </summary>
    [System.Serializable]
    public sealed class MidiAnimation : PlayableBehaviour
    {
        /// <summary>
        /// BPM
        /// </summary>
        public float tempo = 120;
        public uint duration;
        public uint ticksPerQuarterNote;// = 96;
        public MidiEvent[] events;

        public float DurationInSecond
        {
            get { return duration / tempo * 60 / ticksPerQuarterNote; }
        }

        public float GetValue(Playable playable, MidiControl control)
        {
            if (events == null)
                return 0f;
            float t = (float)playable.GetTime() % DurationInSecond;
            switch (control.mode)
            {
                case MidiControl.Mode.NoteEnvelope:
                    return GetNoteEnvelopeValue(control, t);
                case MidiControl.Mode.NoteCurve:
                    return GetNoteCurveValue(control, t);
                case MidiControl.Mode.CC:
                    return GetCCValue(control, t);
                default:
                    return -1f;
            }
        }

        #region PlayableBehaviour Override

        float mPreviourTime;

        public override void OnGraphStart(Playable playable)
        {
            mPreviourTime = (float)playable.GetTime();
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            //处理playable执行完时的情况：clip的signal全部触发
            if (!playable.IsDone())
                return;
            float duration = (float)playable.GetDuration();
            TriggerSignals(playable, info.output, mPreviourTime, duration);
        }

        /// <summary>
        /// PrepareFrame should be used to do topological modifications, change connection weights, time changes , etc.
        /// </summary>
        /// <param name="playable"></param>
        /// <param name="info"></param>
        public override void PrepareFrame(Playable playable, FrameData info)
        {
            Debug.Log($"time check {playable.GetTime()}");
            float current = (float)playable.GetTime();

            if(info.evaluationType == FrameData.EvaluationType.Playback)
            {
                TriggerSignals(playable, info.output, mPreviourTime, current);
            }
            else
            {
                const float maxDiff = 0.1f;
                if (current - mPreviourTime < maxDiff)
                {
                    TriggerSignals(playable, info.output, mPreviourTime, current);
                }
                else
                {
                    float t0 = Mathf.Max(0, current - maxDiff);
                    TriggerSignals(playable, info.output, t0, current);
                }
            }

            mPreviourTime = current;
        }


        #endregion


        MidiSignalPool mSignPool = new MidiSignalPool();

        private void TriggerSignals(Playable playable, PlayableOutput output, float previous, float current)
        {
            mSignPool.ResetFrame();
            uint pre_tick = ConvertSecondToTick(previous);
            uint cur_tick = ConvertSecondToTick(current);

            if(cur_tick < pre_tick)
            {
                cur_tick = cur_tick + (pre_tick / duration + 1) * duration;
            }

            uint offset = (pre_tick / duration) * duration;
            pre_tick -= offset;
            cur_tick -= offset;

            for (; cur_tick >= duration; cur_tick -= duration)
            {
                TriggerSignalsTick(playable, output, pre_tick, 0xffffffffu);
                pre_tick = 0;
            }

            TriggerSignalsTick(playable, output, pre_tick, cur_tick);
        }

        private void TriggerSignalsTick(Playable playable, PlayableOutput output, uint previous, uint current)
        {
            foreach (MidiEvent e in events)
            {
                if (e.time >= current)         //后续midi事件已超时
                    break;
                if (e.time < previous)         //当前midi事件还未执行到
                    continue;
                if (!e.IsNote)                //非按下事件不做处理
                    continue;
                output.PushNotification(playable, mSignPool.Allocate(e));
            }
        }


        private (int i0, int i1) GetCCEventIndexAroundTick(uint tick, int ccNumber)
        {
            int last = -1;
            for (int i = 0; i < events.Length; i++)
            {
                MidiEvent e = events[i];
                if (!e.IsCC || e.data1 != ccNumber)
                    continue;
                if (e.time > tick)
                    return (last, i);
                last = i;
            }
            return (last, last);
        }

        /// <summary>
        /// 从MidiEvent中找到符合tick 和 midiNoteFilter On Off 的MidiEvent  indexs
        /// </summary>
        /// <param name="tick"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        private (int iOn, int iOff) GetNoteEventBeforeTick(uint tick, MidiNoteFilter note)
        {
            int iOn = -1;
            int iOff = -1;
            for (int i = 0; i < events.Length; i++)
            {
                MidiEvent e = events[i];
                if (e.time > tick)
                    break;
                if (!note.Check(e))
                    continue;
                if (e.IsNoteOn)
                    iOn = i;
                else
                    iOff = i;
            }
            return (iOn, iOff);
        }

        private uint ConvertSecondToTick(float time) => (uint)(time * tempo / 60 * ticksPerQuarterNote);

        private float ConvertTicksToSecond(uint tick) => tick * 60 / (tempo * ticksPerQuarterNote);

        private float CalculateEnvelope(MidiEnvelope envelope, float onTime, float offTime)
        {
            float attackTime = envelope.AttackTime;
            float attackRate = 1f / attackTime;

            float decayTime = envelope.DecayTime;
            float decayRate = 1f / decayTime;

            float level = -offTime / envelope.ReleaseTime;

            if (onTime < attackTime)
            {
                level += onTime * attackRate;
            }
            else if (onTime < attackTime + decayTime)
            {
                level += 1 - (onTime - attackTime) * decayRate * (1 - envelope.SustainLevel);
            }
            else
            {
                level += envelope.SustainLevel;
            }

            return Mathf.Max(0, level);
        }

        private float GetNoteEnvelopeValue(MidiControl control, float time)
        {
            uint tick = ConvertSecondToTick(time);
            var pair = GetNoteEventBeforeTick(tick, control.noteFilter);

            if (pair.iOn < 0)
                return 0;
            MidiEvent eOn = events[pair.iOn];

            // Note-on time
            float onTime = ConvertTicksToSecond(eOn.time);
            // Note-off time
            float offTime = pair.iOff < 0 || pair.iOff < pair.iOn ? time : ConvertTicksToSecond(events[pair.iOff].time);

            float envelope = CalculateEnvelope(control.envelope, Mathf.Max(0, offTime - onTime), Mathf.Max(0, time - offTime)); ;

            float velocity = eOn.data2 / 127f;
            return envelope * velocity;
        }


        private float GetNoteCurveValue(MidiControl control, float time)
        {
            uint tick = ConvertSecondToTick(time);
            var pair = GetNoteEventBeforeTick(tick, control.noteFilter);

            if (pair.iOn < 0) return 0;
            MidiEvent eOn = events[pair.iOn];

            //Note-on Time
            float onTime = ConvertTicksToSecond(eOn.time);

            float curve = control.curve.Evaluate(Mathf.Max(Mathf.Max(0, time - onTime)));

            float velocity = eOn.data2 / 127f;
            return curve * velocity;
        }

        private float GetCCValue(MidiControl control, float time)
        {
            uint tick = ConvertSecondToTick(time);
            var pair = GetCCEventIndexAroundTick(tick, control.ccNumber);

            if (pair.i0 < 0) return 0;
            if (pair.i1 < 0) return events[pair.i0].data2 / 127f;

            MidiEvent e0 = events[pair.i0];
            MidiEvent e1 = events[pair.i1];

            float t0 = ConvertTicksToSecond(e0.time);
            float t1 = ConvertTicksToSecond(e1.time);

            float v0 = e0.data2 / 127f;
            float v1 = e0.data2 / 127f;

            return Mathf.Lerp(v0, v1, Mathf.Clamp01((time - t0) / (t1 - t0)));
        }


    }

}
