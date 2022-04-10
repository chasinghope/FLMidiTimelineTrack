using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Chasing.Midi.Timeline
{
    public class BlockNoteFilter : MonoBehaviour
    {
        public SpriteRenderer mSpriteRenderer;
        public TextMeshPro mTextMeshPro;

        public void Set(string note, Color fontColor, Color bgColor)
        {
            mSpriteRenderer.color = bgColor;
            mTextMeshPro.color = fontColor;
            mTextMeshPro.text = note;
        }

        public void Set(string note, Color fontColor, Color bgColor, float alpha)
        {
            fontColor.a = alpha;
            bgColor.a = alpha;
            Set(note, fontColor, bgColor);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}