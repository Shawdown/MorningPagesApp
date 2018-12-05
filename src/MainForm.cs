using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using Timer = System.Timers.Timer;

namespace MorningPagesApp
{
    public partial class MainForm : Form, IConfigListener
    {
        private Timer autosaveTimer;
        private Timer cloudSyncTimer;
        private static DropboxManager dm;

        public MainForm()
        {
            InitializeComponent();

            KeyPreview = true;

            ConfigManager.SubscribeToUpdates(this);

            autosaveTimer = new Timer(int.Parse(ConfigManager.ReadSetting(ConfigKeys.AUTOSAVE_INTERVAL_SEC)) * 1000.0);
            autosaveTimer.Elapsed += OnAutosaveTimer;
            autosaveTimer.AutoReset = true;
            autosaveTimer.Enabled = true;

            cloudSyncTimer = new Timer(int.Parse(ConfigManager.ReadSetting(ConfigKeys.DROPBOX_SYNC_INTERVAL_SEC)) * 1000.0);
            cloudSyncTimer.Elapsed += OnCloudSyncTimer;
            cloudSyncTimer.AutoReset = true;
            cloudSyncTimer.Enabled = true;
        }

        private void rtbText_TextChanged(object sender, EventArgs e)
        {
            rtbText.Font = new Font(new FontFamily(ConfigManager.ReadSetting(ConfigKeys.FONT_NAME)), float.Parse(ConfigManager.ReadSetting(ConfigKeys.FONT_SIZE)));

            var wordsCount = rtbText.Text.Split(new[] {' ', '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries).Length;
            var wordGoal = int.Parse(ConfigManager.ReadSetting(ConfigKeys.WORD_GOAL));

            lCurrentProgress.Text = wordsCount + "/" + ConfigManager.ReadSetting(ConfigKeys.WORD_GOAL);

            if (wordsCount >= wordGoal)
            {
                setFormBackgroundColor(Color.FromName(ConfigManager.ReadSetting(ConfigKeys.GOAL_REACHED_BACKGROUND_COLOR)));
            }
            else
            {
                setFormBackgroundColor(Color.FromName(ConfigManager.ReadSetting(ConfigKeys.BACKGROUND_COLOR)));
            }
        }

        private void menu_bSettings_Click(object sender, EventArgs e)
        {
            SettingsForm sf = new SettingsForm();
            sf.ShowDialog();
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            rtbText.Font = new Font(new FontFamily(ConfigManager.ReadSetting(ConfigKeys.FONT_NAME)), float.Parse(ConfigManager.ReadSetting(ConfigKeys.FONT_SIZE)));
        }

        private void OnCloudSyncTimer(Object source, ElapsedEventArgs e)
        {
            Console.WriteLine("OnCloudSyncTimer event was raised");

            string data = (string)rtbText.Invoke(new Func<string>(() => rtbText.Text));

            try
            {
                if (ConfigManager.ReadBoolSetting(ConfigKeys.DROPBOX_ENABLED))
                {
                    Task.Run<Task>(async () => await dm.Upload(Program.GetTodayFileName(), data));

                    Console.WriteLine("Dropbox upload finished");
                }
            }
            catch (SystemException ex)
            {
                Program.log.Info(ex.Message);
            }

        }

        private void OnAutosaveTimer(Object source, ElapsedEventArgs e)
        {
            string data = (string)rtbText.Invoke(new Func<string>(() => rtbText.Text));

            string filePath = ConfigManager.ReadSetting(ConfigKeys.PAGES_SAVE_DIRECTORY) + "\\" + Program.GetTodayFileName();

            try
            {
                File.WriteAllText(filePath, data);
            }
            catch (IOException ex)
            {
                Program.log.Info(ex.Message);
            }

            Console.WriteLine("OnAutosaveTimer event was raised");
        }

        public void OnConfigChanged(string key, string newValue)
        {
            try
            {
                if (key == ConfigKeys.SHOW_CURRENT_PROGRESS)
                {
                    try
                    {
                        lCurrentProgress.Visible = bool.Parse(newValue);
                    }
                    catch (SystemException e)
                    {
                        Program.log.Info(e.Message);
                    }
                }
                if (key == ConfigKeys.AUTOSAVE_INTERVAL_SEC)
                {
                    autosaveTimer.Stop();

                    autosaveTimer.Interval = int.Parse(newValue) * 1000.0;

                    autosaveTimer.Start();
                }
                else if (key == ConfigKeys.DROPBOX_SYNC_INTERVAL_SEC)
                {
                    cloudSyncTimer.Stop();

                    cloudSyncTimer.Interval = int.Parse(newValue) * 1000.0;

                    cloudSyncTimer.Start();
                }
                else if (key == ConfigKeys.DROPBOX_ENABLED)
                {
                    if (bool.Parse(newValue))
                    {
                        dm = new DropboxManager();
                        dm.Init();
                    }
                    else
                    {
                        dm = null;
                    }
                }
            }
            catch (SystemException e)
            {
                Program.log.Info(e.Message);
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                lCurrentProgress.Visible = ConfigManager.ReadBoolSetting(ConfigKeys.SHOW_CURRENT_PROGRESS);
            }
            catch (SystemException ex)
            {
                Program.log.Info(ex.Message);

                lCurrentProgress.Visible = true;
            }

            string data = null;

            try
            {
                data = File.ReadAllText(ConfigManager.ReadSetting(ConfigKeys.PAGES_SAVE_DIRECTORY) + "\\" + Program.GetTodayFileName());
            }
            catch (SystemException ex)
            {
                Program.log.Info(ex.Message);

                lCurrentProgress.Text = "0/" + ConfigManager.ReadSetting(ConfigKeys.WORD_GOAL);
            }

            rtbText.Text = data;

            var wordsCount = rtbText.Text.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;

            lCurrentProgress.Text = wordsCount + "/" + ConfigManager.ReadSetting(ConfigKeys.WORD_GOAL);

            try
            {
                if (ConfigManager.ReadBoolSetting(ConfigKeys.DROPBOX_ENABLED))
                {
                    dm = new DropboxManager();
                    dm.Init();
                }
                else
                {
                    dm = null;
                }
            }
            catch (SystemException ex)
            {
                Program.log.Info(ex.Message);
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            autosaveTimer.Stop();
            OnAutosaveTimer(null, null);
            cloudSyncTimer.Stop();
            OnCloudSyncTimer(null, null);
        }

        private void rtbText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.S)
            {
                autosaveTimer.Stop();
                OnAutosaveTimer(null, null);
                autosaveTimer.Start();
            }
        }

        private void setFormBackgroundColor(Color color)
        {
            BackColor = color;
            menuStrip1.BackColor = color;
        }
    }
}
