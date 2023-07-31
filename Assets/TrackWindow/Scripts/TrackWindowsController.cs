using Chasing.Midi.Timeline;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


namespace Chasing.TrackWindow
{
    public class TrackWindowsController : MonoBehaviour
    {
        public const float offsetxPerSecond = 10f;

        [SerializeField] MidiFileAsset midiAsset;

        [SerializeField] ScrollRect scrollRect;

        [Header("TimePoint")]
        [SerializeField] RectTransform timeTrackRect;
        [SerializeField] HorizontalLayoutGroup timeLayoutGroup;
        [SerializeField] TimePointView timePointViewPrefab;

        [Header("Track")]
        [SerializeField] RectTransform trackOverviewParent;
        [SerializeField] TrackHeaderView trackHeaderPrefab;
        [SerializeField] RectTransform trackViewParent;
        [SerializeField] TrackView trackPrefab;
        [SerializeField] Slot slotPrefab;


        [Header("Config")]
        public int trackTotalMinutes = 5;
        public int perSecondBeats = 2;

        float perSecxPos;
         
        //List<TimePointView> timePointViews = new List<TimePointView>();
        private void Awake()
        {
            timePointViewPrefab.gameObject.SetActive(false);
            trackHeaderPrefab.gameObject.SetActive(false);
            trackPrefab.gameObject.SetActive(false);
            slotPrefab.gameObject.SetActive(false);
        }

        public void Start()
        {
            Init();
        }


        private void Init()
        {
            // 计算初始化参数
            perSecxPos = slotPrefab.GetComponent<RectTransform>().sizeDelta.x + timeLayoutGroup.spacing;
            perSecxPos *= 2;

            // 初始化时间部分
            Vector3 pos = default;
            int secondCounter = 0;
            for (int i = 0; i <= trackTotalMinutes * 60 * perSecondBeats; i++)   //180     0.5s
            {
                TimePointData timePointData = new TimePointData();
                timePointData.showTimePoint = i % (2 * perSecondBeats) == 0;
                timePointData.isHigh = i % (1 * perSecondBeats) == 0;

                if (secondCounter == perSecondBeats)
                    secondCounter = 0;
                timePointData.isFulSecond = secondCounter == 0;
                timePointData.timePoint = i / perSecondBeats;

                secondCounter++;

                GameObject obj = GameObject.Instantiate(timePointViewPrefab.gameObject, pos, Quaternion.identity, timeTrackRect);
                obj.SetActive(true);
                TimePointView view = obj.GetComponent<TimePointView>();
                view.Refresh(timePointData);
            }


            //for (int i = 0; i < midiAsset.tracks.Length; i++)
            //{
                
            //}
            foreach (var item in midiAsset.tracks)
            {
                TrackView view = CreateNewTrack(item.name);
                FillTrackData(view, item);
            }


        }


        private TrackView CreateNewTrack(string trackName)
        {
            TrackView trackView;
            GameObject obj = Instantiate(trackPrefab.gameObject, trackViewParent);
            trackView = obj.GetComponent<TrackView>();
            obj.SetActive(true);

            TrackHeaderView headView = Instantiate(trackHeaderPrefab.gameObject, trackOverviewParent).GetComponent<TrackHeaderView>();
            headView.SetName(trackName);
            headView.gameObject.SetActive(true);
            return trackView;
        }


        private float GetXPos(float time)
        {
            return 0;
        }

        private void FillTrackData(TrackView parent, MidiAnimationAsset midiAnimation)
        {
            Stack<MidiEvent> eventStack = new Stack<MidiEvent>();
            for (int i = 0; i < midiAnimation.template.events.Length; i++)
            {
                if(eventStack.Count == 0)
                {
                    eventStack.Push(midiAnimation.template.events[i]);
                    continue;
                }
                MidiEvent newEvent = midiAnimation.template.events[i];
                MidiEvent oldEvent = eventStack.Pop();

                if(MidiEvent.IsOnOffComplete(oldEvent, newEvent))
                {
                    CreateSlot(parent.transform, oldEvent, newEvent);
                }

            }
        }


        public void CreateSlot(Transform parent, MidiEvent onMidiEvent, MidiEvent offMidiEvent)
        {
            GameObject obj = Instantiate(slotPrefab.gameObject, parent);
            Slot slot = obj.GetComponent<Slot>();

            float x = 0;
            float width = 0;

            x = GetXPos(onMidiEvent.time);
            width = GetXPos(offMidiEvent.time - onMidiEvent.time);

            slot.realRect.anchoredPosition = new Vector2(x, slot.realRect.anchoredPosition.y);
            slot.realRect.sizeDelta = new Vector2(width, slot.realRect.sizeDelta.y);
            obj.SetActive(true);
        }


    }
}
