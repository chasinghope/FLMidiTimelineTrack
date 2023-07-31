using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Chasing.TrackWindow
{
    public class TrackHeaderView : MonoBehaviour
    {
        [SerializeField] Text trackNameText;

        internal void SetName(string trackName)
        {
            trackNameText.text = trackName;
        }
    }
}
