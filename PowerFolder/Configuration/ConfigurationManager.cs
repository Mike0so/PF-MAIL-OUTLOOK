using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using PowerFolder.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PowerFolder.Configuration
{
    public class ConfigurationManager
    {
        private Configuration _config;

        private Security.SecurityManager _security;
        private static ConfigurationManager _instance;

        private const string _classname = "[ConfigurationManager]";
        private const string _configFile = "config.json";

        private ConfigurationManager()
        {
        }

        public Configuration GetConfig()
        {
            InstallConfiguration();

            if (_config != null) {
                return _config;
            }
            using (StreamReader reader = new StreamReader(GetConfigurationPath())) 
            {
                JsonSerializer serializer = new JsonSerializer();
                Configuration deSerializedConfig = JsonConvert.DeserializeObject<Configuration>(reader.ReadToEnd());
                _config = deSerializedConfig;
                reader.Close();
                return _config;
            }
        }

        public void SaveConfig(Configuration configuration)
        {
            _config = configuration;
            File.WriteAllText(GetConfigurationPath(), JsonConvert.SerializeObject(_config));
            Log.LogThis("Configuration saved.", eloglevel.info);
        }

        private void InstallConfiguration()
        {
            Configuration config = null;
            const string _methodname = "[InstallConfiguration]";
            string _fileContent = string.Empty;
            try
            {
                using (StreamReader reader = new StreamReader(GetConfigurationPath()))
                {
                    _fileContent = reader.ReadToEnd();
                    reader.Close();
                }

                    if (string.IsNullOrEmpty(_fileContent))
                    {
                        config = new Configuration();
                        config.Username = string.Empty;
                        config.Password = string.Empty;
                        config.BaseUrl = @"https://my.powerfolder.com";
                        config.AddInID = "7be436757b194fb1b346250aabb3a97a";
                        config.TrackEmails = true;
                        config.FileLinkDialogEachEmail = true;
                        config.UseDefaultFileLinkConfig = false;
                        config.FileLinkDownloadCount = string.Empty;
                        config.FileLinkValidFor = string.Empty;
                    }
                    else
                    {
                        return;
                    }
                File.WriteAllText(GetConfigurationPath(), JsonConvert.SerializeObject(config));
                Log.LogThis("Successfuly installed the configuration.", eloglevel.info);
            }
            catch (Exception e)
            {
                Log.LogThis(string.Format("{0} {1} [Exception while installing the configuration. Exception : {2}]", _classname, _methodname, e.Message), eloglevel.error);
                return;
            }

        }

        private string GetConfigurationPath()
        {
            string _path = string.Format("{0}\\PowerFolder\\Outlook\\",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

            const string _methodname = "[GetConfigurationPath]";

            if (!Directory.Exists(_path))
            {
                try
                {
                    Directory.CreateDirectory(_path);
                }
                catch (Exception e) 
                {
                    Log.LogThis(string.Format("{0} {1} [Exception while creating the configuration path. Exception : {2}]", _classname, _methodname, e.Message), eloglevel.error);
                    return null;
                }
            }

            _path = _path + _configFile;

            if (!File.Exists(_path))
            {
                try
                {
                    File.Create(_path).Close();
                }
                catch (Exception e)
                {
                    Log.LogThis(string.Format("{0} {1} [Exception while creation the configuration file. Exception : {2}]", _classname, _methodname, e.Message), eloglevel.error);
                    return null;
                }
            }
            return _path;
        }

        public bool SettingsEmpty()
        {
            if(string.IsNullOrEmpty(GetConfig().BaseUrl) ||
                string.IsNullOrEmpty(GetConfig().Password) ||
                string.IsNullOrEmpty(GetConfig().Username))
            {
                return true;
            }
            return false;
        }

        public static ConfigurationManager GetInstance() 
        {
            if (_instance == null)
            {
                return _instance = new ConfigurationManager();
            }
            return _instance;
        }
    }
}
