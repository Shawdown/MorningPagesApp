using System;
using System.Windows.Forms;

namespace MorningPagesApp
{
    public partial class SettingsForm : Form
    {
        private SettingsFormTreeMainPanel tvMainPanel = new SettingsFormTreeMainPanel();
        private SettingsFormTreeDropboxPanel tvDropboxPanel = new SettingsFormTreeDropboxPanel();
        private UserControl activePanel;
        private bool statusChanged = false;


        public SettingsForm()
        {
            InitializeComponent();
        }

        private void tvSettings_AfterSelect(object sender, TreeViewEventArgs e)
        {
            UserControl newPanel = null;

            if (e.Node.Level == 0) // nodes w/o children
            {
                if (e.Node.Index == 0) // Main
                {
                    newPanel = tvMainPanel;
                }
            }
            else if (e.Node.Level == 1) // nodes with children
            {
                if (e.Node.Parent.Index == 1) // Cloud sync
                {
                    if (e.Node.Index == 0) // Dropbox
                    {
                        newPanel = tvDropboxPanel;
                    }
                }
            }

            if (activePanel != null)
            {
                activePanel.Hide();
                Controls.Remove(activePanel);
            }

            if (newPanel != null)
            {
                newPanel.Show();
                newPanel.Dock = DockStyle.Fill;
                tlpSettings.Controls.Add(newPanel);
                tlpSettings.SetColumnSpan(newPanel, 2);

                activePanel = newPanel;
            }
        }

        private void SettingsForm_Load(object sender, System.EventArgs e)
        {
            tvMainPanel.LoadSettings();
            tvDropboxPanel.LoadSettings();

            tvMainPanel.Visible = false;
            tvDropboxPanel.Visible = false;

            tvSettings.SelectedNode = tvSettings.Nodes[0];

            AddEnableSaveOnChanges(tvMainPanel);
            AddEnableSaveOnChanges(tvDropboxPanel);
        }

        private void AddEnableSaveOnChanges(Control obj)
        {
            foreach (Control control in obj.Controls)
            {
                control.TextChanged += EnableSave;

                // some events can occurs with your controls inside panel
                if (control.GetType() == typeof(CheckBox))
                {
                    ((CheckBox)control).CheckedChanged += EnableSave;
                }

                if (control.GetType() == typeof(ListView))
                {
                    ((ListView)control).SelectedIndexChanged += EnableSave;
                }

                // containers
                if (control.Controls.Count > 0)
                    AddEnableSaveOnChanges(control);
            }
        }

        private void EnableSave(object sender, EventArgs e)
        {
            statusChanged = true;
            bSave.Enabled = true;
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            if (activePanel != null)
            {
                ISettingsControlPanel cp = (ISettingsControlPanel) activePanel;
                if (cp.IsValid())
                {
                    cp.Save();
                }
                else
                {
                    MessageBox.Show("Some values are invalid.");
                    return;
                }
            }

            Dispose();
        }

        private void bClose_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}
