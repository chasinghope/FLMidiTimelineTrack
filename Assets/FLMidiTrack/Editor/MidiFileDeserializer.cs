using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chasing.Midi.Timeline
{
    /// <summary>
    /// .midi文件反序列化器
    /// </summary>
    public class MidiFileDeserializer
    {
        private static MidiAnimationAsset ReadTrack(MidiDataStreamReader reader, uint tpqn)
        {
            // 轨道头          MTrk的ASCII码
            string chunkIdent = reader.ReadChars(4);
            if (chunkIdent != "MTrk")
                throw new FormatException("找不到跟踪块：MTrk  请检查正在反序列的midi文件是否存在轨道");

            // 轨道长度        每个轨道开始的标志是 MTrk , 后面的 4 字节就是轨道的长度 ;
            uint chunkEnd = reader.ReadBEUInt32();
            chunkEnd += reader.Position;

            // MIDI event sequence  
            List<MidiEvent> events = new List<MidiEvent>();
            uint ticks = 0u;
            byte stat = 0;

            while (reader.Position < chunkEnd)
            {
                // Delta time   midi 中的音符 , 事件 的时间间隔 , 都是通过 delta-time 体现的
                ticks += reader.ReadMultiByteValue();

                // Status byte
                if ((reader.PeekByte() & 0x80u) != 0)
                    stat = reader.ReadByte();

                if (stat == 0xffu)
                {
                    // 0xff: Meta event (unused)
                    reader.Advance(1);
                    reader.Advance(reader.ReadMultiByteValue());
                }
                else if (stat == 0xf0u)
                {
                    // 0xf0: SysEx (unused)
                    while (reader.ReadByte() != 0xf7u) { }
                }
                else
                {
                    // MIDI event
                    var b1 = reader.ReadByte();
                    var b2 = (stat & 0xe0u) == 0xc0u ? (byte)0 : reader.ReadByte();
                    events.Add(new MidiEvent
                    {
                        time = ticks,
                        status = stat,
                        data1 = b1,
                        data2 = b2
                    });
                }
            }

            // Quantize duration with bars.
            uint bars = (ticks + tpqn * 4 - 1) / (tpqn * 4);

            // Asset instantiation
            MidiAnimationAsset asset = ScriptableObject.CreateInstance<MidiAnimationAsset>();
            asset.template.tempo = 120;
            asset.template.duration = bars * tpqn * 4;
            asset.template.ticksPerQuarterNote = tpqn;
            asset.template.events = events.ToArray();
            return asset;
        }


        #region Public

        public static MidiFileAsset Load(byte[] data)
        {
            var reader = new MidiDataStreamReader(data);

            // 头标识        0 ~ 3 字节 ,   " MThd " 字符串 ASCII 码 , 这是 mid 文件的标识 
            string headerIdent = reader.ReadChars(4);
            if (headerIdent != "MThd")
                throw new FormatException("找不到跟踪块：MTrk  请检查正在反序列的文件是否是.midi文件");

            // 头长度        4 ~ 7 字节 , 数据表示 mid 文件文件头长度
            uint headerLength = reader.ReadBEUInt32();
            if (headerLength != 6u)
                throw new FormatException("头长度必须是 6");

            // MIDI文件类型  8 ~ 9 字节 , 表示 mid 文件的格式
            // 0 : mid 文件只有一条轨道, 所有的通道都在一条轨道中;
            // 1 : mid 文件有多个音轨, 并且是同步的, 即所有的轨道同时播放;
            // 2 : mid 文件有多个音轨, 不同步;
            uint midiType = reader.ReadBEUInt16();
            //reader.Advance(2);

            // 轨道个数      10 ~ 11 字节 , 表示 MIDI 轨道个数
            uint trackCount = reader.ReadBEUInt16();

            // 基本时间      12 ~ 13 字节 , 用于指定基本时间
            uint tpqn = reader.ReadBEUInt16();
            if ((tpqn & 0x8000u) != 0)
                throw new FormatException("SMPTE time code is not supported.");

            // 轨道解析开始
            MidiAnimationAsset[] tracks = new MidiAnimationAsset[trackCount];
            for (int i = 0; i < trackCount; i++)
                tracks[i] = ReadTrack(reader, tpqn);

            // MIDI资源生成
            MidiFileAsset asset = ScriptableObject.CreateInstance<MidiFileAsset>();
            asset.tracks = tracks;
            return asset;
        }

        #endregion
    }

}
