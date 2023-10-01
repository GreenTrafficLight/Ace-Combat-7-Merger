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
using System.Reflection.Metadata;
using System.IO;
using Ace_Combat_Merger.Utils;

namespace Ace_Combat_Merger
{
    public class ModManager
    {
        public List<KeyValuePair<string, Dictionary<string, GameFile>>> PaksGameFiles = new List<KeyValuePair<string, Dictionary<string, GameFile>>>();
        public List<KeyValuePair<string, Dictionary<string, GameFile>>> PaksModsGameFiles = new List<KeyValuePair<string, Dictionary<string, GameFile>>>();
        public DefaultFileProvider GameProvider;
        public DefaultFileProvider ModsProvider;
        private AC7Decrypt _AC7Decrypt = new AC7Decrypt();
        DataTableMerger dataTableMerger = new DataTableMerger();
        LocalizationMerger localizationMerger = new LocalizationMerger();

        // Table states for each uasset
        public Dictionary<string, StateDataTable> StateDataTables = new Dictionary<string, StateDataTable>();
        public Dictionary<string, DictionaryAC7DataTable> DictionaryDataTables = new Dictionary<string, DictionaryAC7DataTable>();
        public Dictionary<string, List<StateDataTable.State>> StateDATs = new Dictionary<string, List<StateDataTable.State>>();

        private readonly string _GameFilePath = "";
        private readonly string _ModFolderPath = "";
        public readonly string ExportFolderPath = "";

        private int stringToAdd = 0;

        public ModManager(string gameFilePath, string modFolderPath, string exportFolderPath, GamePathForm gamePathForm)
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

            gamePathForm.PakProgressLabel.Text = $"Paks : 0 / {PaksModsGameFiles.Count}";
            //gamePathForm.PakProgressLabel.Visible = true;
            gamePathForm.Update();

            int index = 1;
            foreach (KeyValuePair<string, Dictionary<string, GameFile>> modGameFiles in PaksModsGameFiles)
            {
                CreateDirectories(modGameFiles);

                WriteModifications(modGameFiles);

                gamePathForm.PakProgressBar.Text = $"Paks : {index} / {PaksModsGameFiles.Count}";
                gamePathForm.PakProgressBar.Value = index * 100 / PaksModsGameFiles.Count;
                gamePathForm.Update();

                index++;
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
        
        private void GetGameFiles(DefaultFileProvider provider, List<KeyValuePair<string, Dictionary<string, GameFile>>> paksFiles)
        {
            foreach (PakFileReader pak in provider.MountedVfs.OrderBy(x => x.Name).ToList())
            {
                Dictionary<string, GameFile> gameFiles = (Dictionary<string, GameFile>)pak.Mount();
                paksFiles.Add(new KeyValuePair<string, Dictionary<string, GameFile>>(Path.GetFileNameWithoutExtension(pak.Name), gameFiles));
            }
        }

        List<string> paths = new List<string>
        {
            "Nimbus\\Content\\Blueprint\\Information",
            "Nimbus\\Content\\Localization\\Game"
        };

        private void CreateDirectories(KeyValuePair<string, Dictionary<string, GameFile>> paksModsGameFiles)
        {
            foreach (var pakModsGameFiles in paksModsGameFiles.Value)
            {
                string assetFileName = Path.GetFileName(pakModsGameFiles.Key);
                string assetFileNameDirectory = Path.GetDirectoryName(pakModsGameFiles.Key);

                if (!paths.Contains(assetFileNameDirectory)) {
                    continue;
                }

                string originalDirectory = ExportFolderPath + "\\original\\" + assetFileNameDirectory + "\\";
                string exportDirectory = ExportFolderPath + "\\export\\" + assetFileNameDirectory + "\\";

                Directory.CreateDirectory(originalDirectory);
                Directory.CreateDirectory(exportDirectory);

                byte[] gameFile = null;

                // Get the original .uasset from the game paks
                if (!File.Exists(exportDirectory + assetFileName))
                {
                    foreach (var pakGameFiles in PaksGameFiles)
                    {
                        if (pakGameFiles.Value.ContainsKey(pakModsGameFiles.Key) && (!File.Exists(originalDirectory + assetFileName) || pakGameFiles.Key.EndsWith("_P")))
                        {
                            // Get all the game dats if there is a modified Cmn.dat in the mod
                            if (Path.GetExtension(assetFileName) == ".dat" && assetFileName != "Cmn.dat" && paksModsGameFiles.Value.ContainsKey("Nimbus/Content/Localization/Game/Cmn.dat"))
                            {        
                                string[] letters = new string[]{"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M"};
                                foreach (string letter in letters) {
                                    if (pakGameFiles.Value.ContainsKey("Nimbus/Content/Localization/Game/" + letter + ".dat"))
                                    {
                                        gameFile = pakGameFiles.Value["Nimbus/Content/Localization/Game/" + letter + ".dat"].Read();
                                        File.WriteAllBytes(originalDirectory + letter + ".dat", gameFile);
                                        File.WriteAllBytes(exportDirectory + letter + ".dat", gameFile);
                                    }
                                }
                                continue;
                            }

                            gameFile = pakGameFiles.Value[pakModsGameFiles.Key].Read();
                            File.WriteAllBytes(originalDirectory + assetFileName, gameFile);
                            File.WriteAllBytes(exportDirectory + assetFileName, gameFile);
                        }
                    }
                }
            }
        }
        
        private void WriteModifications(KeyValuePair<string, Dictionary<string, GameFile>> pakModsFiles)
        {
            // Localization CMN merger
            // If there is modified Cmn.dat in the mod
            if (pakModsFiles.Value.ContainsKey("Nimbus/Content/Localization/Game/Cmn.dat"))
            {
                string originalDirectory = ExportFolderPath + "\\original\\Nimbus\\Content\\Localization\\Game\\";
                string exportDirectory = ExportFolderPath + "\\export\\Nimbus\\Content\\Localization\\Game\\";

                // Get all the dats of the game
                DAT[] exportDats = Directory.GetFiles(exportDirectory)
                    .Where(path => Path.GetFileNameWithoutExtension(path) != "Cmn")
                    .Select(path => new DAT(path, Path.GetFileNameWithoutExtension(path)[0]))
                    .ToArray();

                // Get the modded dats except the CMN
                DAT[] moddedDats = pakModsFiles.Value.Keys
                    .Where(key => key.Contains("Nimbus/Content/Localization/Game/") && Path.GetFileNameWithoutExtension(key) != "Cmn")
                    .Select(key => new DAT(pakModsFiles.Value[key].Read(), Path.GetFileNameWithoutExtension(key)[0]))
                    .ToArray();

                char[] moddedDatsLetter = moddedDats.Select(dat => dat.Letter).ToArray();

                // The unmodified game CMN to get the strings count
                CMN gameCMN = new CMN(originalDirectory + "Cmn.dat");
                // The finished CMN at the end of the program
                CMN exportCMN = new CMN(exportDirectory + "Cmn.dat");

                // Get the modded CMN to get the added variables
                File.WriteAllBytes(exportDirectory + "temp.dat", pakModsFiles.Value["Nimbus/Content/Localization/Game/Cmn.dat"].Read());
                CMN modCMN = new CMN(exportDirectory + "temp.dat");
                File.Delete(exportDirectory + "temp.dat");

                // Merge the CMN
                foreach (var child in modCMN.Root) {
                    localizationMerger.MergeCMN(gameCMN, exportCMN, modCMN, exportDats, moddedDats, child);
                }

                // Add null string to the other dats that isn't in the mod
                foreach (DAT exportDat in exportDats) {
                    if (!moddedDatsLetter.Contains(exportDat.Letter)) {
                        while (exportDat.Strings.Count < exportCMN.MaxStringNumber){
                            exportDat.Strings.Add("\0");
                        }
                        
                    }
                }

                // Save changes
                foreach (DAT exportDat in exportDats) {
                    exportDat.Write(exportDirectory, exportDat.Letter);
                }
                exportCMN.Write(exportDirectory + "Cmn.dat");

                // Remove the dats from pakModsFile so it's doesn't loop them
                foreach (KeyValuePair<string, GameFile> pakModsFile in pakModsFiles.Value)
                {
                    if (pakModsFile.Key.Contains("Nimbus/Content/Localization/Game/")) {
                        pakModsFiles.Value.Remove(pakModsFile.Key);
                    }
                }
            }

            foreach (var pakModsFile in pakModsFiles.Value)
            {
                string assetFileName = Path.GetFileName(pakModsFile.Key);
                string assetFileNameDirectory = Path.GetDirectoryName(pakModsFile.Key);

                if (!paths.Contains(assetFileNameDirectory)) {
                    continue;
                }

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
                                DictionaryAC7DataTable dictionaryDataTable = new DictionaryAC7DataTable();
                                
                                // Check if there are a StateDataTable and DictionaryDataTable for that UAsset
                                if (StateDataTables.ContainsKey(Path.GetFileNameWithoutExtension(assetFileName)) && DictionaryDataTables.ContainsKey(Path.GetFileNameWithoutExtension(assetFileName)))
                                {
                                    // If yes, get them
                                    stateDataTable = StateDataTables[Path.GetFileNameWithoutExtension(assetFileName)];
                                    dictionaryDataTable = DictionaryDataTables[Path.GetFileNameWithoutExtension(assetFileName)];
                                }
                                else
                                {
                                    // If not, create them
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

                    // Localization DAT merger
                    case ".dat":
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
