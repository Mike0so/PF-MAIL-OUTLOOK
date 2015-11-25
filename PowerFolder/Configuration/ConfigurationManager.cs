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
        private static ConfigurationManager _instance;

        private const string _classname = "[ConfigurationManager]";
        private const string _configFile = "config.json";

        private ConfigurationManager() { }

        /// <summary>
        /// Returns the current instance of the in memory configuration
        /// </summary>
        /// <returns></returns>
        private Configuration GetConfig()
        {
            if (_config != null) {
                return _config;
            }

            CreateDefaultConfiguration();

            using (StreamReader reader = new StreamReader(GetConfigurationPath())) 
            {
                JsonSerializer serializer = new JsonSerializer();
                Configuration deSerializedConfig = JsonConvert.DeserializeObject<Configuration>(reader.ReadToEnd());
                _config = deSerializedConfig;
                reader.Close();
                return _config;
            }
        }

        /// <summary>
        /// Saves the in memory configuration to disk
        /// </summary>
        /// <param name="configuration"></param>
        public void SaveConfig()
        {
            if (_config == null)
            {
                _config = GetConfig();
            }
            try
            {
                File.WriteAllText(GetConfigurationPath(), JsonConvert.SerializeObject(_config));
                Log.LogThis("Configuration saved.", eloglevel.info);
            }
            catch (IOException ie)
            {
                Log.LogThisError(ie);
            }
            catch (Exception e)
            {
                Log.LogThisError(e);
            }
        }

        /// <summary>
        /// Creates the default config file
        /// </summary>
        private void CreateDefaultConfiguration()
        {
            Configuration config = null;
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
                        config.FileSizeConfiguration = "102400";
                    }
                    else
                    {
                        return;
                    }
                File.WriteAllText(GetConfigurationPath(), JsonConvert.SerializeObject(config));
                Log.LogThis("Successfuly installed the configuration.", eloglevel.info);
            }
            catch (IOException ie)
            {
                Log.LogThisError(ie);
                return;
            }
            catch (Exception e)
            {
                Log.LogThisError(e);
            }

        }

        /// <summary>
        /// Builds the configuration path
        /// </summary>
        /// <returns></returns>
        private string GetConfigurationPath()
        {
            string _path = string.Format("{0}\\PowerFolder\\Outlook\\",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

            if (!Directory.Exists(_path))
            {
                try
                {
                    Directory.CreateDirectory(_path);
                }
                catch (Exception e)
                {
                    Log.LogThisError(e);
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
                    Log.LogThisError(e);
                    return null;
                }
            }
            return _path;
        }

        /// <summary>
        /// True if settings are missing required data
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Changes the 'Username' value of the configuration
        /// </summary>
        /// <param name="username">The new username value</param>
        public void SetUsername (string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException("Username null");
            }
            GetConfig().Username = username;
        }

        /// <summary>
        /// The username value of the configuration
        /// </summary>
        /// <returns></returns>
        public string GetUsername()
        {
            string username = GetConfig().Username;

            if (string.IsNullOrEmpty(username))
            {
                return string.Empty;
            }
            return username;
        }

        /// <summary>
        /// Changes the 'Password' value of the configuration
        /// </summary>
        /// <param name="password">The new password value</param>
        public void SetPassword(string password)
        {
            if (password == null)
            {
                throw new ArgumentNullException("Password null");
            }
            string newPassword = Security.SecurityManager.Encrypt(password);
            GetConfig().Password = newPassword;
        }

        /// <summary>
        /// The password value of the configuration
        /// </summary>
        /// <returns></returns>
        public string GetPassword()
        {
            string password = Security.SecurityManager.Decrypt(GetConfig().Password);

            if (string.IsNullOrEmpty(password))
            {
                return string.Empty;
            }
            return password;
        }

        public void SetBaseURL(string prefix, string baseUrl)
        {
            if (prefix == null || baseUrl == null)
            {
                throw new ArgumentNullException("BaseURL null");
            }
            string url = string.Format("{0}{1}", prefix, baseUrl);
            GetConfig().BaseUrl = url;
        }

        /// <summary>
        /// The server url
        /// </summary>
        /// <returns></returns>
        public string GetBaseUrl()
        {
            string[] seperator = new string[] { "://" };
            string[] url = GetConfig().BaseUrl.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            string serverURL = url[1];
            return serverURL;
        }

        /// <summary>
        /// The server url prefix
        /// </summary>
        /// <returns></returns>
        public string GetBaseUrlPrefix()
        {
            string[] seperator = new string[] { "://" };
            string[] url = GetConfig().BaseUrl.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            string serverPrefix = url[0];
            return string.Format("{0}://", serverPrefix);
        }

        /// <summary>
        /// Set's the download count of the default file link configuration
        /// </summary>
        /// <param name="downloadCount"></param>
        public void SetDefaultDownloads(string downloadCount)
        {
            if (string.IsNullOrEmpty(downloadCount))
            {
                Log.LogThis("Not saving max. downloads because value was empty", eloglevel.verbose);
                return;
            }
            if (!PowerFolder.Utils.PFUtils.TryToParseInteger(downloadCount))
            {
                throw new FormatException("download count is not a valid number");
            }
            GetConfig().FileLinkDownloadCount = downloadCount;
        }

        /// <summary>
        /// Returns the download count of the default file link configuration
        /// </summary>
        /// <returns></returns>
        public int GetDefaultDownloads()
        {
            string downloadCountStr = GetConfig().FileLinkDownloadCount;

            if (string.IsNullOrEmpty(downloadCountStr))
            {
                return 0;
            }
            if (!PowerFolder.Utils.PFUtils.TryToParseInteger(downloadCountStr))
            {
                throw new FormatException("wrong value in configuration 'max. Downloads'");
            }
            int downloadCount = int.Parse(downloadCountStr);

            if (downloadCount < 0)
            {
                return 0;
            }
            return downloadCount;           
        }

        /// <summary>
        /// Set's the validity for the default file link configuration
        /// </summary>
        /// <param name="validityInDays"></param>
        public void SetDefaultValidity(string validityInDays)
        {
            string validity = validityInDays;

            if (validity == null)
            {
                throw new ArgumentNullException("validity is null");
            }

            if (string.IsNullOrEmpty(validity))
            {
                Log.LogThis("validity is not valid. not saving", eloglevel.verbose);
                return;
            }

            if (!PowerFolder.Utils.PFUtils.TryToParseInteger(validity))
            {
                throw new FormatException("validity is not a valid number");
            }

            GetConfig().FileLinkValidFor = validity;
        }

        /// <summary>
        /// Returns the default validity in days of the default file link configuration
        /// </summary>
        /// <returns></returns>
        public int GetDefaultValidity()
        {
            string validityStr = GetConfig().FileLinkValidFor;

            if (string.IsNullOrEmpty(validityStr))
            {
                return 0;
            }

            if (!PowerFolder.Utils.PFUtils.TryToParseInteger(validityStr))
            {
                throw new FormatException("unvalid value in config");
            }

            int validity = int.Parse(validityStr);

            if (validity < 0)
            {
                return 0;
            }
            return validity;
        }

        /// <summary>
        /// Defines if the plugin uses the default file link configuration
        /// </summary>
        /// <param name="enabled"></param>
        public void SetUseFileLinkDefaults(bool enabled)
        {
            GetConfig().UseDefaultFileLinkConfig = enabled;
        }

        /// <summary>
        /// Returns the boolean value to define if the plugin uses the default file link configuration
        /// </summary>
        /// <returns></returns>
        public bool GetUseFileLinkDefaults()
        {
            bool state = GetConfig().UseDefaultFileLinkConfig;
            return state;
        }

        /// <summary>
        /// Sets a new value to define if the plugin handles attachments or not
        /// </summary>
        /// <param name="enabled"></param>
        public void SetTrackEmails(bool enabled)
        {
            GetConfig().TrackEmails = enabled;
        }

        /// <summary>
        /// Returns the boolean value to define if the plugin uses the default file link configuration
        /// </summary>
        /// <returns></returns>
        public bool GetTrackEmails()
        {
            bool state = GetConfig().TrackEmails;
            return state;
        }

        /// <summary>
        /// Sets a new value to define if the file link dialog is shown on each Sent-Event
        /// </summary>
        /// <param name="enabled"></param>
        public void SetShowFileLinkDialog(bool enabled)
        {
            GetConfig().FileLinkDialogEachEmail = enabled;
        }

        /// <summary>
        /// Returns a boolean value to define if the file link dialog is shown on each Sent-Event
        /// </summary>
        /// <returns></returns>
        public bool GetShowFileLinkDialog()
        {
            bool state = GetConfig().FileLinkDialogEachEmail;
            return state;
        }

        /// <summary>
        /// Set's the minimum file size to define when the add-in edit's attachments
        /// </summary>
        /// <param name="fileSizeInBytes"></param>
        public void SetMinFileSize(string fileSizeInBytes)
        {
            string fileSizeStr = fileSizeInBytes;

            if (string.IsNullOrEmpty(fileSizeStr))
            {
                throw new ArgumentNullException("file size is null");
            }

            if (!PowerFolder.Utils.PFUtils.TryToParseLong(fileSizeStr))
            {
                fileSizeStr = "0";
            }

            GetConfig().FileSizeConfiguration = fileSizeStr;
        }

        /// <summary>
        /// Returns the minimum file size to define when the add-in edit's attachments
        /// </summary>
        /// <returns></returns>
        public long GetMinFileSize()
        {
            string fileSize = GetConfig().FileSizeConfiguration;

            if (!PowerFolder.Utils.PFUtils.TryToParseLong(fileSize))
            {
                return 0;
            }
            return long.Parse(fileSize);
        }

        /// <summary>
        /// Returns the default file link configuration
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetFileLinkDefaultValues()
        {
            Dictionary<string, string> listparams = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(GetConfig().FileLinkValidFor))
            {
                listparams.Add(FileLinkContraints.VALIDITY, GetConfig().FileLinkValidFor);
            }
            if (!string.IsNullOrEmpty(GetConfig().FileLinkDownloadCount))
            {
                listparams.Add(FileLinkContraints.MAX_DOWNLOAD, GetConfig().FileLinkDownloadCount);
            }
            return listparams;
        }

        /// <summary>
        /// Returns the Add-In ID
        /// </summary>
        /// <returns></returns>
        public string GetAddInID()
        {
            if (GetConfig().AddInID == null)
            {
                throw new ArgumentNullException("addinid is null critical error");
            }

            return GetConfig().AddInID;
        }
    }
}
