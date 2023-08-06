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

namespace Ace_Combat_Merger
{
    public partial class GamePathForm : Form
    {
        private string _GameFilePath = "";
        private string _ModFolderPath = "";
        private string _ExportFolderPath = "";

        private ModManager _ModManager;

        public string GameFilePath
        {
            get { return _GameFilePath; }
            set { if (_GameFilePath != value) _GameFilePath = value; }
        }

        public string ModFolderPath
        {
            get { return _ModFolderPath; }
            set { if (_ModFolderPath != value) _ModFolderPath = value; }
        }

        public string ExportFolderPath
        {
            get { return _ExportFolderPath + "\\temp"; }
            set { if (_ExportFolderPath != value) _ExportFolderPath = value; }
        }
        public GamePathForm()
        {
            InitializeComponent();

#if DEBUG
            GameFilePath = "E:\\Program Files(x86)\\Steam\\steamapps\\common\\ACE COMBAT 7\\Game\\Content\\Paks";
#endif
        }

        #region button

        private void gamePaksFolderPathButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    _GameFilePath = fbd.SelectedPath;
                    gamePaksFolderPathTextBox.Text = _GameFilePath;
                    if (Directory.Exists(GameFilePath + "\\~mods"))
                    {
                        _ModFolderPath = GameFilePath + "\\~mods";
                        modsFolderPathTextBox.Text = _ModFolderPath;
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
                    _ModFolderPath = fbd.SelectedPath;
                    modsFolderPathTextBox.Text = _ModFolderPath;
                }
            }
        }

        private void exportPathButton_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    _ExportFolderPath = fbd.SelectedPath;
                    exportPathTextBox.Text = _ExportFolderPath;
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
            _GameFilePath = gamePaksFolderPathTextBox.Text;
            if (Directory.Exists(GameFilePath + "\\~mods") && string.IsNullOrEmpty(modsFolderPathTextBox.Text))
            {
                _ModFolderPath = GameFilePath + "\\~mods";
                modsFolderPathTextBox.Text = _ModFolderPath;
                exportPathTextBox.Text = _ModFolderPath;
                _ExportFolderPath = _ModFolderPath;
            }
        }

        #endregion

        private void modsFolderPathTextBox_TextChanged(object sender, EventArgs e)
        {
            _ModFolderPath = modsFolderPathTextBox.Text;
        }

        private void exportPathTextBox_TextChanged(object sender, EventArgs e)
        {
            _ExportFolderPath = exportPathTextBox.Text;
        }

        #endregion
    }
}
