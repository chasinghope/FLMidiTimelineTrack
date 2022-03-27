using System.Text;

namespace Chasing.Midi.Timeline
{
    /// <summary>
    ///  MIDI binary data stream reader
    ///  1. 大端格式显示 , 整型低位在高字节 , 整型高位在低字节
    /// </summary>
    sealed class MidiDataStreamReader
    {
        private readonly byte[] mData;
        private readonly StringBuilder mStringBuilder;


        public MidiDataStreamReader(byte[] data)
        {
            mData = data;
            mStringBuilder = new StringBuilder();
        }

        public uint Position { get; private set; }

        public void Advance(uint delta)
        {
            Position += delta;
        }


        #region Reader methods

        public byte PeekByte()
        {
            return mData[Position];
        }

        public byte ReadByte()
        {
            return mData[Position++];
        }

        public string ReadChars(int length)
        {
            mStringBuilder.Clear();
            for (var i = 0; i < length; i++)
                mStringBuilder.Append((char)ReadByte());
            return mStringBuilder.ToString();
        }

        public uint ReadBEUInt32()
        {
            uint b1 = ReadByte();
            uint b2 = ReadByte();
            uint b3 = ReadByte();
            uint b4 = ReadByte();
            return b4 + (b3 << 8) + (b2 << 16) + (b1 << 24);
        }

        public uint ReadBEUInt16()
        {
            uint b1 = ReadByte();
            uint b2 = ReadByte();
            return b2 + (b1 << 8);
        }

        public uint ReadMultiByteValue()
        {
            uint v = 0u;
            while (true)
            {
                uint b = ReadByte();
                v += b & 0x7fu;  
                if (b < 0x80u) break;     //已经到最低位
                v <<= 7;
            }
            return v;
        }

        #endregion
    }
}
