using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace Chasing.Midi.Timeline
{
    public class CalcTime : MonoBehaviour
    {
        public TextMeshPro mTextMeshPro;
        public int Number = 4;


        private void Awake()
        {
            gameObject.SetActive(true);
        }


        public void SetNumber()
        {
            //Debug.Log($"SetNumber() {Number}");
            Number--;
            if (Number == 0)
                gameObject.SetActive(false);
            mTextMeshPro.text = Number.ToString();
        }
    }

}