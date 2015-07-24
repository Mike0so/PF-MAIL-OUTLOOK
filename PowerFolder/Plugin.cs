using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading;

using Microsoft.Office.Interop.Outlook;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;

using Logger = PowerFolder.Logging.Log;
using Config = PowerFolder.Configuration.ConfigurationManager;
using PowerFolder.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace PowerFolder
{
    public partial class Plugin
    {
        public Outlook.Application OutlookApplication;
        public Outlook.Inspectors OutlookInspectors;
        public Outlook.Inspector OutlookInspector;
        public Outlook.MailItem OutlookMailItem;

        bool newMail = true;

        const string _classname = "[Plugin]";

        private void Plugin_Startup(object sender, System.EventArgs e)
        {
            Logging.PowerFolderLogProfile.initLogProfile();

            OutlookApplication = this.Application as Outlook.Application;
            OutlookInspectors = OutlookApplication.Inspectors;
            OutlookInspectors.NewInspector += new Microsoft.Office.Interop.Outlook.InspectorsEvents_NewInspectorEventHandler(OutlookInspectors_NewInspector);
            OutlookApplication.ItemSend += new Microsoft.Office.Interop.Outlook.ApplicationEvents_11_ItemSendEventHandler(OutlookApplication_ItemSend);
        }

        void OutlookInspectors_NewInspector(Microsoft.Office.Interop.Outlook.Inspector Inspector)
        {
            OutlookInspector = Inspector as Outlook.Inspector;

            if (Inspector.CurrentItem is Outlook.MailItem)
            {
                OutlookMailItem = Inspector.CurrentItem as Outlook.MailItem;
            }
        }

        void OutlookApplication_ItemSend(object Item, ref bool Cancel)
        {
            const string _methodname = "[OutlookApplication_ItemSend]";

            if (!Config.GetInstance().GetConfig().TrackEmails)
            {
                return;
            }

            if (Config.GetInstance().SettingsEmpty())
            {
                MessageBox.Show(Properties.Resources.config_empty,
                    Properties.Resources.application_title);

                PreferencesForm.GetInstance().Show();
                Cancel = true;
                return;
            }

            if (OutlookMailItem.Attachments.Count == 0)
            {
                Cancel = true;
                return;
            }

            if (!newMail)
            {
                newMail = true;
                return;
            }

            OutlookMailItem.SaveAs(Path.GetTempPath() + "\\template1.oft", OlSaveAsType.olTemplate);

            /* Use Default values if they are set */
            if (FileLinkDialog.GetInstance().linkParams.Count > 0)
            {
                Dictionary<string, string> cache = ConvertToCache(FileLinkDialog.GetInstance().linkParams);

                Thread thread = new Thread(() => ItemSend_Thread(
                    cache));
                Logger.LogThis(string.Format("{0} {1} [Starting ItemSend_Thread to generate the PowerFolder Email]", _classname, _methodname),
                    Logging.eloglevel.info);

                thread.Start();
                FileLinkDialog.GetInstance().linkParams = new Dictionary<string, string>();
            }
            else if (Config.GetInstance().GetConfig().UseDefaultFileLinkConfig &&
                    Config.GetInstance().GetConfig().GetDefaultValues().Count > 0)
            {
                    Thread thread = new Thread(() => ItemSend_Thread(
                        Config.GetInstance().GetConfig().GetDefaultValues()));

                    Logger.LogThis(string.Format("{0} {1} [Starting ItemSend_Thread to generate the PowerFolder Email]", _classname, _methodname),
                        Logging.eloglevel.info);

                    thread.Start();
            }
            else
            {
                Thread thread = new Thread(() => ItemSend_Thread(
                    new Dictionary<string, string>()));

                Logger.LogThis(string.Format("{0} {1} [Starting ItemSend_Thread to generate the PowerFolder Email]", _classname, _methodname),
                    Logging.eloglevel.info);

                thread.Start();
            }
            /*
            if (Config.GetInstance().GetConfig().FileLinkDialogEachEmail)
            {
                FileLinkDialog.GetInstance().ConfigurePasswordControls();
                if (FileLinkDialog.GetInstance().ShowDialog() == DialogResult.OK)
                {
                    OutlookMailItem.SaveAs(Path.GetTempPath() + "\\template1.oft", OlSaveAsType.olTemplate);

                    Thread thread = new Thread(() => ItemSend_Thread(
                        FileLinkDialog.GetInstance().linkParams));

                    bool passwordSet = string.IsNullOrEmpty(FileLinkDialog.GetInstance()._linkPassword);

                    Logger.LogThis(string.Format("{0} {1} [Starting ItemSend_Thread with following params : maxDownloads : {2}, validDays : {3}, password set : {4}]"
                        , _classname
                        , _methodname
                        , FileLinkDialog.GetInstance()._maxDownloads
                        , FileLinkDialog.GetInstance()._validTill
                        , passwordSet), Logging.eloglevel.verbose);
                    thread.Start();
                }
            }
            else
            {
                OutlookMailItem.SaveAs(Path.GetTempPath() + "\\template1.oft", OlSaveAsType.olTemplate);

                Thread thread = new Thread(() => ItemSend_Thread(
                    FileLinkDialog.GetInstance().linkParams));

                Logger.LogThis(string.Format("{0} {1} [Starting Item_Send Thread without Dialog.]",
                    _classname, _methodname), Logging.eloglevel.info);
                thread.Start();
            }*/
                    Cancel = true;
        }

        private void ItemSend_Thread(Dictionary<string, string> linkParams)
        {
            const string _methodname = "[ItemSend_Thread]";

            /* Hide Email Item from User */
            if (OutlookInspector.CurrentItem is Outlook.MailItem)
            {
                Logger.LogThis(string.Format("{0} {1} [Saving E-Mail hiding from user]",
                    _classname, _methodname),
                    Logging.eloglevel.verbose);

                MailItem mail = OutlookInspector.CurrentItem as Outlook.MailItem;
                mail.Close(OlInspectorClose.olDiscard);
            }

            /* Create a new OutlookItem from a saved template */
            Logger.LogThis(string.Format("{0} {1} [Creating new E-Mail from Template]",
                _classname, _methodname),
                Logging.eloglevel.verbose);

            MailItem newEmail = OutlookApplication.CreateItemFromTemplate(
                Path.GetTempPath() + "\\template1.oft") as Outlook.MailItem;

            PFApi api = new PFApi();
            PFResponse responseGetInfo = api.CollectAccountInfo();

            if (!responseGetInfo.IsValidResponse(newEmail))
            {
                return;
            }

            if (!responseGetInfo.IsJSONResponse())
            {
                Logger.LogThis(string.Format("{0} {1} [Exception : AccountsAPI #getInfo is not a JSON Response cant parse!]",
                    _classname, _methodname),
                    Logging.eloglevel.error);
                return;
            }

            JObject jsonResponse = JObject.Parse(responseGetInfo.Message);

            /* Creating the Attachment ID for an Account MA-<AccountID> */
            string accountID = jsonResponse.GetValue("ID").ToString();

            const string folderName = "$mail_attachments";
            string folderID = string.Format("MA-{0}", accountID);
            string folderIDBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(folderID));

            string directoryName = CollectDetailsForSubject(newEmail);

            long spaceUsed = long.Parse(jsonResponse.GetValue("spaceUsed").ToString());
            long spaceAllowed = long.Parse(jsonResponse.GetValue("spaceAllowed").ToString());
            long attachmentsSize = 0;

            if (accountID.Contains("/"))
            {
                Logger.LogThis(string.Format("{0} {1} *ATTENTION* - The AccountID contains a '/'",
                    _classname, _methodname),
                    Logging.eloglevel.warn);
            }


            /* Counting the total file size of the attachments collection */
            foreach (Attachment a in newEmail.Attachments)
            {
                attachmentsSize += a.Size;
            }

            if (attachmentsSize > (spaceAllowed - spaceUsed))
            {
                DialogResult dialogResult = MessageBox.Show(Properties.Resources.itemsend_not_enough_quota,
                    Properties.Resources.application_title);

                if (dialogResult == DialogResult.Yes)
                {
                    newEmail.Send();
                    return;
                }
                else
                {
                    newEmail.Display();
                    return;
                }
            }

            PFResponse responseFolderExists = api.FolderExists(folderID);

            if (responseFolderExists == null)
            {
                MessageBox.Show("An error occured while sending the E-Mail.\r\n" +
                    "Please Check your logs for more information.",
                    Properties.Resources.application_title);
                newEmail.Display();
                return;
            }

            if (!responseFolderExists.IsValidResponse(newEmail))
            {
                return;
            }

            /* Check if the Mail-Attachment Folder exists.
             * Otherwise create a new Mail-Attachment folder. */
            if (responseFolderExists.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                PFResponse responseCreateFolder = api.CreateFolder(folderID,
                    folderName);

                if (responseCreateFolder == null)
                {
                    MessageBox.Show("An error occured while sending the E-Mail.\r\n" +
                        "Please Check your logs for more information.",
                        Properties.Resources.application_title);
                    newEmail.Display();
                    return;
                }

                if (!responseCreateFolder.IsValidResponse(newEmail))
                {
                    return;
                }
            }

            PFResponse responseDirectoryExists = api.DirectoryExists(folderIDBase64, directoryName);

            if (responseDirectoryExists == null)
            {
                MessageBox.Show("An error occured while sending the E-Mail.\r\n" +
                    "Please Check your logs for more information.",
                    Properties.Resources.application_title);
                newEmail.Display();
                return;
            }

            if (!responseDirectoryExists.IsValidResponse(newEmail))
            {
                return;
            }

            if (responseDirectoryExists.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                PFResponse responseCreateDirectory = api.CreateDirectory(folderIDBase64, directoryName);

                if (responseCreateDirectory == null)
                {
                    MessageBox.Show("An error occured while sending the E-Mail.\r\n" +
                        "Please Check your logs for more information.",
                        Properties.Resources.application_title);
                    newEmail.Display();
                    return;
                }

                if (!responseCreateDirectory.IsValidResponse(newEmail))
                {
                    return;
                }
            }
            else
            {
                folderIDBase64 = CollectDetailsForSubject(newEmail);

                PFResponse responseCreateDirectory = api.CreateDirectory(folderIDBase64, directoryName);

                if (responseCreateDirectory == null)
                {
                    MessageBox.Show("An error occured while sending the E-Mail.\r\n" +
                        "Please Check your logs for more information.",
                        Properties.Resources.application_title);
                    newEmail.Display();
                    return;
                }

                if (!responseCreateDirectory.IsValidResponse(newEmail))
                {
                    return;
                }
            }

            PFResponse uploadFilesResponse = api.UploadFiles(folderIDBase64, directoryName, newEmail.Attachments);

            if (uploadFilesResponse == null)
            {
                MessageBox.Show("An error occured while sending the E-Mail.\r\n" +
                    "Please Check your logs for more information.",
                    Properties.Resources.application_title);
                newEmail.Display();
                return;
            }

            if (!uploadFilesResponse.IsValidResponse(newEmail))
            {
                return;
            }

            List<string> fileLinks = new List<string>();

            while (newEmail.Attachments.Count != 0)
            {
                PFResponse responseStoreFileLink = api.CreateFileLink(
                    folderIDBase64, directoryName, newEmail.Attachments[1], linkParams);

                if (responseStoreFileLink == null)
                {
                    MessageBox.Show("An error occured while sending the E-Mail.\r\n" +
                        "Please Check your logs for more information.",
                        Properties.Resources.application_title);
                    newEmail.Display();
                    return;
                }

                if (!responseStoreFileLink.IsValidResponse(newEmail))
                {
                    return;
                }

                PFResponse responseRecieveFileLink = api.RecieveFileLink(
                    folderIDBase64, directoryName, newEmail.Attachments[1]);

                if (responseRecieveFileLink == null)
                {
                    MessageBox.Show("An error occured while sending the E-Mail.\r\n" +
                        "Please Check your logs for more information.",
                        Properties.Resources.application_title);
                    newEmail.Display();
                    return;
                }

                if (!responseRecieveFileLink.IsValidResponse(newEmail))
                {
                    return;
                }

                if (!responseRecieveFileLink.IsJSONResponse())
                {
                    Logger.LogThis(string.Format("{0} {1} [The File-Link response was not a json response,"
                        + "could not parse.]"), Logging.eloglevel.error);
                }

                string currentFileLink = JObject.Parse(responseRecieveFileLink.Message).GetValue("url").ToString();

                if (string.IsNullOrEmpty(currentFileLink))
                {
                    Logger.LogThis(string.Format("{0} {1} [Error : 'Empty FileLink']",
                        _classname, _methodname),
                        Logging.eloglevel.error);

                    MessageBox.Show("An error occured while sending the E-Mail.\r\n" +
                        "Please Check your logs for more information.",
                        Properties.Resources.application_title);
                    newEmail.Display();
                    return;
                }

                /* Sometimes it can happen that you dont get the full fileLink 
                   so if the baseURL is missing. We will add it by ourselfes. */
                if (currentFileLink.StartsWith("/"))
                {
                    currentFileLink = Config.GetInstance().GetConfig().BaseUrl + currentFileLink;
                }

                /* Replace the getlink with dl (direct downloadlink) 
                   or if the link is password protected replace it with a
                   'dlpw' (direct downloadlink)                          */
                /*if (Config.GetInstance().GetConfig().FileLinkProtection)
                {
                    currentFileLink.Replace("getlink", "dlpw");
                }
                else
                {
                    currentFileLink.Replace("getlink", "dl");
                } Needs to be fixed
                 */

                fileLinks.Add(currentFileLink);
                newEmail.Attachments[1].Delete();
            }
            string linkHeader = Environment.NewLine + "----------*Attachments*----------";
            newEmail.Body += linkHeader;

            foreach (string s in fileLinks)
            {
                string current = s;

                current = Environment.NewLine + s;
                newEmail.Body += current;          
            }
            newEmail.Body += linkHeader;
            newEmail.Save();

            newMail = false;

            Logger.LogThis(string.Format("{0} {1} [Successfuly edited E-Mail. Sending now...]",
                _classname, _methodname), Logging.eloglevel.info);

            newEmail.Send();
        }

        private string CollectDetailsForSubject(Outlook.MailItem item)
        {
            string subject = "";

            subject += DateTime.Now.ToShortDateString() + " ";

            if (!string.IsNullOrEmpty(item.To))
            {
                subject += item.To + " ";
            }

            if (!string.IsNullOrEmpty(item.Subject))
            {
                subject += item.Subject;
            }
            subject += " " + Guid.NewGuid().ToString().Replace("-", "");
            return subject;
        }

        private Dictionary<string, string> ConvertToCache(Dictionary<string, string> values)
        {
            Dictionary<string, string> cache = new Dictionary<string, string>();

            if (values.Keys.Contains("password"))
            {
                cache.Add("password", values["password"]);
            }
            if (values.Keys.Contains("valid"))
            {
                cache.Add("valid", values["valid"]);
            }
            if (values.Keys.Contains("downloads"))
            {
                cache.Add("downloads", values["downloads"]);
            }
            return cache;
        }
        private void Plugin_Shutdown(object sender, System.EventArgs e)
        {
            
        }

        #region Von VSTO generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(Plugin_Startup);
            this.Shutdown += new System.EventHandler(Plugin_Shutdown);
        }
        
        #endregion
    }
}
