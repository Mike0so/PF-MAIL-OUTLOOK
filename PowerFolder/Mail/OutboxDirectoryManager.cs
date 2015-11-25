using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PowerFolder.Mail
{
    public class OutboxDirectoryManager
    {
        private static OutboxDirectory _outbox { get; set; }
        private static OutboxDirectoryManager _instance { get; set; }
        private static Microsoft.Office.Interop.Outlook.Application _appInstance { get; set; }
        private static List<Microsoft.Office.Interop.Outlook.MailItem> _emails { get; set; }

        private OutboxDirectoryManager(Microsoft.Office.Interop.Outlook.Application app) 
        {
            _outbox = new OutboxDirectory();
            _appInstance = app;
        }

        public List<Microsoft.Office.Interop.Outlook.MailItem> SendEmails()
        {
            List<FileInfo> mailDrafts = _outbox.GetDrafts();
            List<MailItem> failedMails = new List<MailItem>();
            MailItem mail;
            if (mailDrafts.Count == 0) 
            {
                return null;
            }

            foreach (FileInfo info in mailDrafts)
            {
                if (!info.Extension.Contains("oft"))
                {
                    continue;
                }
                try
                {
                    mail = _appInstance.CreateItemFromTemplate(info.FullName);
                }
                catch (System.Exception e)
                {
                    Logging.Log.LogThis("Could not load template load mail from 'Outbox' with name '" + info.Name + "' on path : '" + info.FullName + "'", Logging.eloglevel.warn);
                    Logging.Log.LogThisError(e);
                    continue;
                }
                if (mail == null)
                {
                    Logging.Log.LogThis("It was not possible to load a draft from the Outbox directory. Name of file : " + info.Name + " on path : " + info.FullName, Logging.eloglevel.warn);
                    continue;
                }

                if (string.IsNullOrEmpty(mail.To) && string.IsNullOrEmpty(mail.BCC) && string.IsNullOrEmpty(mail.CC))
                {
                    Logging.Log.LogThis("The recipient of the current mail is empty. mail : '" + mail.ToString() + "'", Logging.eloglevel.warn);
                    failedMails.Add(mail);
                    continue;
                }
                try
                {
                    mail.Send();

                    info.Delete();
                }
                catch (IOException e1)
                {
                    Logging.Log.LogThisError(e1);
                }
                catch (System.Exception e)
                {
                    Logging.Log.LogThis("Could not sent email from Template", Logging.eloglevel.error);
                    Logging.Log.LogThisError(e);
                }
            }
            Logging.Log.LogThis("Successfuly sended all drafts that couldnt not be sent due to envoirment error", Logging.eloglevel.info);
            return failedMails;
        }

        public static OutboxDirectoryManager Get(Microsoft.Office.Interop.Outlook.Application app)
        {
            if (_instance == null)
                return _instance = new OutboxDirectoryManager(app);

            return _instance;
        }

        public string GetPath()
        {
            return _outbox.GetPath();
        }
    }
}
