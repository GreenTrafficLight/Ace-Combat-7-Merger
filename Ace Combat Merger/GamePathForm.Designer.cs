
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
            SuspendLayout();
            // 
            // gamePaksFolderPathButton
            // 
            gamePaksFolderPathButton.Location = new Point(730, 33);
            gamePaksFolderPathButton.Margin = new Padding(5, 4, 5, 4);
            gamePaksFolderPathButton.Name = "gamePaksFolderPathButton";
            gamePaksFolderPathButton.Size = new Size(32, 32);
            gamePaksFolderPathButton.TabIndex = 0;
            gamePaksFolderPathButton.Text = "...";
            gamePaksFolderPathButton.UseVisualStyleBackColor = true;
            gamePaksFolderPathButton.Click += gamePaksFolderPathButton_Click;
            // 
            // gamePaksFolderPathLabel
            // 
            gamePaksFolderPathLabel.AutoSize = true;
            gamePaksFolderPathLabel.Location = new Point(11, 12);
            gamePaksFolderPathLabel.Margin = new Padding(5, 0, 5, 0);
            gamePaksFolderPathLabel.Name = "gamePaksFolderPathLabel";
            gamePaksFolderPathLabel.Size = new Size(135, 20);
            gamePaksFolderPathLabel.TabIndex = 1;
            gamePaksFolderPathLabel.Text = "Path to game files :";
            // 
            // gamePaksFolderPathTextBox
            // 
            gamePaksFolderPathTextBox.ForeColor = SystemColors.InactiveCaptionText;
            gamePaksFolderPathTextBox.Location = new Point(11, 36);
            gamePaksFolderPathTextBox.Margin = new Padding(5, 4, 5, 4);
            gamePaksFolderPathTextBox.Name = "gamePaksFolderPathTextBox";
            gamePaksFolderPathTextBox.Size = new Size(714, 27);
            gamePaksFolderPathTextBox.TabIndex = 2;
            gamePaksFolderPathTextBox.Text = "Game paks folder ( i.e : <Game Folder>/Game/Content/Paks ) ";
            gamePaksFolderPathTextBox.TextChanged += gamePaksFolderPathTextBox_TextChanged;
            gamePaksFolderPathTextBox.Enter += gamePaksFolderPathTextBox_Enter;
            gamePaksFolderPathTextBox.Leave += gamePaksFolderPathTextBox_Leave;
            // 
            // cancelButton
            // 
            cancelButton.Location = new Point(663, 219);
            cancelButton.Margin = new Padding(5, 4, 5, 4);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(101, 36);
            cancelButton.TabIndex = 3;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // okButton
            // 
            okButton.Location = new Point(554, 219);
            okButton.Margin = new Padding(5, 4, 5, 4);
            okButton.Name = "okButton";
            okButton.Size = new Size(101, 36);
            okButton.TabIndex = 4;
            okButton.Text = "OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += okButton_Click;
            // 
            // modsFolderPathTextBox
            // 
            modsFolderPathTextBox.Location = new Point(11, 95);
            modsFolderPathTextBox.Margin = new Padding(5, 4, 5, 4);
            modsFolderPathTextBox.Name = "modsFolderPathTextBox";
            modsFolderPathTextBox.Size = new Size(714, 27);
            modsFolderPathTextBox.TabIndex = 5;
            modsFolderPathTextBox.TextChanged += modsFolderPathTextBox_TextChanged;
            // 
            // modsFolderPathLabel
            // 
            modsFolderPathLabel.AutoSize = true;
            modsFolderPathLabel.Location = new Point(11, 71);
            modsFolderPathLabel.Margin = new Padding(5, 0, 5, 0);
            modsFolderPathLabel.Name = "modsFolderPathLabel";
            modsFolderPathLabel.Size = new Size(147, 20);
            modsFolderPathLabel.TabIndex = 6;
            modsFolderPathLabel.Text = "Path to mods folder :";
            // 
            // modsFolderPathButton
            // 
            modsFolderPathButton.Location = new Point(730, 92);
            modsFolderPathButton.Margin = new Padding(5, 4, 5, 4);
            modsFolderPathButton.Name = "modsFolderPathButton";
            modsFolderPathButton.Size = new Size(32, 32);
            modsFolderPathButton.TabIndex = 7;
            modsFolderPathButton.Text = "...";
            modsFolderPathButton.UseVisualStyleBackColor = true;
            modsFolderPathButton.Click += modsFolderPathButton_Click;
            // 
            // exportPathTextBox
            // 
            exportPathTextBox.Location = new Point(11, 153);
            exportPathTextBox.Margin = new Padding(5, 4, 5, 4);
            exportPathTextBox.Name = "exportPathTextBox";
            exportPathTextBox.Size = new Size(714, 27);
            exportPathTextBox.TabIndex = 8;
            exportPathTextBox.TextChanged += exportPathTextBox_TextChanged;
            // 
            // exportPathLabel
            // 
            exportPathLabel.AutoSize = true;
            exportPathLabel.Location = new Point(11, 129);
            exportPathLabel.Margin = new Padding(5, 0, 5, 0);
            exportPathLabel.Name = "exportPathLabel";
            exportPathLabel.Size = new Size(91, 20);
            exportPathLabel.TabIndex = 9;
            exportPathLabel.Text = "Export Path :";
            // 
            // exportPathLabelButton
            // 
            exportPathLabelButton.Location = new Point(730, 150);
            exportPathLabelButton.Margin = new Padding(5, 4, 5, 4);
            exportPathLabelButton.Name = "exportPathLabelButton";
            exportPathLabelButton.Size = new Size(32, 32);
            exportPathLabelButton.TabIndex = 10;
            exportPathLabelButton.Text = "...";
            exportPathLabelButton.UseVisualStyleBackColor = true;
            exportPathLabelButton.Click += exportPathButton_Click;
            // 
            // GamePathForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(778, 272);
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
            Margin = new Padding(5, 4, 5, 4);
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
    }
}