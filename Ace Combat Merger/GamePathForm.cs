using CUE4Parse.UE4.Pak;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.UE4.Versions;
using CUE4Parse.FileProvider;

using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using System.Text.RegularExpressions;

using UAssetAPI;
using UAssetAPI.UnrealTypes;
using UAssetAPI.ExportTypes;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.PropertyTypes.Objects;
using Microsoft.Win32;
using Ace7Localization.Formats;

namespace Ace_Combat_Merger
{
    public partial class GamePathForm : Form
    {
        private string _gameFilePath = "";
        private string _modFolderPath = "";
        private string _exportFolderPath = "";

        private ModManager _ModManager;

        public string GameFilePath
        {
            get { return _gameFilePath; }
            set 
            { 
                if (_gameFilePath != value) 
                { 
                    _gameFilePath = value;
                    gamePaksFolderPathTextBox.Text = value;
                } 
            }
        }

        public string ModFolderPath
        {
            get { return _modFolderPath; }
            set 
            {
                if (_modFolderPath != value) 
                { 
                    _modFolderPath = value;
                    modsFolderPathTextBox.Text = value;
                } 
            }
        }

        public string ExportFolderPath
        {
            get { return _exportFolderPath + "\\temp"; }
            set 
            {
                if (_exportFolderPath != value) 
                {
                    _exportFolderPath = value;
                    exportPathTextBox.Text = value;
                }
                
            }
        }
        public GamePathForm()
        {
            InitializeComponent();

#if DEBUG
            //GameFilePath = "E:\\Program Files(x86)\\Steam\\steamapps\\common\\ACE COMBAT 7\\Game\\Content\\Paks";
#endif

            var strSteamInstallPath = Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Wow6432Node\\Valve\\Steam", "InstallPath", null);
            string libraryfoldersPath = $"{strSteamInstallPath}\\steamapps\\libraryfolders.vdf";
        }

        #region button

        private void gamePaksFolderPathButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    GameFilePath = fbd.SelectedPath;
                    if (Directory.Exists(GameFilePath + "\\~mods"))
                    {
                        ModFolderPath = GameFilePath + "\\~mods";
                    }
                }
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(ModFolderPath))
            {
                DialogResult = DialogResult.OK;

                _ModManager = new ModManager(GameFilePath, ModFolderPath, ExportFolderPath);
            }
            else
                DialogResult = DialogResult.Cancel;
            Close();
        }

        private void modsFolderPathButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    ModFolderPath = fbd.SelectedPath;
                }
            }
        }

        private void exportPathButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    ExportFolderPath = fbd.SelectedPath;
                }
            }
        }

        #endregion

        #region textBox

        #region gamePaksFolderPathTextBox
        private void gamePaksFolderPathTextBox_Enter(object sender, EventArgs e)
        {
            if (gamePaksFolderPathTextBox.Text == "Game paks folder ( i.e : <Game Folder>/Game/Content/Paks ) ")
            {
                gamePaksFolderPathTextBox.Text = "";
                gamePaksFolderPathTextBox.ForeColor = Color.Black;
            }
        }

        private void gamePaksFolderPathTextBox_Leave(object sender, EventArgs e)
        {
            if (gamePaksFolderPathTextBox.Text == "")
            {
                gamePaksFolderPathTextBox.Text = "Game paks folder ( i.e : <Game Folder>/Game/Content/Paks ) ";
                gamePaksFolderPathTextBox.ForeColor = Color.Gray;
            }
        }

        private void gamePaksFolderPathTextBox_TextChanged(object sender, EventArgs e)
        {
            GameFilePath = gamePaksFolderPathTextBox.Text;
            int index = GameFilePath.IndexOf("Paks");
            if (index != -1)
            {
                string result = GameFilePath.Substring(0, index + "Paks".Length);
                GameFilePath = result;
            }

            if (Directory.Exists(GameFilePath + "\\~mods") && string.IsNullOrEmpty(modsFolderPathTextBox.Text))
            {
                ModFolderPath = GameFilePath + "\\~mods";
                ExportFolderPath = GameFilePath + "\\~mods";
            }
        }

        #endregion

        private void modsFolderPathTextBox_TextChanged(object sender, EventArgs e)
        {
            ModFolderPath = modsFolderPathTextBox.Text;
        }

        private void exportPathTextBox_TextChanged(object sender, EventArgs e)
        {
            ExportFolderPath = exportPathTextBox.Text;
        }

        #endregion
    }
}
