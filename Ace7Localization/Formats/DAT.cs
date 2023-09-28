using Ace7Localization.Stream;
using Ace7Localization.Utils;
using System.IO;

namespace Ace7Localization.Formats
{
    public class DAT
    {
        public char Letter;
        public List<string> Strings = new List<string>();

        /// <summary>
        /// Read a DAT file from a path
        /// </summary>
        /// <param name="path">The path of the DAT file</param>
        /// <param name="letter">The name of the DAT file</param>
        public DAT(string path, char letter)
        {
            Letter = letter;
            Read(path, letter);
        }

        public DAT(byte[] data, char letter)
        {
            Letter = letter;
            Read(data, letter);
        }

        /// <summary>
        /// Crypt/Decrypt a DAT file
        /// </summary>
        /// <param name="data">Buffer of the DAT file</param>
        /// <param name="size">Size of the DAT file</param>
        /// <returns></returns>
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

        /// <summary>
        /// Read a DAT file from a path
        /// </summary>
        /// <param name="filepath">The path of the DAT file</param>
        /// <param name="letter">The letter of the DAT file</param>
        public void Read(string filepath, char letter)
        {
            byte[] data = File.ReadAllBytes(filepath);

            uint size = ((uint)data.Length + letter - 65);
            
            data = Crypt(data, size);
            data = CompressionHandler.Decompress(data);

            DATBinaryReader br = new DATBinaryReader(data);

            ReadStrings(br);
        }

        public void Read(byte[] data, char letter)
        {
            uint size = ((uint)data.Length + letter - 65);

            data = Crypt(data, size);
            data = CompressionHandler.Decompress(data);

            DATBinaryReader br = new DATBinaryReader(data);

            ReadStrings(br);
        }

        /// <summary>
        /// Write a DAT file to a path
        /// </summary>
        /// <param name="filepath">The path for the new DAT file</param>
        /// <param name="letter">The leffer for the DAT file</param>
        public void Write(string filepath, char letter)
        {
            DATBinaryWriter bw = new DATBinaryWriter();

            WriteStrings(bw);

            byte[] data = bw.DATBinaryWriterData.ToArray();

            data = CompressionHandler.Compress(data);
            uint size = ((uint)data.Length + letter - 65);

            data = Crypt(data, size);

            File.WriteAllBytes(filepath + letter + ".dat", data);
        }

        /// <summary>
        /// Read strings inside a DAT file
        /// </summary>
        /// <param name="reader"></param>
        private void ReadStrings(DATBinaryReader reader)
        {
            int index = 0;
            while (reader.Position < reader.Length)
            {
                Strings.Add(reader.ReadString());
                index++;
            }

        }

        /// <summary>
        /// Write strings inside a DAT file
        /// </summary>
        /// <param name="writer"></param>
        private void WriteStrings(DATBinaryWriter writer)
        {
            foreach (string s in Strings)
                writer.WriteString(s);
        }
    }
}
