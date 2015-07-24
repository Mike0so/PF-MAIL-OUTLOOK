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

        public Dictionary<string, string> GetDefaultValues()
        {
            Dictionary<string, string> listparams = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(FileLinkValidFor))
            {
                listparams.Add("valid", FileLinkValidFor);
            }
            if (!string.IsNullOrEmpty(FileLinkDownloadCount))
            {
                listparams.Add("downloads", FileLinkDownloadCount);
            }
            return listparams;
        }
    }
}
