using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Ace7Localization.Utils;
using Ace7Localization.Stream;

namespace Ace7Localization.Formats
{
    public class CMN
    {
        public class CMNString
        {
            public int StringNumber = -1;
            public KeyValuePair<string, CMNString> Parent = new KeyValuePair<string, CMNString>();
            public List<KeyValuePair<string, CMNString>> childrens = new List<KeyValuePair<string, CMNString>>();

            public CMNString()
            {

            }

            public CMNString(int stringNumber)
            {
                StringNumber = stringNumber;
            }

            public CMNString(int stringNumber, KeyValuePair<string, CMNString> parent)
            {
                StringNumber = stringNumber;
                Parent = parent;
            }
        }

        public CMN(string path)
        {
            Read(path);
        }

        public List<KeyValuePair<string, CMNString>> Root = new List<KeyValuePair<string, CMNString>>();
        public int StringsCount = 0;

        /// <summary>
        /// Read a CMN file
        /// </summary>
        /// <param name="path">Path of the CMN file</param>
        public void Read(string path)
        {
            byte[] data = File.ReadAllBytes(path);

            data = DAT.Crypt(data, (uint)data.Length);
            data = CompressionHandler.Decompress(data);

            DATBinaryReader br = new DATBinaryReader(data);

            Root = ReadVariables(br, new KeyValuePair<string, CMNString>(null, null), Root);
            StringsCount++;
        }

        /// <summary>
        /// Write a CMN file
        /// </summary>
        /// <param name="path">Output path for the written CMN file</param>
        public void Write(string path)
        {
            DATBinaryWriter bw = new DATBinaryWriter();

            bw.WriteInt(Root.Count);
            foreach (var children in Root)
                WriteVariables(bw, children);

            byte[] data = bw.DATBinaryWriterData.ToArray();

            data = CompressionHandler.Compress(data);
            uint size = (uint)data.Length;
            data = DAT.Crypt(data, size);

            File.WriteAllBytes(path, data);
        }

        /// <summary>
        /// Return a variable contained in the CMN
        /// </summary>
        /// <param name="child"></param>
        /// <returns>
        /// A string representing the variable contained in the CMN
        /// </returns>
        public string GetVariable(KeyValuePair<string, CMNString> child)
        {
            string variable = child.Key;
            while (child.Value.Parent.Key != null && child.Value.Parent.Value != null)
            {
                variable = string.Concat(child.Value.Parent.Key, variable);
                child = child.Value.Parent;
            }
            return variable;
        }

        public void AddVariable(string value, int? stringNumber = null)
        {
            KeyValuePair<string, CMNString> parent = Root.FirstOrDefault(x => value.StartsWith(x.Key));
            while (parent.Value != null)
            {
                value = value.Remove(0, parent.Key.Length);
                KeyValuePair<string, CMNString> child = parent.Value.childrens.FirstOrDefault(x => value.StartsWith(x.Key));
                if (child.Key == null && child.Value == null)
                {
                    break;
                }
                parent = child;
            }

            MergeVariables(parent, value, stringNumber);
        }

        public void AddVariable(string value, KeyValuePair<string, CMNString> parent, int? stringNumber = null)
        {
            MergeVariables(parent, value, stringNumber);
        }
        
        private void MergeVariables(KeyValuePair<string, CMNString> parent, string value, int? stringNumber = null)
        {
            string subString;
            int sortIndex = 0;
            foreach (KeyValuePair<string, CMNString> child in parent.Value.childrens)
            {
                int index = StringUtils.GetCommonSubstringIndex(child.Key, value);
                if (index != -1)
                {
                    subString = value.Substring(0, index + 1);

                    // Make a new node for the merged variable
                    KeyValuePair<string, CMNString> newCMNString = new KeyValuePair<string, CMNString>(subString, new CMNString(-1, parent));

                    // Add the existing node in the new merged variable
                    newCMNString.Value.childrens.Add(new KeyValuePair<string, CMNString>(child.Key.Substring(index + 1), child.Value));
                    // Add the new node in the new merged variable
                    if (stringNumber == null) {
                        newCMNString.Value.childrens.Add(new KeyValuePair<string, CMNString>(value.Substring(index + 1), new CMNString(StringsCount++)));
                    }
                    else {
                        newCMNString.Value.childrens.Add(new KeyValuePair<string, CMNString>(value.Substring(index + 1), new CMNString(stringNumber.Value)));
                    }
                    
                    // Insert the merged variable in the parent node
                    parent.Value.childrens.Insert(sortIndex, new KeyValuePair<string, CMNString>(newCMNString.Key, newCMNString.Value));
                    // Remove the existing node from the parent node
                    parent.Value.childrens.Remove(child);

                    break;
                }
                sortIndex++;
            }
        }

        /// <summary>
        /// Read the variables inside a binary data
        /// </summary>
        private List<KeyValuePair<string, CMNString>> ReadVariables(DATBinaryReader br, KeyValuePair<string, CMNString> parent, List<KeyValuePair<string, CMNString>> node)
        {
            int count = br.ReadInt();
            for (int i = 0; i < count; i++)
            {
                int nameLength = br.ReadInt();
                string name = br.ReadString(nameLength);
                int stringNumber = br.ReadInt();
                // Get the maximum the of the strings contained in the CMN
                if (StringsCount < stringNumber)
                {
                    StringsCount = stringNumber;
                }
                node.Add(new KeyValuePair<string, CMNString>(name, new CMNString(stringNumber, parent)));
                ReadVariables(br, node.SingleOrDefault(x => x.Key == name), node.SingleOrDefault(x => x.Key == name).Value.childrens);
            }
            return node;
        }

        /// <summary>
        /// Write the variables inside a binary data
        /// </summary>
        /// <param name="bw"></param>
        /// <param name="parent"></param>
        private void WriteVariables(DATBinaryWriter bw, KeyValuePair<string, CMNString> parent)
        {
            bw.WriteInt(parent.Key.Length);
            bw.WriteString(parent.Key);
            bw.WriteInt(parent.Value.StringNumber);
            bw.WriteInt(parent.Value.childrens.Count);
            foreach (KeyValuePair<string, CMNString> child in parent.Value.childrens)
                WriteVariables(bw, child);
        }
    }
}