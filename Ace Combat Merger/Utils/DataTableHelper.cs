using CUE4Parse.FileProvider.Objects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Objects;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes;

namespace Ace_Combat_Merger.Utils
{
    public static class DataTableHelper
    {
        public static List<string> GetRowNames(DataTableExport dataTableExport)
        {
            return null;
        }

        public static bool ContainsRowName(DataTableExport dataTableExport, string name)
        {
            return dataTableExport.Table.Data.Any(row => row.Name.ToString().Equals(name));
        }

        public static StructPropertyData GetRowStruct(DataTableExport dataTableExport, string name)
        {
            return dataTableExport.Table.Data[dataTableExport.Table.Data.FindIndex(row => row.Name.ToString().Equals(name))];
        }

        public static StructPropertyData GetRowStruct(DataTableExport dataTableExport, int index)
        {
            return dataTableExport.Table.Data[index];
        }

        public static bool TableContainsValue(UDataTable table, string name, int? value)
        {
            if (table == null)
                return false;
            return table.Data.Any(structPropertyData => StructContainsValue(structPropertyData, name, value));
        }

        /// <summary>
        /// Check if a StructPropertyData contains a value in the specified name
        /// </summary>
        /// <param name="structPropertyData">The StructPropertyData that the function will read from</param>
        /// <param name="name"> The name where we want to check the value</param>
        /// <param name="value">The value that we want to check</param>
        public static bool StructContainsValue(StructPropertyData structPropertyData, string name, string value)
        {
            if (structPropertyData == null)
                return false;
            return structPropertyData.Value.Any(propertyData => PropertyDataContainsValue(propertyData, name, value));
        }

        /// <summary>
        /// Check if a StructPropertyData contains a value in the specified name
        /// </summary>
        /// <param name="structPropertyData">The StructPropertyData that the function will read from</param>
        /// <param name="name"> The name where we want to check the value</param>
        /// <param name="value">The value that we want to check</param>
        public static bool StructContainsValue(StructPropertyData structPropertyData, string name, int? value)
        {
            if (structPropertyData == null)
                return false;
            return structPropertyData.Value.Any(propertyData => PropertyDataContainsValue(propertyData, name, value));
        }

        public static bool PropertyDataContainsValue(PropertyData propertyData, string name, string value)
        {
            if (propertyData == null)
                return false;
            return propertyData.Name.ToString().Equals(name) && propertyData.ToString().Equals(value);
        }

        public static bool PropertyDataContainsValue(PropertyData propertyData, string name, int? value)
        {
            if (propertyData == null)
                return false;
            return propertyData.Name.ToString().Equals(name) && propertyData.ToString().TryGetInt() == value;
        }

        public static void AddToNameMap(UAsset exportAsset, PropertyData propertyData)
        {
            if (propertyData.GetType().Name == "SoftObjectPropertyData")
            {
                var FSoftObjectPathData = ((SoftObjectPropertyData)propertyData).Value;
                if (!exportAsset.ContainsNameReference(FSoftObjectPathData.AssetPathName.Value))
                {
                    // Update Name Map Index
                    int nameIndex = exportAsset.AddNameReference(FSoftObjectPathData.AssetPathName.Value);
                    ((SoftObjectPropertyData)propertyData).Value = new FSoftObjectPath(new FName(exportAsset, nameIndex), FSoftObjectPathData.SubPathString);
                }
            }
        }

        public static int? TryGetInt(this string item)
        {
            int i;
            bool success = int.TryParse(item, out i);
            return success ? i : null;
        }

        public static int? TryParseNullable(string val)
        {
            int outValue;
            return int.TryParse(val, out outValue) ? outValue : null;
        }

        public static uint GetFileSignature(string path)
        {
            byte[] buffer = new byte[4];
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                var bytes_read = fs.Read(buffer, 0, buffer.Length);
                fs.Close();
            }
            return BitConverter.ToUInt32(buffer, 0);
        }

        public static void GetPackageFromPaks(string packageName, string exportPath, List<IReadOnlyDictionary<string, GameFile>> paksFiles)
        {
            foreach (var pakFiles in paksFiles)
            {
                if (pakFiles.ContainsKey(packageName + ".uasset"))
                {
                    File.WriteAllBytes(exportPath + ".uasset", pakFiles[packageName + ".uasset"].Read());
                }
                if (pakFiles.ContainsKey(packageName + ".uexp"))
                {
                    File.WriteAllBytes(exportPath + ".uexp", pakFiles[packageName + ".uexp"].Read());
                }
                if (pakFiles.ContainsKey(packageName + ".dat"))
                {
                    File.WriteAllBytes(exportPath + ".dat", pakFiles[packageName + ".dat"].Read());
                }
            }
        }
    }
}
