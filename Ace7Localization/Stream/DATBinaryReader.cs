using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ace7Localization.Stream
{
    public class DATBinaryReader : BinaryReader
    {

        public byte[] DATBinaryReaderData;

        public int Position = 0;
        public int Length = 0;

        public DATBinaryReader(byte[] data) : base(new MemoryStream(data))
        {
            DATBinaryReaderData = data;
            Length = data.Length;
        }

        public void Seek(int position)
        {
            if (position >= 0)
                Position = position;
            else
                Position = Length - position;
        }

        public override byte[] ReadBytes(int count)
        {
            byte[] array = new byte[count];
            Array.Copy(DATBinaryReaderData, Position, array, 0, count);
            return array;
        }

        public override byte ReadByte()
        {
            byte value = DATBinaryReaderData[Position];
            Position++;
            return value;
        }

        public override sbyte ReadSByte()
        {
            sbyte value = (sbyte)DATBinaryReaderData[Position];
            Position++;
            return value;
        }

        public override int ReadInt32()
        {
            int value = BitConverter.ToInt32(ReadBytes(4), 0);
            Position += 4;
            return value;
        }

        public override uint ReadUInt32()
        {
            uint value = BitConverter.ToUInt32(ReadBytes(4), 0);
            Position += 4;
            return value;
        }
        public override string ReadString()
        {
            List<byte> StringData = new List<byte>();

            while (true)
            {
                StringData.Add(ReadByte());
                if (StringData[StringData.Count - 1] == 0)
                    break;
            }

            return Encoding.UTF8.GetString(StringData.ToArray());
        }

        public string ReadString(int length)
        {
            byte[] StringData = new byte[length];

            for (int i = 0; i < length; i++)
                StringData[i] = ReadByte();

            return Encoding.UTF8.GetString(StringData);
        }
    }
}
