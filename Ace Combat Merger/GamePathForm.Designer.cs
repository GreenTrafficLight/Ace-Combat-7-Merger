
namespace Ace_Combat_Merger
{
    partial class GamePathForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            gamePaksFolderPathButton = new Button();
            gamePaksFolderPathLabel = new Label();
            gamePaksFolderPathTextBox = new TextBox();
            cancelButton = new Button();
            okButton = new Button();
            modsFolderPathTextBox = new TextBox();
            modsFolderPathLabel = new Label();
            modsFolderPathButton = new Button();
            exportPathTextBox = new TextBox();
            exportPathLabel = new Label();
            exportPathLabelButton = new Button();
            pakProgressBar = new ProgressBar();
            PakProgresLabel = new Label();
            SuspendLayout();
            // 
            // gamePaksFolderPathButton
            // 
            gamePaksFolderPathButton.Location = new Point(639, 25);
            gamePaksFolderPathButton.Margin = new Padding(4, 3, 4, 3);
            gamePaksFolderPathButton.Name = "gamePaksFolderPathButton";
            gamePaksFolderPathButton.Size = new Size(28, 24);
            gamePaksFolderPathButton.TabIndex = 0;
            gamePaksFolderPathButton.Text = "...";
            gamePaksFolderPathButton.UseVisualStyleBackColor = true;
            gamePaksFolderPathButton.Click += gamePaksFolderPathButton_Click;
            // 
            // gamePaksFolderPathLabel
            // 
            gamePaksFolderPathLabel.AutoSize = true;
            gamePaksFolderPathLabel.Location = new Point(10, 9);
            gamePaksFolderPathLabel.Margin = new Padding(4, 0, 4, 0);
            gamePaksFolderPathLabel.Name = "gamePaksFolderPathLabel";
            gamePaksFolderPathLabel.Size = new Size(108, 15);
            gamePaksFolderPathLabel.TabIndex = 1;
            gamePaksFolderPathLabel.Text = "Path to game files :";
            // 
            // gamePaksFolderPathTextBox
            // 
            gamePaksFolderPathTextBox.ForeColor = SystemColors.InactiveCaptionText;
            gamePaksFolderPathTextBox.Location = new Point(10, 27);
            gamePaksFolderPathTextBox.Margin = new Padding(4, 3, 4, 3);
            gamePaksFolderPathTextBox.Name = "gamePaksFolderPathTextBox";
            gamePaksFolderPathTextBox.Size = new Size(625, 23);
            gamePaksFolderPathTextBox.TabIndex = 2;
            gamePaksFolderPathTextBox.Text = "Game paks folder ( i.e : <Game Folder>/Game/Content/Paks ) ";
            gamePaksFolderPathTextBox.TextChanged += gamePaksFolderPathTextBox_TextChanged;
            gamePaksFolderPathTextBox.Enter += gamePaksFolderPathTextBox_Enter;
            gamePaksFolderPathTextBox.Leave += gamePaksFolderPathTextBox_Leave;
            // 
            // cancelButton
            // 
            cancelButton.Location = new Point(581, 156);
            cancelButton.Margin = new Padding(4, 3, 4, 3);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(88, 27);
            cancelButton.TabIndex = 3;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // okButton
            // 
            okButton.Location = new Point(485, 156);
            okButton.Margin = new Padding(4, 3, 4, 3);
            okButton.Name = "okButton";
            okButton.Size = new Size(88, 27);
            okButton.TabIndex = 4;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // modsFolderPathTextBox
            // 
            modsFolderPathTextBox.Location = new Point(10, 71);
            modsFolderPathTextBox.Margin = new Padding(4, 3, 4, 3);
            modsFolderPathTextBox.Name = "modsFolderPathTextBox";
            modsFolderPathTextBox.Size = new Size(625, 23);
            modsFolderPathTextBox.TabIndex = 5;
            modsFolderPathTextBox.TextChanged += modsFolderPathTextBox_TextChanged;
            // 
            // modsFolderPathLabel
            // 
            modsFolderPathLabel.AutoSize = true;
            modsFolderPathLabel.Location = new Point(10, 53);
            modsFolderPathLabel.Margin = new Padding(4, 0, 4, 0);
            modsFolderPathLabel.Name = "modsFolderPathLabel";
            modsFolderPathLabel.Size = new Size(118, 15);
            modsFolderPathLabel.TabIndex = 6;
            modsFolderPathLabel.Text = "Path to mods folder :";
            // 
            // modsFolderPathButton
            // 
            modsFolderPathButton.Location = new Point(639, 69);
            modsFolderPathButton.Margin = new Padding(4, 3, 4, 3);
            modsFolderPathButton.Name = "modsFolderPathButton";
            modsFolderPathButton.Size = new Size(28, 24);
            modsFolderPathButton.TabIndex = 7;
            modsFolderPathButton.Text = "...";
            modsFolderPathButton.UseVisualStyleBackColor = true;
            modsFolderPathButton.Click += modsFolderPathButton_Click;
            // 
            // exportPathTextBox
            // 
            exportPathTextBox.Location = new Point(10, 112);
            exportPathTextBox.Margin = new Padding(4, 3, 4, 3);
            exportPathTextBox.Name = "exportPathTextBox";
            exportPathTextBox.Size = new Size(625, 23);
            exportPathTextBox.TabIndex = 8;
            exportPathTextBox.TextChanged += exportPathTextBox_TextChanged;
            // 
            // exportPathLabel
            // 
            exportPathLabel.AutoSize = true;
            exportPathLabel.Location = new Point(10, 97);
            exportPathLabel.Margin = new Padding(4, 0, 4, 0);
            exportPathLabel.Name = "exportPathLabel";
            exportPathLabel.Size = new Size(73, 15);
            exportPathLabel.TabIndex = 9;
            exportPathLabel.Text = "Export Path :";
            // 
            // exportPathLabelButton
            // 
            exportPathLabelButton.Location = new Point(639, 112);
            exportPathLabelButton.Margin = new Padding(4, 3, 4, 3);
            exportPathLabelButton.Name = "exportPathLabelButton";
            exportPathLabelButton.Size = new Size(28, 24);
            exportPathLabelButton.TabIndex = 10;
            exportPathLabelButton.Text = "...";
            exportPathLabelButton.UseVisualStyleBackColor = true;
            exportPathLabelButton.Click += exportPathButton_Click;
            // 
            // pakProgressBar
            // 
            pakProgressBar.Location = new Point(10, 156);
            pakProgressBar.Name = "pakProgressBar";
            pakProgressBar.Size = new Size(468, 27);
            pakProgressBar.TabIndex = 11;
            // 
            // PakProgresLabel
            // 
            PakProgresLabel.AutoSize = true;
            PakProgresLabel.Location = new Point(10, 138);
            PakProgresLabel.Name = "PakProgresLabel";
            PakProgresLabel.Size = new Size(38, 15);
            PakProgresLabel.TabIndex = 12;
            PakProgresLabel.Text = "label1";
            PakProgresLabel.Visible = false;
            // 
            // GamePathForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(681, 218);
            Controls.Add(PakProgresLabel);
            Controls.Add(pakProgressBar);
            Controls.Add(exportPathLabelButton);
            Controls.Add(exportPathLabel);
            Controls.Add(exportPathTextBox);
            Controls.Add(modsFolderPathButton);
            Controls.Add(modsFolderPathLabel);
            Controls.Add(modsFolderPathTextBox);
            Controls.Add(okButton);
            Controls.Add(cancelButton);
            Controls.Add(gamePaksFolderPathTextBox);
            Controls.Add(gamePaksFolderPathLabel);
            Controls.Add(gamePaksFolderPathButton);
            Margin = new Padding(4, 3, 4, 3);
            Name = "GamePathForm";
            Text = "GamePathForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button gamePaksFolderPathButton;
        private Label gamePaksFolderPathLabel;
        private TextBox gamePaksFolderPathTextBox;
        private Button cancelButton;
        private Button okButton;
        private TextBox modsFolderPathTextBox;
        private Label modsFolderPathLabel;
        private Button modsFolderPathButton;
        private TextBox exportPathTextBox;
        private Label exportPathLabel;
        private Button exportPathLabelButton;
        private ProgressBar pakProgressBar;
        private Label PakProgresLabel;
    }
}