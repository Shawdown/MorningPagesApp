using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace MorningPagesApp
{
    public class SettingsFormTreePanel1 : UserControl, ISettingsControlPanel
    {
        private Label lWordGoal;
        private TextBox tbPagesSaveDir;
        private Label lPagesSaveDir;
        private TextBox tbFont;
        private Label lFont;
        private TextBox tbFontSize;
        private Label lFontSize;
        private TextBox tbAutosaveInterval;
        private Label lAutosaveInterval;
        private Button bChooseFont;
        private Button bChoosePagesSaveDirectory;
        private TextBox tbWordGoal;

        public bool IsValid()
        {
            try
            {
                return tbPagesSaveDir.Text.Length > 0 &&
                       tbFont.Text.Length > 0 &&
                       tbFontSize.Text.Length > 0 &&
                       tbAutosaveInterval.Text.Length > 0 &&
                       tbWordGoal.Text.Length > 0 &&
                       int.Parse(tbAutosaveInterval.Text) > 1 &&
                       int.Parse(tbWordGoal.Text) > 0 &&
                       Regex.IsMatch(tbFontSize.Text, "\\d+,?\\d*") &&
                       Directory.Exists(tbPagesSaveDir.Text);
            }
            catch (FormatException ex)
            {
                Program.log.Info(ex);
            }

            return false;
        }

        public void Save()
        {
            ConfigManager.UpdateSetting(ConfigKeys.WORD_GOAL, tbWordGoal.Text);
            ConfigManager.UpdateSetting(ConfigKeys.PAGES_SAVE_DIRECTORY, tbPagesSaveDir.Text);
            ConfigManager.UpdateSetting(ConfigKeys.FONT_NAME, tbFont.Text);
            ConfigManager.UpdateSetting(ConfigKeys.FONT_SIZE, tbFontSize.Text);
            ConfigManager.UpdateSetting(ConfigKeys.AUTOSAVE_INTERVAL_SEC, tbAutosaveInterval.Text);
        }

        public void LoadSettings()
        {
            tbWordGoal.Text = ConfigManager.ReadSetting(ConfigKeys.WORD_GOAL);
            tbPagesSaveDir.Text = ConfigManager.ReadSetting(ConfigKeys.PAGES_SAVE_DIRECTORY);
            tbFont.Text = ConfigManager.ReadSetting(ConfigKeys.FONT_NAME);
            tbFontSize.Text = ConfigManager.ReadSetting(ConfigKeys.FONT_SIZE);
            tbAutosaveInterval.Text = ConfigManager.ReadSetting(ConfigKeys.AUTOSAVE_INTERVAL_SEC);
        }

        public SettingsFormTreePanel1()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.lWordGoal = new System.Windows.Forms.Label();
            this.tbWordGoal = new System.Windows.Forms.TextBox();
            this.tbPagesSaveDir = new System.Windows.Forms.TextBox();
            this.lPagesSaveDir = new System.Windows.Forms.Label();
            this.tbFont = new System.Windows.Forms.TextBox();
            this.lFont = new System.Windows.Forms.Label();
            this.tbFontSize = new System.Windows.Forms.TextBox();
            this.lFontSize = new System.Windows.Forms.Label();
            this.tbAutosaveInterval = new System.Windows.Forms.TextBox();
            this.lAutosaveInterval = new System.Windows.Forms.Label();
            this.bChooseFont = new System.Windows.Forms.Button();
            this.bChoosePagesSaveDirectory = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lWordGoal
            // 
            this.lWordGoal.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lWordGoal.AutoSize = true;
            this.lWordGoal.Location = new System.Drawing.Point(85, 26);
            this.lWordGoal.Name = "lWordGoal";
            this.lWordGoal.Size = new System.Drawing.Size(59, 13);
            this.lWordGoal.TabIndex = 0;
            this.lWordGoal.Text = "Word goal:";
            this.lWordGoal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbWordGoal
            // 
            this.tbWordGoal.Location = new System.Drawing.Point(150, 23);
            this.tbWordGoal.Name = "tbWordGoal";
            this.tbWordGoal.Size = new System.Drawing.Size(175, 20);
            this.tbWordGoal.TabIndex = 1;
            // 
            // tbPagesSaveDir
            // 
            this.tbPagesSaveDir.Location = new System.Drawing.Point(150, 49);
            this.tbPagesSaveDir.Name = "tbPagesSaveDir";
            this.tbPagesSaveDir.Size = new System.Drawing.Size(175, 20);
            this.tbPagesSaveDir.TabIndex = 3;
            // 
            // lPagesSaveDir
            // 
            this.lPagesSaveDir.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lPagesSaveDir.AutoSize = true;
            this.lPagesSaveDir.Location = new System.Drawing.Point(35, 52);
            this.lPagesSaveDir.Name = "lPagesSaveDir";
            this.lPagesSaveDir.Size = new System.Drawing.Size(109, 13);
            this.lPagesSaveDir.TabIndex = 2;
            this.lPagesSaveDir.Text = "Pages save directory:";
            this.lPagesSaveDir.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbFont
            // 
            this.tbFont.Enabled = false;
            this.tbFont.Location = new System.Drawing.Point(150, 75);
            this.tbFont.Name = "tbFont";
            this.tbFont.Size = new System.Drawing.Size(175, 20);
            this.tbFont.TabIndex = 5;
            // 
            // lFont
            // 
            this.lFont.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lFont.AutoSize = true;
            this.lFont.Location = new System.Drawing.Point(113, 78);
            this.lFont.Name = "lFont";
            this.lFont.Size = new System.Drawing.Size(31, 13);
            this.lFont.TabIndex = 4;
            this.lFont.Text = "Font:";
            this.lFont.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbFontSize
            // 
            this.tbFontSize.Location = new System.Drawing.Point(150, 101);
            this.tbFontSize.Name = "tbFontSize";
            this.tbFontSize.Size = new System.Drawing.Size(175, 20);
            this.tbFontSize.TabIndex = 7;
            // 
            // lFontSize
            // 
            this.lFontSize.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lFontSize.AutoSize = true;
            this.lFontSize.Location = new System.Drawing.Point(92, 104);
            this.lFontSize.Name = "lFontSize";
            this.lFontSize.Size = new System.Drawing.Size(52, 13);
            this.lFontSize.TabIndex = 6;
            this.lFontSize.Text = "Font size:";
            this.lFontSize.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbAutosaveInterval
            // 
            this.tbAutosaveInterval.Location = new System.Drawing.Point(150, 127);
            this.tbAutosaveInterval.Name = "tbAutosaveInterval";
            this.tbAutosaveInterval.Size = new System.Drawing.Size(175, 20);
            this.tbAutosaveInterval.TabIndex = 9;
            // 
            // lAutosaveInterval
            // 
            this.lAutosaveInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lAutosaveInterval.AutoSize = true;
            this.lAutosaveInterval.Location = new System.Drawing.Point(26, 130);
            this.lAutosaveInterval.Name = "lAutosaveInterval";
            this.lAutosaveInterval.Size = new System.Drawing.Size(118, 13);
            this.lAutosaveInterval.TabIndex = 8;
            this.lAutosaveInterval.Text = "Autosave interval (sec):";
            this.lAutosaveInterval.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bChooseFont
            // 
            this.bChooseFont.Location = new System.Drawing.Point(326, 75);
            this.bChooseFont.Name = "bChooseFont";
            this.bChooseFont.Size = new System.Drawing.Size(26, 20);
            this.bChooseFont.TabIndex = 10;
            this.bChooseFont.Text = "...";
            this.bChooseFont.UseVisualStyleBackColor = true;
            this.bChooseFont.Click += new System.EventHandler(this.bChooseFont_Click);
            // 
            // bChoosePagesSaveDirectory
            // 
            this.bChoosePagesSaveDirectory.Location = new System.Drawing.Point(326, 49);
            this.bChoosePagesSaveDirectory.Name = "bChoosePagesSaveDirectory";
            this.bChoosePagesSaveDirectory.Size = new System.Drawing.Size(26, 20);
            this.bChoosePagesSaveDirectory.TabIndex = 11;
            this.bChoosePagesSaveDirectory.Text = "...";
            this.bChoosePagesSaveDirectory.UseVisualStyleBackColor = true;
            this.bChoosePagesSaveDirectory.Click += new System.EventHandler(this.bChoosePagesSaveDirectory_Click);
            // 
            // SettingsFormTreePanel1
            // 
            this.Controls.Add(this.bChoosePagesSaveDirectory);
            this.Controls.Add(this.bChooseFont);
            this.Controls.Add(this.tbAutosaveInterval);
            this.Controls.Add(this.lAutosaveInterval);
            this.Controls.Add(this.tbFontSize);
            this.Controls.Add(this.lFontSize);
            this.Controls.Add(this.tbFont);
            this.Controls.Add(this.lFont);
            this.Controls.Add(this.tbPagesSaveDir);
            this.Controls.Add(this.lPagesSaveDir);
            this.Controls.Add(this.tbWordGoal);
            this.Controls.Add(this.lWordGoal);
            this.Name = "SettingsFormTreePanel1";
            this.Size = new System.Drawing.Size(371, 177);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void bChooseFont_Click(object sender, System.EventArgs e)
        {
            FontDialog fd = new FontDialog();
            float fSize = 10.0f;
            try
            {
                fSize = float.Parse(tbFontSize.Text);
            }
            catch (FormatException ex)
            {
                Program.log.Info(ex.Message);
            }

            fd.Font = new Font(new FontFamily(tbFont.Text), fSize);
            fd.Color = Color.FromName(ConfigManager.ReadSetting(ConfigKeys.FONT_COLOR));

            var dr = fd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                tbFont.Text = fd.Font.Name;
                tbFontSize.Text = fd.Font.SizeInPoints.ToString();
            }
        }

        private void bChoosePagesSaveDirectory_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = tbPagesSaveDir.Text;
            var dr = fbd.ShowDialog();

            if (dr == DialogResult.OK)
            {
                tbPagesSaveDir.Text = fbd.SelectedPath;
            }
        }
    }
}