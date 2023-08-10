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
using System.Collections;

namespace Ace_Combat_Merger.Localization.Formats
{
    public class CMN
    {
        public class CMNString
        {
            public int StringNumber = -1;
            public Dictionary<string, CMNString> childrens = new Dictionary<string, CMNString>();

            public CMNString()
            {

            }

            public CMNString(int stringNumber)
            {
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

        
        public Dictionary<string, CMNString> Root = new Dictionary<string, CMNString>();
        public void Read(string filepath)
        {
            byte[] data = File.ReadAllBytes(filepath);

            data = DAT.Crypt(data, (uint)data.Length);
            data = DATCompression.Decompress(data);

            DATBinaryReader br = new DATBinaryReader(data);

            Root = ReadVariables(br, Root);
            AddString("Aircraft_Name_f35ra");
        }

        public void Write(string filepath)
        {

        }

        public void AddString(string value)
        {
            /*var parent = Root.FirstOrDefault(x => value.StartsWith(x.Key));
            KeyValuePair<string, CMNString> test = new KeyValuePair<string, CMNString>();
            while (parent.Value != null)
            {
                value = value.Remove(0, parent.Key.Length);
                test = parent;
                parent = parent.Value.childrens.FirstOrDefault(x => value.StartsWith(x.Key));
            }

            // TO DO : Check if there is a string that start with the same substring
            var test2 = test.Value.childrens.Keys.Where(s => s.StartsWith(value.Substring(0, GetCommonSubstringIndex(s, value))));
            test.Value.childrens.Add(value, new CMNString(-1));*/
            
        }


        private Dictionary<string, CMNString> ReadVariables(DATBinaryReader br, Dictionary<string, CMNString> parent)
        {
            int count = br.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int nameLength = br.ReadInt();
                string name = br.ReadString(nameLength);
                int stringNumber = br.ReadInt();
                parent.Add(name, new CMNString(stringNumber));
                ReadVariables(br, parent[name].childrens);
            }
            return parent;
        }

        private void WriteVariables(DATBinaryWriter bw, CMNString cmnString)
        {

        }
        private int GetCommonSubstringIndex(string str1, string str2, int startIndex = 0)
        {
            int index = -1;

            for (int i = startIndex; i < Math.Min(str1.Length, str2.Length); i++)
            {
                if (str1[i] != str2[i])
                {
                    return index;
                }
                index++;
            }
            return index;
        }
    }
}
