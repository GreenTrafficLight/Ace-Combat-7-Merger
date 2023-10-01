using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI;
using UAssetAPI.UnrealTypes;
using CUE4Parse.FileProvider.Objects;
using Ace_Combat_Merger.Utils;

namespace Ace_Combat_Merger
{
    public class DataTableMerger
    {
        public void MergeDataTables(UAsset gameUasset, UAsset exportUasset, UAsset modUasset, StateDataTable stateDataTable, DictionaryAC7DataTable dictionaryDataTable, IReadOnlyDictionary<string, GameFile> pakModsFile, ModManager modManager)
        {
            for (int i = 0; i < gameUasset.Exports.Count; i++)
            {
                StateDataTable exportStateDataTable = stateDataTable.StateDataTableChild[i];

                Export gameAssetExport = gameUasset.Exports[i];
                FName gameAssetExportClassTypeName = gameAssetExport.GetExportClassType();
                string gameAssetExportClassType = gameAssetExportClassTypeName.Value.Value;

                Export exportAssetExport = exportUasset.Exports[i];

                Export modAssetExport = modUasset.Exports[i];
                FName modAssetExportClassTypeName = modAssetExport.GetExportClassType();
                string modAssetExportClassType = modAssetExportClassTypeName.Value.Value;

                switch (gameAssetExportClassType)
                {
                    default:
                        if (gameAssetExportClassType.EndsWith("DataTable") && modAssetExportClassType.EndsWith("DataTable"))
                        {
                            // Check each Row
                            DataTableExport gameAssetTableExport = (DataTableExport)gameAssetExport;
                            DataTableExport exportAssetTableExport = (DataTableExport)exportAssetExport;
                            DataTableExport modAssetTableExport = (DataTableExport)modAssetExport;

                            for (int j = 0; j < modAssetTableExport.Table.Data.Count; j++)
                            {
                                // Check each row of Row of the modded asset
                                StructPropertyData modAssetRow = modAssetTableExport.Table.Data[j];

                                // Check if rowName is already in the export asset datatable
                                string rowName = modAssetRow.Name.Value.ToString() + "_" + (modAssetRow.Name.Number - 1).ToString();
                                // If it's already in the export asset datatable, change the rowName
                                // The rowName is the follow : Row_(Number)
                                while (dictionaryDataTable.RowNames.Contains(rowName))
                                {
                                    int number = modAssetRow.Name.Number + 1;
                                    rowName = modAssetRow.Name.Value.ToString() + "_" + modAssetRow.Name.Number.ToString();
                                    modAssetRow.Name = new FName(modUasset, modAssetRow.Name.Value.ToString(), number);
                                }
                                dictionaryDataTable.RowNames.Add(rowName);

                                switch (gameAssetExport.ObjectName.ToString())
                                {
                                    case "AircraftViewerDataTable":
                                        if (MergeAircraftViewerDataTable(dictionaryDataTable, modAssetRow, exportAssetTableExport, exportStateDataTable)) {
                                            continue;
                                        }
                                        break;

                                    case "PlayerPlaneDataTable":
                                        if (MergePlayerPlaneDataTable(dictionaryDataTable, modAssetRow, exportUasset, exportAssetTableExport, exportStateDataTable)) {
                                            continue;
                                        }
                                        break;

                                    case "PlayerWeaponDataTable":
                                        if (MergePlayerWeaponDataTable(dictionaryDataTable, modAssetRow, exportUasset, exportAssetTableExport, exportStateDataTable))
                                        {
                                            continue;
                                        }
                                        break;

                                    case "SkinDataTable":
                                        if (MergeSkinDataTable(dictionaryDataTable, modAssetRow, exportAssetTableExport, exportStateDataTable, modAssetTableExport, j)){
                                            continue;
                                        }
                                        break;

                                    default:
                                        break;
                                }

                                // If it's not a row to add, do replacement
                                /*if (gameAssetTableExport.Table.Data.Count > j)
                                {
                                    var exportRowStateDataTable = exportStateDataTable.StateDataTableChild[j];

                                    var exportAssetRow = exportAssetTableExport.Table.Data[j];

                                    // Check modifications of existing row cells
                                    for (int k = 0; k < exportAssetRow.Value.Count; k++)
                                    {
                                        if ((exportAssetRow.Value[k].ToString() != modAssetRow.Value[k].ToString()) && exportRowStateDataTable.States[k] == StateDataTable.State.UNCHANGED)
                                        {
                                            exportAssetRow.Value[k] = modAssetRow.Value[k];
                                            exportRowStateDataTable.States[k] = StateDataTable.State.MODIFIED;
                                            exportStateDataTable.States[j] = StateDataTable.State.MODIFIED;
                                        }
                                    }
                                }*/


                            }
                        }
                        break;
                }
            }
        }

        public bool MergeAircraftViewerDataTable(DictionaryAC7DataTable dictionaryDataTable, StructPropertyData modAssetRow, DataTableExport exportAssetTableExport, StateDataTable exportStateDataTable)
        {
            string planeStringID = modAssetRow.Value[0].ToString();
            if (planeStringID != null && !dictionaryDataTable.IDs["AircraftViewerID"].Contains(planeStringID))
            {
                int aircraftViewerID = 1;
                aircraftViewerID = increaseID(dictionaryDataTable, "ExportAircraftViewerID", aircraftViewerID);
                dictionaryDataTable.IDs["ExportAircraftViewerID"].Add(aircraftViewerID.ToString());
                modAssetRow.Value[0].RawValue = aircraftViewerID;

                exportAssetTableExport.Table.Data.Add(modAssetRow);
                exportStateDataTable.StateDataTableChild.Add(new StateDataTable());
                exportStateDataTable.States.Add(StateDataTable.State.ADDED);
                return true;
            }
            return false;
        }

        public bool MergePlayerPlaneDataTable(DictionaryAC7DataTable dictionaryDataTable, StructPropertyData modAssetRow, UAsset exportUAsset, DataTableExport exportAssetTableExport, StateDataTable exportStateDataTable)
        {
            string planeStringID = modAssetRow.Value[2].ToString();
            if (planeStringID != null && !dictionaryDataTable.IDs["PlaneStringID"].Contains(planeStringID))
            {
                dictionaryDataTable.IDs["PlaneStringID"].Add(planeStringID);

                int planeId = 100;
                planeId = increaseID(dictionaryDataTable, "PlaneID", planeId);
                dictionaryDataTable.IDs["PlaneID"].Add(planeId.ToString());
                modAssetRow.Value[0].RawValue = planeId;

                int sortNumber = 1;
                sortNumber = increaseID(dictionaryDataTable, "SortNumber", sortNumber);
                dictionaryDataTable.IDs["SortNumber"].Add(sortNumber.ToString());
                modAssetRow.Value[20].RawValue = sortNumber;

                DataTableHelper.AddToNameMap(exportUAsset, modAssetRow.Value[getIndexFromPropertyDataValues(modAssetRow, "Reference")]); // Reference
                DataTableHelper.AddToNameMap(exportUAsset, modAssetRow.Value[getIndexFromPropertyDataValues(modAssetRow, "HangarAcquireCamera")]); // HangarAcquireCamera
                DataTableHelper.AddToNameMap(exportUAsset, modAssetRow.Value[getIndexFromPropertyDataValues(modAssetRow, "RefPlaneImagePortrait")]); // RefPlaneImagePortrait
                DataTableHelper.AddToNameMap(exportUAsset, modAssetRow.Value[getIndexFromPropertyDataValues(modAssetRow, "RefPlaneImageLandscape")]); // RefPlaneImageLandscape

                // Add the new row to the end of the dataTable 
                exportAssetTableExport.Table.Data.Add(modAssetRow);

                exportStateDataTable.StateDataTableChild.Add(new StateDataTable());
                exportStateDataTable.States.Add(StateDataTable.State.ADDED);
                return true;
            }
            return false;
        }

        public bool MergePlayerWeaponDataTable(DictionaryAC7DataTable dictionaryDataTable, StructPropertyData modAssetRow, UAsset exportUAsset, DataTableExport exportAssetTableExport, StateDataTable exportStateDataTable)
        {
            string weaponStringID = modAssetRow.Value[1].ToString();
            if (weaponStringID != null && !dictionaryDataTable.IDs["WeaponStringID"].Contains(weaponStringID))
            {
                dictionaryDataTable.IDs["WeaponStringID"].Add(weaponStringID);

                int weaponId = 0;
                weaponId = increaseID(dictionaryDataTable, "WeaponID", weaponId);
                dictionaryDataTable.IDs["WeaponID"].Add(weaponId.ToString());
                modAssetRow.Value[0].RawValue = weaponId;

                DataTableHelper.AddToNameMap(exportUAsset, modAssetRow.Value[getIndexFromPropertyDataValues(modAssetRow, "WeaponConceptManaTexture")]); // WeaponConceptManaTexture
                DataTableHelper.AddToNameMap(exportUAsset, modAssetRow.Value[getIndexFromPropertyDataValues(modAssetRow, "WeaponConceptVideoMaterial")]); // WeaponConceptVideoMaterial

                // Add the new row to the end of the dataTable 
                exportAssetTableExport.Table.Data.Add(modAssetRow);

                exportStateDataTable.StateDataTableChild.Add(new StateDataTable());
                exportStateDataTable.States.Add(StateDataTable.State.ADDED);
                return true;
            }
            return false;
        }

        public bool MergeSkinDataTable(DictionaryAC7DataTable dictionaryDataTable, StructPropertyData modAssetRow, DataTableExport exportAssetTableExport, StateDataTable exportStateDataTable, DataTableExport modAssetTableExport, int j)
        {
            string planeStringID = modAssetRow.Value[1].ToString();
            int skinID = -1;
            // Add new PlaneStringID to SkinDataTable if it isn't in the dataTable
            if (planeStringID != null && !dictionaryDataTable.IDs["PlaneStringID"].Contains(planeStringID))
            {
                dictionaryDataTable.IDs["PlaneStringID"].Add(planeStringID);
                dictionaryDataTable.PlaneSkinNoDictionary.Add(planeStringID, new List<int>());

                // Get the first SkinID of the new added plane
                skinID = 101;
                skinID = increaseID(dictionaryDataTable, "ExportSkinID", skinID, 100);
                dictionaryDataTable.IDs["ExportSkinID"].Add(skinID.ToString());

                // Add the new SkinID to the end of the dataTable
                exportAssetTableExport.Table.Data.Add(modAssetRow);
                
                exportStateDataTable.StateDataTableChild.Add(new StateDataTable());
                exportStateDataTable.States.Add(StateDataTable.State.ADDED);

                // Update the skinId of the row
                modAssetTableExport.Table.Data[j].Value[0].RawValue = skinID; // SkinID
                modAssetTableExport.Table.Data[j].Value[3].RawValue = skinID; // SortNumber

                dictionaryDataTable.PlaneSkinNoDictionary[planeStringID].Add((int)modAssetTableExport.Table.Data[j].Value[2].RawValue);

                return true;
            }
            // Add new skin to SkinDataTable
            else if (!dictionaryDataTable.IDs["SkinID"].Contains(modAssetRow.Value[0].ToString()))
            {
                if (!dictionaryDataTable.PlaneSkinNoDictionary[planeStringID].Contains((int)modAssetTableExport.Table.Data[j].Value[2].RawValue))
                {
                    // Find the first SkinID of the plane by finding the plane string IDs
                    var skinStructPropertyData = exportAssetTableExport.Table.Data.First(structPropertyData => structPropertyData.Value[1].ToString().Equals(planeStringID));
                    // Get the index of the first SkinID
                    int index = dictionaryDataTable.IDs["ExportSkinID"].IndexOf(skinStructPropertyData.Value[0].ToString());

                    // Increase SkinID and index to add the new SkinID
                    int.TryParse(skinStructPropertyData.Value[0].ToString(), out skinID);
                    while (dictionaryDataTable.IDs["ExportSkinID"].Contains(skinID.ToString()))
                    {
                        skinID++;
                        index++;
                    }

                    // Add the new SkinID to the end of the dataTable
                    if (dictionaryDataTable.IDs["ExportSkinID"].Count <= index)
                    {
                        dictionaryDataTable.IDs["ExportSkinID"].Add(skinID.ToString());

                        // Add the new row to the end of the dataTable 
                        exportAssetTableExport.Table.Data.Add(modAssetRow);
                        //
                        exportStateDataTable.StateDataTableChild.Add(new StateDataTable());
                        exportStateDataTable.States.Add(StateDataTable.State.ADDED);
                    }
                    // Insert the new SkinID in the dataTable
                    else
                    {
                        dictionaryDataTable.IDs["ExportSkinID"].Insert(index, skinID.ToString());

                        // Insert the new row in the dataTable
                        exportAssetTableExport.Table.Data.Insert(index, modAssetRow);
                        //
                        exportStateDataTable.StateDataTableChild.Insert(index, new StateDataTable());
                        exportStateDataTable.States.Insert(index, StateDataTable.State.ADDED);
                    }

                    // Update the skinId of the row
                    modAssetTableExport.Table.Data[j].Value[0].RawValue = skinID; // SkinID
                    modAssetTableExport.Table.Data[j].Value[3].RawValue = skinID; // SortNumber

                    dictionaryDataTable.PlaneSkinNoDictionary[planeStringID].Add((int)modAssetTableExport.Table.Data[j].Value[2].RawValue);
                }
                return true;
            }
            return false;
        }

        public void BuildStateTable(UAsset uasset, StateDataTable stateDataTable, DictionaryAC7DataTable dictionaryDataTable)
        {
            for (int i = 0; i < uasset.Exports.Count; i++)
            {
                stateDataTable.States.Add(StateDataTable.State.UNCHANGED);
                Export assetExport = uasset.Exports[i];
                FName assetExportClassTypeName = assetExport.GetExportClassType();
                string assetExportClassType = assetExportClassTypeName.Value.Value;

                // Add Cell Name as key to dictionary
                switch (assetExport.ObjectName.ToString())
                {
                    case "AircraftViewerDataTable":
                        dictionaryDataTable.IDs.Add("AircraftViewerID", new List<string>());
                        // To check if added IDs from mods are already used
                        dictionaryDataTable.IDs.Add("ExportAircraftViewerID", new List<string>());
                        break;

                    case "PlayerPlaneDataTable":
                        dictionaryDataTable.IDs.Add("PlaneID", new List<string>());
                        dictionaryDataTable.IDs.Add("PlaneStringID", new List<string>());
                        dictionaryDataTable.IDs.Add("SortNumber", new List<string>());
                        break;

                    case "PlayerWeaponDataTable":
                        dictionaryDataTable.IDs.Add("WeaponID", new List<string>());
                        dictionaryDataTable.IDs.Add("WeaponStringID", new List<string>());
                        break;

                    case "SkinDataTable":
                        dictionaryDataTable.IDs.Add("SkinID", new List<string>());
                        dictionaryDataTable.IDs.Add("PlaneStringID", new List<string>());
                        dictionaryDataTable.IDs.Add("SortNumber", new List<string>());
                        // To check if added IDs from mods are already used
                        dictionaryDataTable.IDs.Add("ExportSkinID", new List<string>());
                        break;

                    default:
                        break;
                }

                switch (assetExportClassType)
                {
                    default:
                        stateDataTable.StateDataTableChild.Add(new StateDataTable());
                        if (assetExportClassType.EndsWith("DataTable"))
                        {
                            DataTableExport originalAssetTableExport = (DataTableExport)assetExport;

                            // For each row
                            for (int j = 0; j < originalAssetTableExport.Table.Data.Count; j++)
                            {
                                StructPropertyData originalAssetRow = originalAssetTableExport.Table.Data[j];

                                dictionaryDataTable.RowNames.Add(originalAssetRow.Name.ToString());

                                stateDataTable.StateDataTableChild[i].StateDataTableChild.Add(new StateDataTable());
                                stateDataTable.StateDataTableChild[i].States.Add(StateDataTable.State.UNCHANGED);
                                // Put all states of row cells to UNCHANGED
                                for (int k = 0; k < originalAssetRow.Value.Count; k++)
                                    stateDataTable.StateDataTableChild[i].StateDataTableChild[j].States.Add(StateDataTable.State.UNCHANGED);

                                // Add ID to dictionary keys
                                switch (assetExport.ObjectName.ToString())
                                {
                                    case "AircraftViewerDataTable":
                                        dictionaryDataTable.IDs["AircraftViewerID"].Add(originalAssetRow.Value[0].ToString());
                                        
                                        dictionaryDataTable.IDs["ExportAircraftViewerID"].Add(originalAssetRow.Value[0].ToString());
                                        break;

                                    case "PlayerPlaneDataTable":
                                        dictionaryDataTable.IDs["PlaneID"].Add(originalAssetRow.Value[0].ToString());
                                        dictionaryDataTable.IDs["PlaneStringID"].Add(originalAssetRow.Value[2].ToString());
                                        dictionaryDataTable.IDs["SortNumber"].Add(originalAssetRow.Value[20].ToString());
                                        break;

                                    case "PlayerWeaponDataTable":
                                        dictionaryDataTable.IDs["WeaponID"].Add(originalAssetRow.Value[0].ToString());
                                        dictionaryDataTable.IDs["WeaponStringID"].Add(originalAssetRow.Value[1].ToString());
                                        break;

                                    case "SkinDataTable":
                                        dictionaryDataTable.IDs["SkinID"].Add(originalAssetRow.Value[0].ToString());
                                        if (!dictionaryDataTable.IDs["PlaneStringID"].Contains(originalAssetRow.Value[1].ToString()))
                                            dictionaryDataTable.IDs["PlaneStringID"].Add(originalAssetRow.Value[1].ToString());

                                        // Used to give a new ID
                                        // Where instead "SkinID" is used to detect which ID are from a mod
                                        dictionaryDataTable.IDs["ExportSkinID"].Add(originalAssetRow.Value[0].ToString());

                                        string planeStringID = originalAssetRow.Value[1].ToString();
                                        if (!dictionaryDataTable.PlaneSkinNoDictionary.ContainsKey(planeStringID))
                                            dictionaryDataTable.PlaneSkinNoDictionary.Add(planeStringID, new List<int>());
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }
                        break;
                }
            }
        }

        private int increaseID(DictionaryAC7DataTable dictionaryDataTable, string exportName, int id)
        {
            while (dictionaryDataTable.IDs[exportName].Contains(id.ToString()))
                id++;
            return id;
        }
     
        private int increaseID(DictionaryAC7DataTable dictionaryDataTable, string exportName, int id, int increase)
        {
            while (dictionaryDataTable.IDs[exportName].Contains(id.ToString()))
                id += increase;
            return id;
        }

        private int getIndexFromPropertyDataValues(StructPropertyData structPropertyData, string value)
        {
            return structPropertyData.Value.FindIndex(x => x.Name.ToString().Equals(value));
        }

        private void addNewRow()
        {
            /*
            // Check if rowName is already in data table
            string rowName = modAssetRow.Name.Value.ToString() + "_" + (modAssetRow.Name.Number - 1).ToString();
            while (dictionaryDataTable.RowNames.Contains(rowName))
            {
                int number = modAssetRow.Name.Number + 1;
                rowName = modAssetRow.Name.Value.ToString() + "_" + (modAssetRow.Name.Number).ToString();
                modAssetRow.Name = new FName(modUasset, modAssetRow.Name.Value.ToString(), number);
            }
            dictionaryDataTable.RowNames.Add(rowName);*/
        }
    }
}
