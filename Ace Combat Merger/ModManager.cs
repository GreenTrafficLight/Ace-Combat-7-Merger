using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider.Objects;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Pak;
using CUE4Parse.UE4.Versions;
using UAssetAPI;
using UAssetAPI.UnrealTypes;
using Ace7Localization.Formats;

namespace Ace_Combat_Merger
{
    public class ModManager
    {
        public List<KeyValuePair<string, IReadOnlyDictionary<string, GameFile>>> PaksGameFiles = new List<KeyValuePair<string, IReadOnlyDictionary<string, GameFile>>>();
        public List<KeyValuePair<string, IReadOnlyDictionary<string, GameFile>>> PaksModsGameFiles = new List<KeyValuePair<string, IReadOnlyDictionary<string, GameFile>>>();
        public DefaultFileProvider GameProvider;
        public DefaultFileProvider ModsProvider;
        private AC7Decrypt _AC7Decrypt = new AC7Decrypt();
        DataTableMerger dataTableMerger = new DataTableMerger();
        LocalizationMerger localizationMerger = new LocalizationMerger();

        // Table states for each uasset
        public Dictionary<string, StateDataTable> StateDataTables = new Dictionary<string, StateDataTable>();
        public Dictionary<string, DictionaryDataTable> DictionaryDataTables = new Dictionary<string, DictionaryDataTable>();
        public Dictionary<string, List<StateDataTable.State>> StateDATs = new Dictionary<string, List<StateDataTable.State>>();

        private readonly string _GameFilePath = "";
        private readonly string _ModFolderPath = "";
        public readonly string ExportFolderPath = "";

        public ModManager(string gameFilePath, string modFolderPath, string exportFolderPath)
        {
            _GameFilePath = gameFilePath;
            _ModFolderPath = modFolderPath;
            ExportFolderPath = exportFolderPath;

            GameProvider = new DefaultFileProvider(_GameFilePath, SearchOption.TopDirectoryOnly, false, new VersionContainer(EGame.GAME_AceCombat7));
            GameProvider.Initialize();
            GameProvider.SubmitKey(new(0U), new FAesKey("68747470733a2f2f616365372e616365636f6d6261742e6a702f737065636961"));
            GetGameFiles(GameProvider, PaksGameFiles);

            ModsProvider = new DefaultFileProvider(_ModFolderPath, SearchOption.AllDirectories, false, new VersionContainer(EGame.GAME_UE4_18));
            ModsProvider.Initialize();
            ModsProvider.SubmitKey(new(0U), new FAesKey("0000000000000000000000000000000000000000000000000000000000000000"));
            GetGameFiles(ModsProvider, PaksModsGameFiles);

            Directory.CreateDirectory(ExportFolderPath);

            foreach (KeyValuePair<string, IReadOnlyDictionary<string, GameFile>> modGameFiles in PaksModsGameFiles)
            {
                CreateDirectories(modGameFiles);

                WriteModifications(modGameFiles);
            }

            Directory.Delete(ExportFolderPath + "\\original", true);
        }


        public UAsset GetGameUasset(string assetPath)
        {
            uint sig = DataTableHelper.GetFileSignature(assetPath);
            if (sig == UAsset.ACE7_MAGIC)
                _AC7Decrypt.Decrypt(assetPath, assetPath);
            UAsset gameUasset = new UAsset(assetPath, EngineVersion.VER_UE4_18, null);
            return gameUasset;
        }
        private void GetGameFiles(DefaultFileProvider provider, List<KeyValuePair<string, IReadOnlyDictionary<string, GameFile>>> paksFiles)
        {
            foreach (PakFileReader pak in provider.MountedVfs.OrderBy(x => x.Name).ToList())
            {
                IReadOnlyDictionary<string, GameFile> gameFiles = pak.Mount();
                paksFiles.Add(new KeyValuePair<string, IReadOnlyDictionary<string, GameFile>>(Path.GetFileNameWithoutExtension(pak.Name), gameFiles));
            }
        }


        private void CreateDirectories(KeyValuePair<string, IReadOnlyDictionary<string, GameFile>> paksModsGameFiles)
        {
            foreach (var pakModsGameFiles in paksModsGameFiles.Value)
            {
                string assetFileName = Path.GetFileName(pakModsGameFiles.Key);
                string assetFileNameDirectory = Path.GetDirectoryName(pakModsGameFiles.Key);

                string originalDirectory = ExportFolderPath + "\\original\\" + assetFileNameDirectory + "\\";
                string exportDirectory = ExportFolderPath + "\\export\\" + assetFileNameDirectory + "\\";

                Directory.CreateDirectory(originalDirectory);
                Directory.CreateDirectory(exportDirectory);

                switch (assetFileNameDirectory)
                {
                    case "Nimbus\\Content\\Blueprint\\Information":
                        break;

                    default:
                        break;
                }

                byte[] gameFile = null;

                // Get the original .uasset from the game paks
                foreach (var pakGameFiles in PaksGameFiles)
                {
                    if (pakGameFiles.Value.ContainsKey(pakModsGameFiles.Key))
                    {
                        gameFile = pakGameFiles.Value[pakModsGameFiles.Key].Read();
                    }

                    if (gameFile != null && (!File.Exists(originalDirectory + assetFileName) || pakGameFiles.Key.EndsWith("_P")))
                    {
                        File.WriteAllBytes(originalDirectory + assetFileName, gameFile);
                        File.WriteAllBytes(exportDirectory + assetFileName, gameFile);
                    }
                }
            }
        }
        
        private void WriteModifications(KeyValuePair<string, IReadOnlyDictionary<string, GameFile>> pakModsFiles)
        {
            foreach (var pakModsFile in pakModsFiles.Value)
            {
                string assetFileName = Path.GetFileName(pakModsFile.Key);
                string assetFileNameDirectory = Path.GetDirectoryName(pakModsFile.Key);

                string originalDirectory = ExportFolderPath + "\\original\\" + assetFileNameDirectory + "\\";
                string exportDirectory = ExportFolderPath + "\\export\\" + assetFileNameDirectory + "\\";

                Directory.CreateDirectory(originalDirectory);
                Directory.CreateDirectory(exportDirectory);

                switch (Path.GetExtension(assetFileName))
                {
                    case ".uasset":
                        UAsset gameUasset;
                        switch (assetFileNameDirectory)
                        {
                            case "Nimbus\\Content\\Blueprint\\Information":
                                // DataTable merger
                                string gameUassetPath = originalDirectory + Path.GetFileNameWithoutExtension(assetFileName) + ".uasset";
                                string exportUassetPath = exportDirectory + Path.GetFileNameWithoutExtension(assetFileName) + ".uasset";

                                gameUasset = GetGameUasset(gameUassetPath);
                                UAsset exportUasset = GetGameUasset(exportUassetPath);

                                // Get modded asset
                                File.WriteAllBytes(exportDirectory + "temp.uasset", pakModsFiles.Value[pakModsFile.Key].Read());
                                File.WriteAllBytes(exportDirectory + "temp.uexp", pakModsFiles.Value[Path.ChangeExtension(pakModsFile.Key, ".uexp")].Read());
                                UAsset modUasset = new UAsset(exportDirectory + "temp.uasset", EngineVersion.VER_UE4_18, null);
                                File.Delete(exportDirectory + "temp.uasset");
                                File.Delete(exportDirectory + "temp.uexp");

                                // Write modifications to datatable
                                StateDataTable stateDataTable = new StateDataTable();
                                DictionaryDataTable dictionaryDataTable = new DictionaryDataTable();
                                if (StateDataTables.ContainsKey(Path.GetFileNameWithoutExtension(assetFileName)) && DictionaryDataTables.ContainsKey(Path.GetFileNameWithoutExtension(assetFileName)))
                                {
                                    stateDataTable = StateDataTables[Path.GetFileNameWithoutExtension(assetFileName)];
                                    dictionaryDataTable = DictionaryDataTables[Path.GetFileNameWithoutExtension(assetFileName)];
                                }
                                else
                                {
                                    dataTableMerger.BuildStateTable(gameUasset, stateDataTable, dictionaryDataTable);
                                    StateDataTables[Path.GetFileNameWithoutExtension(assetFileName)] = stateDataTable;
                                    DictionaryDataTables[Path.GetFileNameWithoutExtension(assetFileName)] = dictionaryDataTable;
                                }

                                dataTableMerger.MergeDataTables(gameUasset, exportUasset, modUasset, stateDataTable, dictionaryDataTable, pakModsFiles.Value, this);

                                exportUasset.Write(exportUassetPath);
                                break;

                            default:
                                break;
                        }
                        break;

                    // Localization merger
                    case ".dat":
                        string gameDatPath = originalDirectory + Path.GetFileNameWithoutExtension(assetFileName) + ".dat";
                        string exportDatPath = exportDirectory + Path.GetFileNameWithoutExtension(assetFileName) + ".dat";

                        switch (Path.GetFileNameWithoutExtension(assetFileName))
                        {
                            case "Cmn":
                                CMN gameCMN = new CMN(gameDatPath);
                                CMN exportCMN = new CMN(exportDatPath);

                                File.WriteAllBytes(exportDirectory + "temp.dat", pakModsFiles.Value[pakModsFile.Key].Read());
                                CMN modCMN = new CMN(exportDirectory + "temp.dat");
                                File.Delete(exportDirectory + "temp.dat");

                                foreach (var child in modCMN.Root)
                                {
                                    localizationMerger.MergeCMN(gameCMN, exportCMN, modCMN, child);
                                }

                                exportCMN.Write(exportDatPath);
                                break;

                            default:
                                /*
                                DAT gameDAT = new DAT(gameDatPath);
                                DAT exportDAT = new DAT(exportDatPath);

                                File.WriteAllBytes(exportDirectory + "temp.dat", pakModsFiles[pakModsFile.Key].Read());
                                DAT modDAT = new DAT(exportDirectory + "temp.dat", assetFileName);
                                File.Delete(exportDirectory + "temp.dat");

                                List<StateDataTable.State> stateDAT;
                                if (StateDATs.ContainsKey(Path.GetFileNameWithoutExtension(assetFileName)))
                                {
                                    stateDAT = StateDATs[Path.GetFileNameWithoutExtension(assetFileName)];
                                }
                                else
                                {
                                    stateDAT = new List<StateDataTable.State>();
                                    foreach (string str in gameDAT.Strings)
                                        stateDAT.Add(StateDataTable.State.UNCHANGED);
                                    StateDATs[Path.GetFileNameWithoutExtension(assetFileName)] = stateDAT;
                                }

                                for (int j = 0; j < modDAT.Strings.Count; j++)
                                {
                                    if (gameDAT.Strings.Count <= j)
                                        exportDAT.Strings.Add(modDAT.Strings[j]);
                                    else
                                    {
                                        if (!exportDAT.Strings[j].Equals(modDAT.Strings[j]) && stateDAT[j] == StateDataTable.State.UNCHANGED)
                                        {
                                            exportDAT.Strings[j] = modDAT.Strings[j];
                                            stateDAT[j] = StateDataTable.State.MODIFIED;
                                        }
                                        
                                    }
                                }
                                exportDAT.Write(exportDatPath);*/
                                break;
                        }


                        break;

                    default:
                        break;
                }
            }
        }
    }
}
