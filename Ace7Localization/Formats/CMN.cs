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
        // The highest string number in the CMN
        public int MaxStringNumber = 0;

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
        /// Get the full variable string from a child CMN
        /// </summary>
        /// <param name="child">The child CMN that we want the full string</param>
        /// <returns>
        /// Return the full variable string of a child CMN
        /// </returns>
        public static string GetVariable(KeyValuePair<string, CMNString> child)
        {
            string variable = child.Key;
            while (child.Value.Parent.Key != null && child.Value.Parent.Value != null)
            {
                variable = string.Concat(child.Value.Parent.Key, variable);
                child = child.Value.Parent;
            }
            return variable;
        }

        /// <summary>
        /// Add a new variable in the CMN
        /// </summary>
        /// <param name="variable">Variable to be added</param>
        /// <param name="stringNumber">String number tied to this variable</param>
        /// <returns>
        /// Retrun true if the variable didn't exist, false if the variable already exist
        /// </returns>
        public bool AddVariable(string variable, int? stringNumber = null)
        {
            KeyValuePair<string, CMNString> parent = Root.FirstOrDefault(x => variable.StartsWith(x.Key));
            while (parent.Value != null)
            {
                variable = variable.Remove(0, parent.Key.Length);
                KeyValuePair<string, CMNString> child = parent.Value.childrens.FirstOrDefault(x => variable.StartsWith(x.Key));
                if (child.Key == null && child.Value == null)
                {
                    // If the variable doesn't exist
                    if (variable == ""){
                        break;
                    }
                    MaxStringNumber++;
                    MergeVariables(parent, variable, stringNumber);
                    return true;
                }
                parent = child;
            }
            return false;
        }

        /// <summary>
        /// Add a new variable in the CMN
        /// </summary>
        /// <param name="value">Variable to be added</param>
        /// <param name="parent">The parent of the variable that will be added</param>
        /// <param name="stringNumber">String number tied to this variable</param>
        public void AddVariable(string value, KeyValuePair<string, CMNString> parent, int? stringNumber = null)
        {
            MergeVariables(parent, value, stringNumber);
        }
        
        private void MergeVariables(KeyValuePair<string, CMNString> parent, string value, int? stringNumber = null)
        {
            // Index where the added variable will be placed in the parent childrens
            int sortIndex = 0;
            foreach (KeyValuePair<string, CMNString> child in parent.Value.childrens)
            {
                int index = StringUtils.GetCommonSubstringIndex(child.Key, value);
                
                // If there is a node to merge
                if (index != -1)
                {
                    string subString = value.Substring(0, index + 1);

                    // Make a new node for the merged variable
                    KeyValuePair<string, CMNString> mergedCMNString = new KeyValuePair<string, CMNString>(subString, new CMNString(-1, parent));

                    /// Existing node
                    // Substring the variable of the existing node
                    KeyValuePair<string, CMNString> existingCMNString = new KeyValuePair<string, CMNString>(child.Key.Substring(index + 1), new CMNString(child.Value.StringNumber, mergedCMNString));
                    // Add the childrens of the existing node
                    foreach (KeyValuePair<string, CMNString> existingCMNStringChild in child.Value.childrens){
                        existingCMNString.Value.childrens.Add(existingCMNStringChild);
                    }
                    // Add the existing node in the new merged variable
                    mergedCMNString.Value.childrens.Add(existingCMNString);
                    
                    /// New node
                    // Add the new node in the new merged variable
                    CMNString newCMNString = stringNumber == null ? new CMNString(MaxStringNumber, mergedCMNString) : new CMNString(stringNumber.Value, mergedCMNString);
                    // Compare the casing and number with the existing node
                    var comparisonResult = string.Compare(value.Substring(index + 1), child.Key.Substring(index + 1), StringComparison.Ordinal);
                    mergedCMNString.Value.childrens.Insert(comparisonResult < 0 ? 0 : mergedCMNString.Value.childrens.Count, new KeyValuePair<string, CMNString>(value.Substring(index + 1), newCMNString));

                    /// Parent node
                    // Insert the merged variable in the parent node
                    parent.Value.childrens.Insert(sortIndex, new KeyValuePair<string, CMNString>(mergedCMNString.Key, mergedCMNString.Value));
                    // Remove the existing node from the parent node
                    parent.Value.childrens.Remove(child);

                    return;
                }

                // Compare the casing and number of the added variable with the existing childrens
                if (string.Compare(value, child.Key, StringComparison.Ordinal) < 0) {
                    break; // Break the loop if the added variable has a inferior order
                }

                // Increase the index where the added variable will be placed in the parent childrens
                sortIndex++;
            }
            // If there isn't any node to merge
            parent.Value.childrens.Insert(sortIndex, new KeyValuePair<string, CMNString>(value, stringNumber == null ? new CMNString(MaxStringNumber, parent) : new CMNString(stringNumber.Value, parent)));
        }

        /// <summary>
        /// Read the variables inside a binary data
        /// </summary>
        private List<KeyValuePair<string, CMNString>> ReadVariables(DATBinaryReader br, KeyValuePair<string, CMNString> parent, List<KeyValuePair<string, CMNString>> node)
        {
            int count = br.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int nameLength = br.ReadInt32();
                string name = br.ReadString(nameLength);
                int stringNumber = br.ReadInt32();
                // Get the maximum the of the strings contained in the CMN
                if (MaxStringNumber < stringNumber) {
                    MaxStringNumber = stringNumber;
                }
                node.Add(new KeyValuePair<string, CMNString>(name, new CMNString(stringNumber, parent)));
                ReadVariables(br, node.FirstOrDefault(x => x.Key == name), node.FirstOrDefault(x => x.Key == name).Value.childrens);
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