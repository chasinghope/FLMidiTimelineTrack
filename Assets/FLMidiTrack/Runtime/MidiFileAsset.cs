using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chasing.Midi.Timeline
{
    /// <summary>
    /// Midi资产 <ScriptableObject>
    /// 1. .midi文件将转换为该资源
    /// </summary>
    sealed public class MidiFileAsset : ScriptableObject
    {
        public MidiAnimationAsset[] tracks;
    }

}
