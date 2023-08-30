using Ace7Localization.Stream;
using Ace7Localization.Utils;

namespace Ace7Localization.Formats
{
    public class DAT
    {
        public List<string> Strings = new List<string>();

        public DAT()
        {
        }

        public DAT(string path)
        {
            Read(path);
        }

        public DAT(string path, string letter)
        {
            Read(path, letter);
        }

        public static byte[] Crypt(byte[] data, uint size)
        {
            uint ebx = 0;
            uint edi = size;
            uint position = 0;
            ulong r15 = 0;

            while (position < data.Length)
            {
                uint ecx = ebx * 8;
                position++;
                ecx ^= ebx;
                uint eax = ebx + ebx;
                ecx = ~ecx;
                edi = edi + edi * 4;
                ecx >>= 7;
                r15++;
                ecx &= 1;
                edi++;
                ebx = ecx;
                ebx |= eax;
                eax = (byte)ebx;
                eax += (byte)edi;
                data[r15 - 1] ^= (byte)eax;
            }

            return data;
        }

        public void Read(string filepath)
        {
            byte[] data = File.ReadAllBytes(filepath);

            uint size = (uint)data.Length + Path.GetFileNameWithoutExtension(filepath)[0] - 65;
            data = Crypt(data, size);
            data = CompressionHandler.Decompress(data);

            DATBinaryReader br = new DATBinaryReader(data);

            ReadStrings(br);
        }

        public void Read(string filepath, string letter)
        {
            byte[] data = File.ReadAllBytes(filepath);

            uint size = (uint)data.Length + letter[0] - 65;
            data = Crypt(data, size);
            data = CompressionHandler.Decompress(data);

            DATBinaryReader br = new DATBinaryReader(data);

            ReadStrings(br);
        }

        public void Write(string filepath)
        {
            DATBinaryWriter bw = new DATBinaryWriter();

            WriteStrings(bw);

            byte[] data = bw.DATBinaryWriterData.ToArray();

            data = CompressionHandler.Compress(data);
            uint size = (uint)data.Length + Path.GetFileNameWithoutExtension(filepath)[0] - 65;
            data = Crypt(data, size);

            File.WriteAllBytes(filepath, data);
        }

        private void ReadStrings(DATBinaryReader reader)
        {
            int index = 0;
            while (reader.Position < reader.Length)
            {
                Strings.Add(reader.ReadString());
                index++;
            }

        }

        private void WriteStrings(DATBinaryWriter writer)
        {
            foreach (string s in Strings)
                writer.WriteString(s);
        }
    }
}
