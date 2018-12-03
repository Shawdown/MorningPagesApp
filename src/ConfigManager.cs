using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace MorningPagesApp
{
    public class ConfigKeys
    {
        public const string WORD_GOAL = "word_goal";
        public const string PAGES_SAVE_DIRECTORY = "pages_save_dir";
        public const string FONT_NAME = "font_name";
        public const string FONT_SIZE = "font_size";
        public const string FONT_COLOR = "font_color";
        public const string BACKGROUND_COLOR = "background_color";
        public const string AUTOSAVE_INTERVAL_SEC = "autosave_interval_sec";
        public const string SHOW_CURRENT_PROGRESS = "show_current_progress";
        public const string GOAL_REACHED_BACKGROUND_COLOR = "goal_reached_background_color";
        public const string DROPBOX_ACCESS_TOKEN = "dropbox_access_token";
        public const string DROPBOX_UID = "dropbox_uid";
        public const string DROPBOX_ENABLED = "dropbox_enabled";
        public const string DROPBOX_SYNC_INTERVAL_SEC = "dropbox_sync_interval_sec";
    }

    public class ConfigManager
    {
        private static readonly HashSet<IConfigListener> Listeners = new HashSet<IConfigListener>();

        public static void SubscribeToUpdates(IConfigListener listener)
        {
            Listeners.Add(listener);
        }

        public static void UnsubscribeFromUpdates(IConfigListener listener)
        {
            Listeners.Remove(listener);
        }

        public static bool ReadBoolSetting(string key)
        {
            return bool.Parse(ReadSetting(key));
        }

        public static string ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                return appSettings[key];
            }
            catch (ConfigurationErrorsException)
            {
                Program.log.Info("Error reading app settings: key=" + key);
            }

            return null;
        }

        public static void UpdateSetting(string key, string value)
        {
            try
            {
                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var settings = configFile.AppSettings.Settings;
                if (settings[key] == null)
                {
                    settings.Add(key, value);
                }
                else
                {
                    if (settings[key].Value == value)
                    {
                        // Values are the same.
                        return;
                    }

                    settings[key].Value = value;
                }
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);

                NotifyListeners(key, value);
            }
            catch (ConfigurationErrorsException)
            {
                Program.log.Info("Error writing app settings: key=" + key + ", value=" + value);
            }
        }

        private static void NotifyListeners(string key, string newValue)
        {
            foreach (var l in Listeners)
            {
                l.OnConfigChanged(key, newValue);
            }
        }
    }
}