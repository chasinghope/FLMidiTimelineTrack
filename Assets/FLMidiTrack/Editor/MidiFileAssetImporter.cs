using System.Collections;
using System.Collections.Generic;
using UnityEditor.AssetImporters;
using UnityEngine;
using System.IO;

namespace Chasing.Midi.Timeline
{
    /// <summary>
    /// Custom importer for .mid files
    /// </summary>
    [ScriptedImporter(1, "mid")]
    sealed class MidiFileAssetImporter : ScriptedImporter
    {
        /// <summary>
        /// ����
        /// </summary>
        [SerializeField] private float mTempo = 120f;

        public override void OnImportAsset(AssetImportContext ctx)
        {
            string fileName = Path.GetFileNameWithoutExtension(ctx.assetPath);
            byte[] buffer = File.ReadAllBytes(ctx.assetPath);

            MidiFileAsset midiAsset = MidiFileDeserializer.Load(buffer);
            midiAsset.name = fileName;
            ctx.AddObjectToAsset("MidiFileAsset", midiAsset);
            ctx.SetMainObject(midiAsset);

            for (int i = 0, index = 0; i < midiAsset.tracks.Length; i++)
            {
                MidiAnimationAsset track = midiAsset.tracks[i];
  
                track.template.tempo = mTempo;
                if(track.template.events.Length > 0)
                {
                    index++;
                    track.name = fileName + "_t" + index;
                    ctx.AddObjectToAsset(track.name, track);
                }

            }
        }
    }
}