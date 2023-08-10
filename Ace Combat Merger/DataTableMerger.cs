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
using Microsoft.VisualBasic;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using System.Numerics;
using System.Collections;

namespace Ace_Combat_Merger
{
    public class DataTableMerger
    {
        /// <summary>
        /// Merge two datatables of Ace Combat 7
        /// </summary>
        /// <param name="gameUasset">The game uasset</param>
        /// <param name="exportUasset"> The uasset that is going to be exported</param>
        /// <param name="modUasset">The modded uasset we want to compare with the gameUasset</param>
        public void MergeDataTables(UAsset gameUasset, UAsset exportUasset, UAsset modUasset, StateDataTable stateDataTable, DictionaryDataTable dictionaryDataTable, IReadOnlyDictionary<string, GameFile> pakModsFile, ModManager modManager)
        {
            for (int i = 0; i < gameUasset.Exports.Count; i++)
            {
                var exportStateDataTable = stateDataTable.StateDataTableChild[i];

                // Get the exports of the game
                var gameAssetExport = gameUasset.Exports[i];
                FName gameAssetExportClassTypeName = gameAssetExport.GetExportClassType();
                string gameAssetExportClassType = gameAssetExportClassTypeName.Value.Value;

                var exportAssetExport = exportUasset.Exports[i];

                // Get the exports of the mod
                var modAssetExport = modUasset.Exports[i];
                FName modAssetExportClassTypeName = modAssetExport.GetExportClassType();
                string modAssetExportClassType = modAssetExportClassTypeName.Value.Value;

                switch (gameAssetExportClassType)
                {
                    case "DataTable":
                        // Check each Row
                        DataTableExport gameAssetTableExport = (DataTableExport)gameAssetExport;
                        DataTableExport exportAssetTableExport = (DataTableExport)exportAssetExport;
                        DataTableExport modAssetTableExport = (DataTableExport)modAssetExport;

                        for (int j = 0; j < modAssetTableExport.Table.Data.Count; j++)
                        {
                            // Check each row of Row of the modded asset
                            StructPropertyData modAssetRow = modAssetTableExport.Table.Data[j];

                            // Add new row to the export asset datatable
                            if (gameAssetTableExport.Table.Data.Count <= j)
                            {
                                exportStateDataTable.StateDataTableChild.Add(new StateDataTable());
                                exportStateDataTable.States.Add(StateDataTable.State.ADDED);

                                var rowStateDataTable = exportStateDataTable.StateDataTableChild.LastOrDefault();

                                // Check if rowName is already in the export asset datatable
                                string rowName = modAssetRow.Name.Value.ToString() + "_" + (modAssetRow.Name.Number - 1).ToString();
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
                                        int? aircraftViewerID = DataTableHelper.TryGetInt(modAssetRow.Value[0].ToString());
                                        aircraftViewerID = increaseID(dictionaryDataTable, "AircraftViewerID", aircraftViewerID);
                                        dictionaryDataTable.IDs["AircraftViewerID"].Add(aircraftViewerID.ToString());
                                        modAssetRow.Value[0].RawValue = aircraftViewerID;
                                        break;

                                    case "PlayerPlaneDataTable":
                                        // Add a unique PlaneID to the dataTable
                                        int? planeID = DataTableHelper.TryGetInt(modAssetRow.Value[0].ToString());
                                        // Check if the PlaneID exists and if so, increase until it doesn't exist
                                        planeID = increaseID(dictionaryDataTable, "PlaneID", planeID);
                                        dictionaryDataTable.IDs["PlaneID"].Add(planeID.ToString());
                                        modAssetRow.Value[0].RawValue = planeID;

                                        int? sortNumber = DataTableHelper.TryGetInt(modAssetRow.Value[20].ToString());
                                        sortNumber = increaseID(dictionaryDataTable, "SortNumber", sortNumber);
                                        dictionaryDataTable.IDs["SortNumber"].Add(sortNumber.ToString());
                                        modAssetRow.Value[20].RawValue = sortNumber;

                                        DataTableHelper.AddToNameMap(exportUasset, modAssetRow.Value[4]); // Reference
                                        DataTableHelper.AddToNameMap(exportUasset, modAssetRow.Value[19]); // HangarAcquireCamera
                                        DataTableHelper.AddToNameMap(exportUasset, modAssetRow.Value[49]); // RefPlaneImagePortrait
                                        DataTableHelper.AddToNameMap(exportUasset, modAssetRow.Value[50]); // RefPlaneImageLandscape

                                        // TO DO : Add ID to SkinDataTable if there isn't any SkinDataTable in the .pak
                                        if (!pakModsFile.Any(p => p.Key.Contains("Nimbus/Content/Blueprint/Information/SkinDataTable.uasset")) && !File.Exists(modManager.ExportFolderPath + "\\export\\Nimbus\\Content\\Blueprint\\Information\\SkinDataTable.uasset"))
                                        {
                                            //DataTableHelper.GetPackageFromPaks("Nimbus/Content/Blueprint/Information/SkinDataTable", modManager.ExportFolderPath + "\\export\\Nimbus\\Content\\Blueprint\\Information\\SkinDataTable", modManager.PaksGameFiles);
                                            //UAsset skinDataTable = modManager.GetGameUasset(modManager.ExportFolderPath + "\\export\\Nimbus\\Content\\Blueprint\\Information\\SkinDataTable.uasset");
                                        }

                                        /*if (!pakModsFile.Any(p => p.Key.Contains("Nimbus/Content/Localization/Game")))
                                        {
                                            DataTableHelper.GetPackageFromPaks("Nimbus/Content/Localization/Game/Cmn", modManager.ExportFolderPath + "\\export\\Nimbus\\Content\\Localization\\Game\\Cmn", modManager.PaksGameFiles);

                                            string[] datLetters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "K", "L", "M" };
                                            foreach (string datLetter in datLetters)
                                            {
                                                DataTableHelper.GetPackageFromPaks("Nimbus/Content/Localization/Game/" + datLetter, modManager.ExportFolderPath + "\\export\\Nimbus\\Content\\Localization\\Game\\" + datLetter, modManager.PaksGameFiles);
                                            }     
                                        }*/


                                        break;

                                    case "SkinDataTable":
                                        string planeStringID = modAssetRow.Value[1].ToString();
                                        int? skinID = -1;
                                        // Add new plane to SkinDataTable
                                        if (!dictionaryDataTable.IDs["PlaneStringID"].Contains(planeStringID))
                                        {
                                            dictionaryDataTable.IDs["PlaneStringID"].Add(planeStringID);
                                            skinID = 101;
                                            skinID = increaseID(dictionaryDataTable, "SkinID", skinID, 100);
                                            dictionaryDataTable.IDs["SkinID"].Add(skinID.ToString());
                                            dictionaryDataTable.IDs["SortNumber"].Add(skinID.ToString());
                                        }
                                        else
                                        {
                                            var skinStructPropertyData = exportAssetTableExport.Table.Data.First(structPropertyData => structPropertyData.Value[1].ToString().Equals(planeStringID)); // Find the first SkinID of the plane
                                            int index = dictionaryDataTable.IDs["SkinID"].IndexOf(skinStructPropertyData.Value[0].ToString()); // Get the index of the SkinID

                                            // Increase SkinID
                                            skinID = DataTableHelper.TryParseNullable(skinStructPropertyData.Value[0].ToString());
                                            skinID = increaseID(dictionaryDataTable, "SkinID", skinID);

                                            if (dictionaryDataTable.IDs["SkinID"].Count <= index + 1)
                                            {
                                                dictionaryDataTable.IDs["SkinID"].Add(skinID.ToString());
                                                dictionaryDataTable.IDs["SortNumber"].Add(skinID.ToString());
                                            }
                                            else
                                            {
                                                dictionaryDataTable.IDs["SkinID"].Insert(index + 1, skinID.ToString());
                                                dictionaryDataTable.IDs["SortNumber"].Insert(index + 1, skinID.ToString());
                                            }

                                            // Extract the number from PlaneReference
                                            string pattern = @"_s(\d+)";
                                            Match match = Regex.Match(modAssetRow.Value[7].RawValue.ToString(), pattern);
                                            if (match.Success)
                                            {
                                                int skinNumber = int.Parse(match.Groups[1].Value);
                                                modAssetRow.Value[2].RawValue = skinNumber; // SkinNo
                                            }
                                            else
                                            {

                                            }
                                        }

                                        modAssetRow.Value[0].RawValue = skinID; // SkinID
                                        modAssetRow.Value[3].RawValue = skinID; // SortNumber

                                        /*modAssetRow.Value[0].RawValue = skinID; // SkinID
                                        modAssetRow.Value[2].RawValue = skinID % 100 - 1; // SkinNo
                                        modAssetRow.Value[3].RawValue = skinID; // SortNumber
                                        if (skinID % 100 == 1)
                                            modAssetRow.Value[7].RawValue = new FString($"/Game/Blueprint/Player/Pawn/AcePlayerPawn_{planeStringID}.AcePlayerPawn_{planeStringID}_C");
                                        else
                                            modAssetRow.Value[7].RawValue = new FString($"/Game/Blueprint/Player/Pawn/AcePlayerPawn_{planeStringID}_s{skinID % 100 - 1:00}.AcePlayerPawn_{planeStringID}_s{skinID % 100 - 1:00}_C");*/

                                        break;

                                    default:

                                        break;
                                }

                                exportAssetTableExport.Table.Data.Add(modAssetRow);
                            }
                            // Modify the existing row
                            else
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
                            }
                        }
                        break;

                    default:

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
                        dictionaryDataTable.IDs.Add("PlaneStringID", new List<string>());
                        break;

                    case "PlayerPlaneDataTable":
                        dictionaryDataTable.IDs.Add("PlaneID", new List<string>());
                        dictionaryDataTable.IDs.Add("SortNumber", new List<string>());
                        break;

                    case "SkinDataTable":
                        dictionaryDataTable.IDs.Add("SkinID", new List<string>());
                        dictionaryDataTable.IDs.Add("PlaneStringID", new List<string>());
                        dictionaryDataTable.IDs.Add("SortNumber", new List<string>());
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
                                    case "PlayerPlaneDataTable":
                                        dictionaryDataTable.IDs["PlaneID"].Add(originalAssetRow.Value[0].ToString());
                                        dictionaryDataTable.IDs["SortNumber"].Add(originalAssetRow.Value[20].ToString());
                                        break;

                                    case "SkinDataTable":
                                        dictionaryDataTable.IDs["SkinID"].Add(originalAssetRow.Value[0].ToString());
                                        if (!dictionaryDataTable.IDs["PlaneStringID"].Contains(originalAssetRow.Value[1].ToString()))
                                            dictionaryDataTable.IDs["PlaneStringID"].Add(originalAssetRow.Value[1].ToString());
                                        dictionaryDataTable.IDs["SortNumber"].Add(originalAssetRow.Value[3].ToString());
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

        private int? increaseID(DictionaryDataTable dictionaryDataTable, string exportName, int? id)
        {
            while (dictionaryDataTable.IDs[exportName].Contains(id.ToString()))
                id++;
            return id;
        }

        private int? increaseID(DictionaryDataTable dictionaryDataTable, string exportName, int? id, int increase)
        {
            while (dictionaryDataTable.IDs[exportName].Contains(id.ToString()))
                id += increase;
            return id;
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
