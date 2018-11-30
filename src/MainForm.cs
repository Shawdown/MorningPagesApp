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

        public MainForm()
        {
            InitializeComponent();

            KeyPreview = true;

            ConfigManager.SubscribeToUpdates(this);

            autosaveTimer = new Timer(int.Parse(ConfigManager.ReadSetting(ConfigKeys.AUTOSAVE_INTERVAL_SEC)) * 1000.0);
            autosaveTimer.Elapsed += OnAutosaveTimer;
            autosaveTimer.AutoReset = true;
            autosaveTimer.Enabled = true;
        }

        private void rtbText_TextChanged(object sender, EventArgs e)
        {
            rtbText.Font = new Font(new FontFamily(ConfigManager.ReadSetting(ConfigKeys.FONT_NAME)), float.Parse(ConfigManager.ReadSetting(ConfigKeys.FONT_SIZE)));

            var wordsCount = rtbText.Text.Split(new[] {' ', '\r', '\n'}, StringSplitOptions.RemoveEmptyEntries).Length;

            lCurrentProgress.Text = wordsCount + "/" + ConfigManager.ReadSetting(ConfigKeys.WORD_GOAL);
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

            Console.WriteLine("The Elapsed event was raised");
        }

        public void OnConfigChanged(string key, string newValue)
        {
            if (key == ConfigKeys.AUTOSAVE_INTERVAL_SEC)
            {
                autosaveTimer.Stop();

                autosaveTimer.Interval = int.Parse(ConfigManager.ReadSetting(ConfigKeys.AUTOSAVE_INTERVAL_SEC)) * 1000.0;

                autosaveTimer.Start();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                string data = File.ReadAllText(ConfigManager.ReadSetting(ConfigKeys.PAGES_SAVE_DIRECTORY) + "\\" + Program.GetTodayFileName());

                rtbText.Text = data;

                var wordsCount = rtbText.Text.Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).Length;

                lCurrentProgress.Text = wordsCount + "/" + ConfigManager.ReadSetting(ConfigKeys.WORD_GOAL);
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
    }
}
