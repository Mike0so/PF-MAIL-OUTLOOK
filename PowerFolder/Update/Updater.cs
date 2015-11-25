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
    /// <summary>
    /// Needs to be implemented currently, currently a hack
    /// </summary>
    public class Updater
    {
        const string _classname = "[Updater]";
        private bool calledByControl { get; set; }

        public Updater() { }

        public Updater(bool calledByControl)
        {
            this.calledByControl = calledByControl;
        }

        public async void CheckForUpdate(bool calledByControl = false)
        {
            const string _methodname = "[CheckForUpdate]";

            Logger.LogThis(string.Format("{0} {1} [Checking for updates...]",
                _classname, _methodname), Logging.eloglevel.info);
            try
            {
                Thread thread = null;

                HttpClient client = new HttpClient();

                string versionCurrentStr = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 5);
                string versionLatestStr = await client.GetStringAsync("http://checkversion.powerfolder.com/PowerFolderOutlook_LatestVersion.txt");

                if (string.IsNullOrEmpty(versionLatestStr))
                {
                    Logger.LogThis(string.Format("{0} {1} [The version from server was empty]",
                        _classname, _methodname), Logging.eloglevel.warn);
                    return;
                }
                string[] seperator = { "." };

                string[] splittedCurrent = versionCurrentStr.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
                string[] splittedLatest = versionLatestStr.Split(seperator, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 0; i < 3; i++)
                {
                    int versionPartCurrent = int.Parse(splittedCurrent[i]);
                    int versionPartLatest = int.Parse(splittedLatest[i]);

                    if (versionPartCurrent < versionPartLatest)
                    {
                        Logger.LogThis(string.Format("{0} {1} [A new version is available. Download-Link : https://my.powerfolder.com/dl/fi8Z33UQp9gf3L6pxJumVZ7c/PowerFolder_Outlook_Add-In_Beta.exe ]", _classname, _methodname), Logging.eloglevel.info);
                        thread = new Thread(() => ShowVersionDialog(true));
                        thread.Start();
                    }
                }
                Logger.LogThis(string.Format("{0} {1} [Add-In is up to date]", _classname, _methodname), Logging.eloglevel.info);
                thread = new Thread(() => ShowVersionDialog(false, calledByControl));
                thread.Start();
                return;
            }
            catch (Exception e)
            {
                Logger.LogThisError(e);
                return;
            }
        }

        public void ShowVersionDialog(bool updateAvailable = false, bool calledByControl = false)
        {
            if (updateAvailable)
            {
                if (MessageBox.Show(Properties.Resources.update_new_version, Properties.Resources.application_title, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    Process.Start("https://my.powerfolder.com/dl/fi8Z33UQp9gf3L6pxJumVZ7c/PowerFolder_Outlook_Add-In_Beta.exe");
                }
            }
            else
            {
                if (calledByControl)
                {
                    MessageBox.Show(Properties.Resources.update_up_to_date, Properties.Resources.application_title);
                }
            }
        }
    }
}
