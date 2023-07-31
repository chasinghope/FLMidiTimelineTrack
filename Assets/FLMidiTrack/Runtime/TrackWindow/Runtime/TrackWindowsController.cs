using Chasing.Midi.Timeline;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Chasing.TrackWindow
{
    public class TrackWindowsController : MonoBehaviour
    {
        public const float offsetxPerSecond = 10f;

        public static TrackWindowsController Instance;



        [SerializeField] MidiFileAsset midiAsset;
        [SerializeField] TextMeshProUGUI popTip;
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
        [SerializeField] MidiNoteFilterScriptable noteFilterConfig;
        public int trackTotalMinutes = 5;
        public int perSecondBeats = 2;

        public SelectSlot selectSlot;

        public Dictionary<MidiOctave, OctaveSetting> octaveDict = new Dictionary<MidiOctave, OctaveSetting>();
        public Dictionary<MidiNote, NoteSetting> noteDict = new Dictionary<MidiNote, NoteSetting>();

        float perSecxPos;

        bool popTipBusy;

        //List<TimePointView> timePointViews = new List<TimePointView>();
        private void Awake()
        {
            if(Instance == null)
                Instance = this;

            timePointViewPrefab.gameObject.SetActive(false);
            trackHeaderPrefab.gameObject.SetActive(false);
            trackPrefab.gameObject.SetActive(false);
            slotPrefab.gameObject.SetActive(false);

            popTip.gameObject.SetActive(false);
        }

        public void Start()
        {
            InitConfig();
            Init();
        }


        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.S))
            {
                //Save
#if UNITY_EDITOR
                if(popTipBusy == false)
                {
                    EditorUtility.SetDirty(midiAsset);
                    StartCoroutine(PopupTip($"保存成功... {midiAsset.name}"));
                }

#endif
            }
        }

        private void InitConfig()
        {
            foreach (OctaveSetting item in noteFilterConfig.octaveSettings)
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

            foreach (NoteSetting item in noteFilterConfig.noteSettings)
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

        private void Init()
        {
            // 计算初始化参数
            perSecxPos = timePointViewPrefab.GetComponent<RectTransform>().sizeDelta.x + timeLayoutGroup.spacing;
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
                if(item)
                {
                    TrackView view = CreateNewTrack(item.name);
                    FillTrackData(view, item);
                }
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
            return perSecxPos * time;
        }

        private void FillTrackData(TrackView parent, MidiAnimationAsset midiAnimation)
        {
            Stack<MidiEvent> eventStack = new Stack<MidiEvent>();
            for (int i = 0; i < midiAnimation.template.events.Length; i++)
            {
                if (midiAnimation.template.events[i].IsNote == false)
                    continue;

                if(eventStack.Count == 0)
                {
                    eventStack.Push(midiAnimation.template.events[i]);
                    continue;
                }
                MidiEvent newEvent = midiAnimation.template.events[i];
                MidiEvent oldEvent = eventStack.Pop();

                if(MidiEvent.IsOnOffComplete(oldEvent, newEvent))
                {
                    CreateSlot(parent.transform, midiAnimation.template, oldEvent, newEvent);
                }

            }
        }

        public void CreateSlot(Transform parent, MidiAnimation midiAnimation, MidiEvent onMidiEvent, MidiEvent offMidiEvent)
        {
            GameObject obj = Instantiate(slotPrefab.gameObject, parent);
            Slot slot = obj.GetComponent<Slot>();

            float x;
            float width;

            float startTime = midiAnimation.ConvertTicksToSecond(onMidiEvent.time);
            float endTime = midiAnimation.ConvertTicksToSecond(offMidiEvent.time);
            x = GetXPos(startTime);
            width = GetXPos(endTime - startTime);

            slot.realRect.anchoredPosition = new Vector2(x, slot.realRect.anchoredPosition.y);
            slot.realRect.sizeDelta = new Vector2(width, slot.realRect.sizeDelta.y);
            slot.Refresh(onMidiEvent, offMidiEvent);
            obj.SetActive(true);
        }


        public IEnumerator PopupTip(string tipDetail)
        {
            popTipBusy = true;
            popTip.gameObject.SetActive(true);
            yield return new WaitForSeconds(2f);
            popTip.gameObject.SetActive(false);
            popTipBusy = false;
        }

    }
}
