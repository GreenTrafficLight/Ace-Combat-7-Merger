using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI;
using UAssetAPI.UnrealTypes;
using UAssetAPI.PropertyTypes.Objects;
using CUE4Parse.FileProvider.Objects;
using CUE4Parse.UE4.Objects.Engine;
using Microsoft.VisualBasic;
using System.Reflection.Metadata;
using static System.Net.Mime.MediaTypeNames;
using System.Numerics;

namespace Ace_Combat_Merger
{
    public class DataTableMerger
    {
        public void MergeDataTables(UAsset gameUasset, UAsset exportUasset, UAsset modUasset, StateDataTable stateDataTable, DictionaryDataTable dictionaryDataTable, IReadOnlyDictionary<string, GameFile> pakModsFile, ModManager modManager)
        {
            for (int i = 0; i < gameUasset.Exports.Count; i++)
            {
                var exportStateDataTable = stateDataTable.StateDataTableChild[i];

                var gameAssetExport = gameUasset.Exports[i];
                FName gameAssetExportClassTypeName = gameAssetExport.GetExportClassType();
                string gameAssetExportClassType = gameAssetExportClassTypeName.Value.Value;

                var exportAssetExport = exportUasset.Exports[i];

                var modAssetExport = modUasset.Exports[i];
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

                                string planeStringID;
                                switch (gameAssetExport.ObjectName.ToString())
                                {
                                    case "AircraftViewerDataTable":
                                        if (!dictionaryDataTable.IDs["AircraftViewerID"].Contains(modAssetRow.Value[0].ToString()))
                                        {
                                            int? aircraftViewerID = DataTableHelper.TryGetInt(modAssetRow.Value[0].ToString());
                                            while (dictionaryDataTable.IDs["AircraftViewerID"].Contains(aircraftViewerID.ToString()))
                                                aircraftViewerID++;
                                            modAssetRow.Value[0].RawValue = aircraftViewerID;

                                            exportAssetTableExport.Table.Data.Add(modAssetRow);
                                            exportStateDataTable.StateDataTableChild.Add(new StateDataTable());
                                            exportStateDataTable.States.Add(StateDataTable.State.ADDED);
                                            continue;
                                        }
                                        break;

                                    case "PlayerPlaneDataTable":
                                        planeStringID = modAssetRow.Value[2].ToString();
                                        if (!dictionaryDataTable.IDs["PlaneStringID"].Contains(planeStringID))
                                        {
                                            dictionaryDataTable.IDs["PlaneStringID"].Add(planeStringID);

                                            int? planeID = DataTableHelper.TryGetInt(modAssetRow.Value[0].ToString());
                                            while (dictionaryDataTable.IDs["PlaneID"].Contains(planeID.ToString()))
                                                planeID++;
                                            modAssetRow.Value[0].RawValue = planeID;
                                            dictionaryDataTable.IDs["PlaneID"].Add(planeID.ToString());
                                            
                                            int? sortNumber = DataTableHelper.TryGetInt(modAssetRow.Value[20].ToString());
                                            while (dictionaryDataTable.IDs["SortNumber"].Contains(sortNumber.ToString()))
                                                sortNumber++;
                                            modAssetRow.Value[20].RawValue = sortNumber;
                                            dictionaryDataTable.IDs["SortNumber"].Add(sortNumber.ToString());

                                            DataTableHelper.AddToNameMap(exportUasset, modAssetRow.Value[4]); // Reference
                                            DataTableHelper.AddToNameMap(exportUasset, modAssetRow.Value[19]); // HangarAcquireCamera
                                            DataTableHelper.AddToNameMap(exportUasset, modAssetRow.Value[49]); // RefPlaneImagePortrait
                                            DataTableHelper.AddToNameMap(exportUasset, modAssetRow.Value[50]); // RefPlaneImageLandscape

                                            exportAssetTableExport.Table.Data.Add(modAssetRow);
                                            exportStateDataTable.StateDataTableChild.Add(new StateDataTable());
                                            exportStateDataTable.States.Add(StateDataTable.State.ADDED);
                                            continue;
                                        }
                                        break;

                                    case "SkinDataTable":
                                        planeStringID = modAssetRow.Value[1].ToString();
                                        int skinID = -1;
                                        // Add new PlaneStringID to SkinDataTable if it isn't in the dataTable
                                        if (!dictionaryDataTable.IDs["PlaneStringID"].Contains(planeStringID))
                                        {
                                            dictionaryDataTable.IDs["PlaneStringID"].Add(planeStringID);
                                            // Get the first SkinID of the new added plane
                                            skinID = 101;
                                            while (dictionaryDataTable.IDs["ModdedSkinID"].Contains(skinID.ToString()))
                                                skinID += 100;
                                            dictionaryDataTable.IDs["ModdedSkinID"].Add(skinID.ToString());

                                            exportAssetTableExport.Table.Data.Add(modAssetRow);
                                            exportStateDataTable.StateDataTableChild.Add(new StateDataTable());
                                            exportStateDataTable.States.Add(StateDataTable.State.ADDED);

                                            modAssetTableExport.Table.Data[j].Value[0].RawValue = skinID; // SkinID
                                            modAssetTableExport.Table.Data[j].Value[2].RawValue = skinID % 100 - 1; // SkinNo
                                            modAssetTableExport.Table.Data[j].Value[3].RawValue = skinID; // SortNumber
                                            if (skinID % 100 == 1)
                                                modAssetTableExport.Table.Data[j].Value[7].RawValue = new FString($"/Game/Blueprint/Player/Pawn/AcePlayerPawn_{planeStringID}.AcePlayerPawn_{planeStringID}_C");
                                            else
                                                modAssetTableExport.Table.Data[j].Value[7].RawValue = new FString($"/Game/Blueprint/Player/Pawn/AcePlayerPawn_{planeStringID}_s{skinID % 100 - 1:00}.AcePlayerPawn_{planeStringID}_s{skinID % 100 - 1:00}_C");

                                            continue;
                                        }
                                        // Add new skin to SkinDataTable
                                        else if (!dictionaryDataTable.IDs["SkinID"].Contains(modAssetRow.Value[0].ToString()))
                                        {
                                            // Find the first SkinID of the plane by finding the plane string IDs
                                            var skinStructPropertyData = exportAssetTableExport.Table.Data.First(structPropertyData => structPropertyData.Value[1].ToString().Equals(planeStringID));
                                            // Get the index of the first SkinID
                                            int index = dictionaryDataTable.IDs["ModdedSkinID"].IndexOf(skinStructPropertyData.Value[0].ToString());

                                            // Increase SkinID and index to add the new SkinID
                                            int.TryParse(skinStructPropertyData.Value[0].ToString(), out skinID);
                                            while (dictionaryDataTable.IDs["ModdedSkinID"].Contains(skinID.ToString()))
                                            {
                                                skinID++;
                                                index++;
                                            }
                                               
                                            // Add the new SkinID to the end of the dataTable
                                            if (dictionaryDataTable.IDs["ModdedSkinID"].Count <= index)
                                            {
                                                dictionaryDataTable.IDs["ModdedSkinID"].Add(skinID.ToString());
                                                
                                                exportAssetTableExport.Table.Data.Add(modAssetRow);
                                                exportStateDataTable.StateDataTableChild.Add(new StateDataTable());
                                                exportStateDataTable.States.Add(StateDataTable.State.ADDED);
                                            }
                                            // Insert the new SkinID in the dataTable
                                            else
                                            {
                                                dictionaryDataTable.IDs["ModdedSkinID"].Insert(index, skinID.ToString());
                                                
                                                exportAssetTableExport.Table.Data.Insert(index, modAssetRow);
                                                exportStateDataTable.StateDataTableChild.Insert(index, new StateDataTable());
                                                exportStateDataTable.States.Insert(index, StateDataTable.State.ADDED);
                                            }

                                            modAssetTableExport.Table.Data[j].Value[0].RawValue = skinID; // SkinID
                                            modAssetTableExport.Table.Data[j].Value[2].RawValue = skinID % 100 - 1; // SkinNo
                                            modAssetTableExport.Table.Data[j].Value[3].RawValue = skinID; // SortNumber
                                            if (skinID % 100 == 1)
                                                modAssetTableExport.Table.Data[j].Value[7].RawValue = new FString($"/Game/Blueprint/Player/Pawn/AcePlayerPawn_{planeStringID}.AcePlayerPawn_{planeStringID}_C");
                                            else
                                                modAssetTableExport.Table.Data[j].Value[7].RawValue = new FString($"/Game/Blueprint/Player/Pawn/AcePlayerPawn_{planeStringID}_s{skinID % 100 - 1:00}.AcePlayerPawn_{planeStringID}_s{skinID % 100 - 1:00}_C");

                                            continue;
                                        }




                                        break;

                                    default:
                                        break;
                                }

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
                            }
                        }
                        break;
                }
            }
        }
        public void BuildStateTable(UAsset uasset, StateDataTable stateDataTable, DictionaryDataTable dictionaryDataTable)
        {
            for (int i = 0; i < uasset.Exports.Count; i++)
            {
                stateDataTable.States.Add(StateDataTable.State.UNCHANGED);
                var assetExport = uasset.Exports[i];
                FName assetExportClassTypeName = assetExport.GetExportClassType();
                string assetExportClassType = assetExportClassTypeName.Value.Value;

                // Add Cell Name as key to dictionary
                switch (assetExport.ObjectName.ToString())
                {
                    case "AircraftViewerDataTable":
                        dictionaryDataTable.IDs.Add("AircraftViewerID", new List<string>());
                        
                        dictionaryDataTable.IDs.Add("ExportAircraftViewerID", new List<string>());
                        break;

                    case "PlayerPlaneDataTable":
                        dictionaryDataTable.IDs.Add("PlaneID", new List<string>());
                        dictionaryDataTable.IDs.Add("PlaneStringID", new List<string>());
                        dictionaryDataTable.IDs.Add("SortNumber", new List<string>());
                        break;

                    case "SkinDataTable":
                        dictionaryDataTable.IDs.Add("SkinID", new List<string>());
                        dictionaryDataTable.IDs.Add("PlaneStringID", new List<string>());
                        dictionaryDataTable.IDs.Add("SortNumber", new List<string>());

                        dictionaryDataTable.IDs.Add("ModdedSkinID", new List<string>());
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
                                var originalAssetRow = originalAssetTableExport.Table.Data[j];

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

                                    case "SkinDataTable":
                                        dictionaryDataTable.IDs["SkinID"].Add(originalAssetRow.Value[0].ToString());
                                        if (!dictionaryDataTable.IDs["PlaneStringID"].Contains(originalAssetRow.Value[1].ToString()))
                                            dictionaryDataTable.IDs["PlaneStringID"].Add(originalAssetRow.Value[1].ToString());

                                        // Used to give a new ID
                                        // Where instead "SkinID" is used to detect which ID are from a mod
                                        dictionaryDataTable.IDs["ModdedSkinID"].Add(originalAssetRow.Value[0].ToString());
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
