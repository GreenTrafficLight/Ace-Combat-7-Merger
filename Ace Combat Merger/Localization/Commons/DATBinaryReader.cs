using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ace_Combat_Merger.Localization.Commons
{
    public class DATBinaryReader
    {

        public List<byte> DATBinaryReaderData;

        public int Position = 0;
        public int Length = 0;

        public DATBinaryReader(byte[] data)
        {
            DATBinaryReaderData = data.ToList();
            Length = data.Length;
        }

        public void Seek(int position)
        {
            if (position >= 0)
                Position = position;
            else
                Position = Length - position;
        }

        public byte[] GetBytes(int count)
        {
            byte[] array;
            array = DATBinaryReaderData.GetRange(Position, count).ToArray();
            return array;
        }

        public byte ReadUByte()
        {
            byte value = DATBinaryReaderData[Position];
            Position++;
            return value;
        }

        public sbyte ReadByte()
        {
            sbyte value = (sbyte)DATBinaryReaderData[Position];
            Position++;
            return value;
        }

        public int ReadInt()
        {
            int value = BitConverter.ToInt32(GetBytes(4), 0);
            Position += 4;
            return value;
        }

        public uint ReadUInt()
        {
            uint value = BitConverter.ToUInt32(GetBytes(4), 0);
            Position += 4;
            return value;
        }
        public string ReadString()
        {
            List<byte> StringData = new List<byte>();

            while (true)
            {
                StringData.Add(ReadUByte());
                if (StringData[StringData.Count - 1] == 0)
                    break;
            }

            return Encoding.UTF8.GetString(StringData.ToArray());
        }

        public string ReadString(int length)
        {
            List<byte> StringData = new List<byte>();

            for (int i = 0; i < length; i++)
                StringData.Add(ReadUByte());

            return Encoding.UTF8.GetString(StringData.ToArray());
        }
    }
}
