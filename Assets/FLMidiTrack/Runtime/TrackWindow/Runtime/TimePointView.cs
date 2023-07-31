using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Chasing.TrackWindow
{
    public class TimePointView : MonoBehaviour
    {
        [SerializeField] Image timeImg;
        [SerializeField] RectTransform timeImgRect;
        [SerializeField] TextMeshProUGUI timePointText;
        [SerializeField] Image vlineImg;

        public void Refresh(TimePointData timePointData)
        {
            timeImgRect.sizeDelta = new Vector2(timeImgRect.sizeDelta.x, timePointData.isHigh && timePointData.isFulSecond ? 45f : 20f);
            int minute, second;
            minute = timePointData.timePoint / 60;
            second = timePointData.timePoint % 60;
            timePointText.text = string.Format("{0:00}:{1:00}", minute, second);
            timePointText.gameObject.SetActive(timePointData.showTimePoint && timePointData.isFulSecond);
            vlineImg.gameObject.SetActive(timePointData.isHigh);
        }
    }

    public class TimePointData
    {
        public int timePoint;
        public bool isFulSecond;
        public bool showTimePoint;
        public bool isHigh;
    }


}

