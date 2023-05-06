using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Ace_Combat_Merger.Localization.Compression;
using Ace_Combat_Merger.Localization.Commons;
using System.ComponentModel.Design.Serialization;

namespace Ace_Combat_Merger.Localization.Formats
{
    public class CMN
    {
        public class CMNString
        {
            public string VariableName = "";
            public int StringNumber = -1;
            public List<CMNString> childrens = new List<CMNString>();

            public CMNString()
            {

            }

            public CMNString(string name, int stringNumber)
            {
                VariableName = name;
                StringNumber = stringNumber;
            }
        }

        public CMN()
        {

        }

        public CMN(string path)
        {
            Read(path);
        }

        
        public CMNString Root = new CMNString();
        public void Read(string filepath)
        {
            byte[] data = File.ReadAllBytes(filepath);

            data = DAT.Crypt(data, (uint)data.Length);
            data = DATCompression.Decompress(data);

            DATBinaryReader br = new DATBinaryReader(data);

            Root = ReadVariables(br, Root);
        }

        public void Write(string filepath)
        {
            DATBinaryWriter bw = new DATBinaryWriter();

            bw.WriteInt(Root.childrens.Count);
            foreach (CMNString children in Root.childrens)
                WriteVariables(bw, children);

            byte[] data = bw.DATBinaryWriterData.ToArray();

            data = DATCompression.Compress(data);
            uint size = (uint)data.Length;
            data = DAT.Crypt(data, size);

            File.WriteAllBytes(filepath, data);
        }

        public void AddString(string value)
        {

        }


        private CMNString ReadVariables(DATBinaryReader br, CMNString parent)
        {
            int count = br.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int nameLength = br.ReadInt();
                CMNString cmnString = new CMNString(br.ReadString(nameLength), br.ReadInt());
                parent.childrens.Add(cmnString);
            }
            return parent;
        }

        private void WriteVariables(DATBinaryWriter bw, CMNString cmnString)
        {
            bw.WriteInt(cmnString.VariableName.Length);
            bw.WriteString(cmnString.VariableName);
            bw.WriteInt(cmnString.StringNumber);
            bw.WriteInt(cmnString.childrens.Count);
            foreach (CMNString children in cmnString.childrens)
                WriteVariables(bw, children);
        }
    }
}
