﻿using System;
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
using System.Threading.Tasks;
using PowerFolder.Template;

namespace PowerFolder
{
    public partial class Plugin
    {
        private Outlook.Application OutlookApplication;
        private Outlook.Inspectors OutlookInspectors;
        private Outlook.Inspector OutlookInspector;
        private Outlook.MailItem OutlookMailItem;

        bool newMail = true;

        const string _classname = "[Plugin]";

        private void Plugin_Startup(object sender, System.EventArgs e)
        {
            Logging.PowerFolderLogProfile.initLogProfile();

            OutlookApplication = this.Application as Outlook.Application;
            OutlookInspectors = OutlookApplication.Inspectors;
            OutlookInspectors.NewInspector += new Microsoft.Office.Interop.Outlook.InspectorsEvents_NewInspectorEventHandler(OutlookInspectors_NewInspector);
            OutlookApplication.ItemSend += new Microsoft.Office.Interop.Outlook.ApplicationEvents_11_ItemSendEventHandler(OutlookApplication_ItemSend);

            Update.Updater updater = new Update.Updater();
            updater.CheckForUpdate();


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
                return;
            }

            if (!newMail)
            {
                newMail = true;
                return;
            }
            OutlookMailItem.SaveAs(Path.GetTempPath() + "\\template1.oft", OlSaveAsType.olTemplate);

            Thread thread = null;


            if (Config.GetInstance().GetConfig().FileLinkDialogEachEmail)
            {
                if (FileLinkDialog.GetInstance().ShowDialog() == DialogResult.OK)
                {
                    thread = new Thread(() => ItemSend_Thread(
                        FileLinkDialog.GetInstance().GetFileLinkParams(true)));
                    thread.Start();
                    Logger.LogThis(string.Format("{0} {1} [Staring ItemSend_Thread to generate the PowerFolder Email]",
                        _classname, _methodname), Logging.eloglevel.info);
                }
            }
            else if (FileLinkDialog.GetInstance().GetFileLinkParams().Count > 0)
            {
                thread = new Thread(() => ItemSend_Thread(
                    FileLinkDialog.GetInstance().GetFileLinkParams(true)));
                thread.Start();
                Logger.LogThis(string.Format("{0} {1} [Starting ItemSend_Thread to generate the PowerFolder Email]", _classname, _methodname),
                    Logging.eloglevel.info);
            }
            else if (Config.GetInstance().GetConfig().GetDefaultValues().Count > 0)
            {
                thread = new Thread(() => ItemSend_Thread(
                    Config.GetInstance().GetConfig().GetDefaultValues()));
                Logger.LogThis(string.Format("{0} {1} [Starting ItemSend_Thread to generate the PowerFolder Email]", _classname, _methodname),
                    Logging.eloglevel.info);
                thread.Start();
            }
            else
            {
                thread = new Thread(() => ItemSend_Thread(
                    new Dictionary<string, string>()));
                thread.Start();
                Logger.LogThis(string.Format("{0} {1} [Starting ItemSend_Thread to generate the PowerFolder Email]", _classname, _methodname),
                    Logging.eloglevel.info);
            }
                Cancel = true;
        }

        private void ItemSend_Thread(Dictionary<string, string> linkParams)
        {
            const string _methodname = "[ItemSend_Thread]";

            if (OutlookInspector.CurrentItem is Outlook.MailItem)
            {
                Logger.LogThis(string.Format("{0} {1} [Saving E-Mail as Template]",
                    _classname, _methodname),
                    Logging.eloglevel.verbose);

                MailItem mail = OutlookInspector.CurrentItem as Outlook.MailItem;
                mail.Close(OlInspectorClose.olDiscard);
            }

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
            if (responseGetInfo == null)
            {
                MessageBox.Show("An error occured while sending the Email. Please check your logs for more informations.", Properties.Resources.application_title);
                newEmail.Display();
                return;
            }

            if (responseGetInfo.Message == null)
            {
                MessageBox.Show("An error occured while sending the Email. Please check your logs for more informations.", Properties.Resources.application_title);
                newEmail.Display();
                return;
            }

            /*if (!responseGetInfo.IsJSONResponse())
            {
                Logger.LogThis(string.Format("{0} {1} [Exception : AccountsAPI #getInfo is not a JSON Response cant parse!]",
                    _classname, _methodname),
                    Logging.eloglevel.error);
                return;
            }*/
            JObject jsonResponse = JObject.Parse(responseGetInfo.Message);

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

            foreach (Attachment a in newEmail.Attachments)
            {
                attachmentsSize += a.Size;
            }
            try
            {
                if (attachmentsSize > (spaceAllowed - spaceUsed))
                {
                  if(MessageBox.Show(Properties.Resources.mail_quota_exceeded,
                        Properties.Resources.application_title, MessageBoxButtons.YesNo) == DialogResult.Yes)
                  {
                        newMail = false;
                        newEmail.Send();
                        return;
                  }
                  else
                  {
                        newEmail.Display();
                        return;
                  }
                }
            }
            catch (System.Exception e) 
            {
                Logger.LogThis(string.Format("{0} {1} [Error while checking QUOTA : {2}]", _classname, _methodname, e.Message), Logging.eloglevel.error);
                return;
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
                        + "could not parse.]", _classname, _methodname), Logging.eloglevel.error);
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

                if (currentFileLink.StartsWith("/"))
                {
                    currentFileLink = Config.GetInstance().GetConfig().BaseUrl + currentFileLink;
                }
                if (linkParams.ContainsKey(FileLinkContraints.PASSWORD))
                {
                    currentFileLink = currentFileLink.Replace("getlink", "dlpw");
                }
                else
                {
                    currentFileLink = currentFileLink.Replace("getlink", "dl");
                }
                fileLinks.Add(currentFileLink);
                newEmail.Attachments[1].Delete();
            }
            HtmlHandler htmlHandler = new HtmlHandler(fileLinks);

            if (htmlHandler.CreateTempFolder())
            {
                Logger.LogThis(string.Format("{0} {1} [Created temp folder for html files]"
                    , _classname, _methodname)
                    , Logging.eloglevel.info);

                List<string> htmlPaths = htmlHandler.CreateFiles();
                if (!htmlHandler._innerError)
                {
                    foreach (string s in htmlPaths)
                    {
                        newEmail.Attachments.Add(s, Outlook.OlAttachmentType.olByValue);
                    }
                }
                else
                {
                    Logger.LogThis(string.Format("{0} {1} [It was not possible to create the html files. Using file links instead]"
                        , _classname, _methodname)
                        , Logging.eloglevel.warn);

                    string linkHeader = Environment.NewLine + "----------*Attachments*----------";
                    newEmail.Body += linkHeader;
                    foreach (string s in fileLinks)
                    {
                        string current = s;

                        current = Environment.NewLine + s;
                        newEmail.Body += current;
                    }
                    newEmail.Body += linkHeader;
                }
            }
            else
            {
                Logger.LogThis(string.Format("{0} {1} [It was not possible to create the html files. Using file links instead]"
                    , _classname, _methodname)
                    , Logging.eloglevel.warn);

                string linkHeader = Environment.NewLine + "----------*Attachments*----------";
                newEmail.Body += linkHeader;
                foreach (string s in fileLinks)
                {
                    string current = s;

                    current = Environment.NewLine + s;
                    newEmail.Body += current;          
                }
                newEmail.Body += linkHeader;
            }
            
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
