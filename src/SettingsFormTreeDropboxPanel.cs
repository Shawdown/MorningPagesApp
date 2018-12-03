using System;
using System.Windows.Forms;

namespace MorningPagesApp
{
    public class SettingsFormTreeDropboxPanel : UserControl, ISettingsControlPanel
    {
        private TextBox tbDropboxSyncIntervalSec;
        private Label lDropboxSyncInterval;
        private CheckBox cbUseDropbox;

        public SettingsFormTreeDropboxPanel()
        {
            InitializeComponent();
        }

        public bool IsValid()
        {
            try
            {
                if (cbUseDropbox.Checked)
                {
                    return tbDropboxSyncIntervalSec.Text.Length > 1 &&
                           int.Parse(tbDropboxSyncIntervalSec.Text) > 0;
                }

                return true;
            }
            catch (SystemException ex)
            {
                Program.log.Info(ex.Message);
            }

            return false;
        }

        public void Save()
        {
            ConfigManager.UpdateSetting(ConfigKeys.DROPBOX_ENABLED, cbUseDropbox.Checked.ToString());
            ConfigManager.UpdateSetting(ConfigKeys.DROPBOX_SYNC_INTERVAL_SEC, tbDropboxSyncIntervalSec.Text);
        }

        public void LoadSettings()
        {
            try
            {
                cbUseDropbox.Checked = bool.Parse(ConfigManager.ReadSetting(ConfigKeys.DROPBOX_ENABLED));
            }
            catch (SystemException ex)
            {
                Program.log.Info(ex.Message);
            }

            tbDropboxSyncIntervalSec.Text = ConfigManager.ReadSetting(ConfigKeys.DROPBOX_SYNC_INTERVAL_SEC);
        }

        private void InitializeComponent()
        {
            this.cbUseDropbox = new System.Windows.Forms.CheckBox();
            this.tbDropboxSyncIntervalSec = new System.Windows.Forms.TextBox();
            this.lDropboxSyncInterval = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbUseDropbox
            // 
            this.cbUseDropbox.AutoSize = true;
            this.cbUseDropbox.Location = new System.Drawing.Point(179, 26);
            this.cbUseDropbox.Name = "cbUseDropbox";
            this.cbUseDropbox.Size = new System.Drawing.Size(94, 17);
            this.cbUseDropbox.TabIndex = 3;
            this.cbUseDropbox.Text = "Use Dropbox?";
            this.cbUseDropbox.UseVisualStyleBackColor = true;
            // 
            // tbDropboxSyncIntervalSec
            // 
            this.tbDropboxSyncIntervalSec.Location = new System.Drawing.Point(130, 53);
            this.tbDropboxSyncIntervalSec.Name = "tbDropboxSyncIntervalSec";
            this.tbDropboxSyncIntervalSec.Size = new System.Drawing.Size(175, 20);
            this.tbDropboxSyncIntervalSec.TabIndex = 5;
            // 
            // lDropboxSyncInterval
            // 
            this.lDropboxSyncInterval.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lDropboxSyncInterval.AutoSize = true;
            this.lDropboxSyncInterval.Location = new System.Drawing.Point(27, 56);
            this.lDropboxSyncInterval.Name = "lDropboxSyncInterval";
            this.lDropboxSyncInterval.Size = new System.Drawing.Size(97, 13);
            this.lDropboxSyncInterval.TabIndex = 4;
            this.lDropboxSyncInterval.Text = "Sync interval (sec):";
            this.lDropboxSyncInterval.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SettingsFormTreeDropboxPanel
            // 
            this.Controls.Add(this.tbDropboxSyncIntervalSec);
            this.Controls.Add(this.lDropboxSyncInterval);
            this.Controls.Add(this.cbUseDropbox);
            this.Name = "SettingsFormTreeDropboxPanel";
            this.Size = new System.Drawing.Size(370, 200);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}