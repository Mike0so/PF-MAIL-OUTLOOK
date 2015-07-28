using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Logger = PowerFolder.Logging.Log;

namespace PowerFolder.Update
{
    public class Updater
    {
        const string _classname = "[Updater]";
        private bool calledByControl { get; set; }

        public Updater() { }

        public Updater(bool calledByControl)
        {
            this.calledByControl = calledByControl;
        }

        /// <summary>
        /// Recieves the latest version of the PowerFolder Outlook Add-In
        /// </summary>
        /// <param name="calledByControl">...</param>
        /// <returns>0 if the version is up to date,
        ///         1 if the version is outdated,
        ///         2 if an error orccures while validating the version</returns>
        public async Task<int> CheckVersionAsync(bool calledByControl)
        {
            const string _methodname = "[CheckVersionAsync]";

            Logger.LogThis(string.Format("{0} {1} [Checking for updates...]",
                _classname, _methodname), Logging.eloglevel.info);

            try
            {
                HttpClient client = new HttpClient();

                string currentVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 5);
                string latestVersion = await client.GetStringAsync("http://checkversion.powerfolder.com/PowerFolderOutlook_LatestVersion.txt");

                if (string.IsNullOrEmpty(latestVersion))
                {
                    Logger.LogThis(string.Format("{0} {1} [The version from server was empty]",
                        _classname, _methodname), Logging.eloglevel.warn);
                    return 2;
                }
                /* *TODO* PFC-2740
                string[] seperator = { "." };

                string[] splitedLocal = currentVersion.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                string[] splitedLatest = latestVersion.Split(seperator, StringSplitOptions.RemoveEmptyEntries);

                int localversion = int.Parse(splitedLocal[0]);
                int latestversion = int.Parse(splitedLatest[0]);
                */

                if (currentVersion.Equals(latestVersion))
                {
                    Logger.LogThis(string.Format("{0} {1} [Version : {2} is up to date]",
                        _classname, _methodname, latestVersion), Logging.eloglevel.info);
                    return 0;
                }
                else
                {
                    Logger.LogThis(string.Format("{0} {1} [A new version is available. Download-Link : https://my.powerfolder.com/dl/fi8Z33UQp9gf3L6pxJumVZ7c/PowerFolder_Outlook_Add-In_Beta.exe ]", _classname, _methodname), Logging.eloglevel.info);
                    Thread thread = new Thread(() => ShowVersionDialog());
                    thread.Start();
                    return 1;
                }
            }
            catch (Exception e)
            {
                Logger.LogThis(string.Format("{0} {1} [Error while trying to check the version. Exception : {2}]", _classname, _methodname, e.Message), Logging.eloglevel.error);
                return 2;
            }
        }

        private void ShowVersionDialog()
        {
            if (MessageBox.Show(Properties.Resources.update_new_version, Properties.Resources.application_title, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Process.Start("https://my.powerfolder.com/dl/fi8Z33UQp9gf3L6pxJumVZ7c/PowerFolder_Outlook_Add-In_Beta.exe");
            }
        }
    }
}
