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
        /// ½Ú×à
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

            for (int i = 0; i < midiAsset.tracks.Length; i++)
            {
                MidiAnimationAsset track = midiAsset.tracks[i];
                track.name = fileName + "_Track" + i;
                track.template.tempo = mTempo;
                ctx.AddObjectToAsset(track.name, track);
            }
        }
    }
}