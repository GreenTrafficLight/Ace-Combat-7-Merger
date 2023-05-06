using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Pak;
using CUE4Parse.UE4.Versions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UAssetAPI;
using UAssetAPI.UnrealTypes;
using Ace_Combat_Merger.Localization.Formats;

namespace Ace_Combat_Merger
{
    public class ModManager
    {
        public List<IReadOnlyDictionary<string, GameFile>> PaksGameFiles = new List<IReadOnlyDictionary<string, GameFile>>();
        public List<IReadOnlyDictionary<string, GameFile>> PaksModsGameFiles = new List<IReadOnlyDictionary<string, GameFile>>();
        public DefaultFileProvider GameProvider;
        public DefaultFileProvider ModsProvider;
        private AC7Decrypt _AC7Decrypt = new AC7Decrypt();
        DataTableMerger dataTableMerger = new DataTableMerger();

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

            foreach (IReadOnlyDictionary<string, GameFile> modGameFiles in PaksModsGameFiles)
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
        private void GetGameFiles(DefaultFileProvider provider, List<IReadOnlyDictionary<string, GameFile>> paksFiles)
        {
            foreach (PakFileReader pak in provider.MountedVfs.OrderBy(x => x.Name).ToList())
            {
                IReadOnlyDictionary<string, GameFile> gameFiles = pak.Mount();
                paksFiles.Add(gameFiles);
            }
        }


        private void CreateDirectories(IReadOnlyDictionary<string, GameFile> paksModsGameFiles)
        {
            foreach (var pakModsGameFiles in paksModsGameFiles)
            {
                string assetFileName = Path.GetFileName(pakModsGameFiles.Key);
                string assetFileNameDirectory = Path.GetDirectoryName(pakModsGameFiles.Key);

                string originalDirectory = ExportFolderPath + "\\original\\" + assetFileNameDirectory + "\\";
                string exportDirectory = ExportFolderPath + "\\export\\" + assetFileNameDirectory + "\\";

                Directory.CreateDirectory(originalDirectory);
                Directory.CreateDirectory(exportDirectory);


                if (assetFileNameDirectory.Equals("Nimbus\\Content\\Blueprint\\Player\\Pawn")) // Replace for skins
                {
                    Directory.CreateDirectory(ExportFolderPath + "\\original\\Nimbus\\Content\\Blueprint\\Information");
                    // Get the original .uasset from the game paks
                    foreach (var pakGameFiles in PaksGameFiles)
                    {
                        if (pakGameFiles.ContainsKey("Nimbus/Content/Blueprint/Information/PlayerPlaneDataTable.uasset"))
                        {
                            File.WriteAllBytes(ExportFolderPath + "\\original\\Nimbus\\Content\\Blueprint\\Information\\PlayerPlaneDataTable.uasset", pakGameFiles["Nimbus/Content/Blueprint/Information/PlayerPlaneDataTable.uasset"].Read());
                        }
                        if (pakGameFiles.ContainsKey("Nimbus/Content/Blueprint/Information/PlayerPlaneDataTable.uexp"))
                        {
                            File.WriteAllBytes(ExportFolderPath + "\\original\\Nimbus\\Content\\Blueprint\\Information\\PlayerPlaneDataTable.uexp", pakGameFiles["Nimbus/Content/Blueprint/Information/PlayerPlaneDataTable.uexp"].Read());
                        }
                    }
                    File.WriteAllBytes(exportDirectory + assetFileName, pakModsGameFiles.Value.Read());
                }
                else
                {
                    byte[] gameFile = null;

                    // Get the original .uasset from the game paks
                    foreach (var pakGameFiles in PaksGameFiles)
                    {
                        if (pakGameFiles.ContainsKey(pakModsGameFiles.Key))
                        {
                            gameFile = pakGameFiles[pakModsGameFiles.Key].Read();
                        }
                    }

                    if (gameFile != null && !File.Exists(originalDirectory + assetFileName))
                    {
                        File.WriteAllBytes(originalDirectory + assetFileName, gameFile);
                        File.WriteAllBytes(exportDirectory + assetFileName, gameFile);
                    }
                }
            }
        }
        private void WriteModifications(IReadOnlyDictionary<string, GameFile> pakModsFiles)
        {
            foreach (var pakModsFile in pakModsFiles)
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
                        if (assetFileNameDirectory.Equals("Nimbus\\Content\\Blueprint\\Player\\Pawn")) // Replace for skins
                        {
                            _AC7Decrypt.Decrypt(ExportFolderPath + "\\original\\Nimbus\\Content\\Blueprint\\Information\\PlayerPlaneDataTable.uasset", ExportFolderPath + "\\original\\Nimbus\\Content\\Blueprint\\Information\\PlayerPlaneDataTable.uasset");
                            UAsset gameUasset = new UAsset(ExportFolderPath + "\\original\\Nimbus\\Content\\Blueprint\\Information\\PlayerPlaneDataTable.uasset", EngineVersion.VER_UE4_18, null);

                            Regex reg = new Regex(@"\b[AcePlayerPawn_]\w+");
                            var files = Directory.GetFiles(exportDirectory, "*.uasset").Where(path => reg.IsMatch(path));
                        }
                        else
                        {
                            // DataTable merger
                            string gameUassetPath = originalDirectory + Path.GetFileNameWithoutExtension(assetFileName) + ".uasset";
                            string exportUassetPath = exportDirectory + Path.GetFileNameWithoutExtension(assetFileName) + ".uasset";

                            UAsset gameUasset = GetGameUasset(gameUassetPath);
                            UAsset exportUasset = GetGameUasset(exportUassetPath);

                            // Get modded asset
                            File.WriteAllBytes(exportDirectory + "temp.uasset", pakModsFiles[pakModsFile.Key].Read());
                            File.WriteAllBytes(exportDirectory + "temp.uexp", pakModsFiles[Path.ChangeExtension(pakModsFile.Key, ".uexp")].Read());
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

                            dataTableMerger.MergeDataTables(gameUasset, exportUasset, modUasset, stateDataTable, dictionaryDataTable, pakModsFiles, this);

                            exportUasset.Write(exportUassetPath);
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

                                File.WriteAllBytes(exportDirectory + "temp.dat", pakModsFiles[pakModsFile.Key].Read());
                                CMN modCMN = new CMN(exportDirectory + "temp.dat");
                                File.Delete(exportDirectory + "temp.dat");
                                break;

                            default:
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
                                exportDAT.Write(exportDatPath);

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
