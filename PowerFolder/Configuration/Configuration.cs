using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerFolder.Configuration
{
    public class Configuration
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string BaseUrl { get; set; }
        public string AddInID { get; set; }
        public bool UseDefaultFileLinkConfig { get; set; }
        public bool TrackEmails { get; set; }
        public bool FileLinkDialogEachEmail { get; set; }
        public string FileLinkValidFor { get; set; }
        public string FileLinkDownloadCount { get; set; }
        public string FileSizeConfiguration { get; set; }
    }

    public static class ConfigurationContrains
    {
        public const string KEY_USERNAME = "Username";
        public const string KEY_PASSWORD = "Password";
        public const string KEY_BASEURL = "BaseUrl";
        public const string KEY_ADDINID = "AddInID";
        public const string KEY_USEFILELINKDEFAULTS = "UseDefaultFileLinkConfig";
        public const string KEY_FILELINKVALIDITY = "FileLinkValidFor";
        public const string KEY_FILELINKDOWNLOADCOUNT = "FileLinkDownloadCount";
        public const string KEY_FILESIZE = "FileSizeConfiguration";
        public const string KEY_TRACKEMAILS = "TrackEmails";
        public const string KEY_FILELINKDIALOGONSEND = "FileLinkDialogEachEmail";
    }
}
