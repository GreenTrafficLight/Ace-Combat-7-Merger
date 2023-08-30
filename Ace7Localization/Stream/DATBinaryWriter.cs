using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ace7Localization.Stream
{
    public class DATBinaryWriter
    {
        public List<byte> DATBinaryWriterData = new List<byte>();

        public int Position = 0;
        public int Length = 0;
        public void WriteUByte(byte value)
        {
            DATBinaryWriterData.Add(value);
            Position++;
            Length++;
        }
        public void WriteInt(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            DATBinaryWriterData.AddRange(bytes);
            Position += 4;
            Length += 4;
        }
        public void WriteString(string value)
        {
            byte[] stringData = Encoding.UTF8.GetBytes(value);

            DATBinaryWriterData.AddRange(stringData);

            Position += stringData.Length;
            Length += stringData.Length;
        }
    }
}
