using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerFolder.Mail
{
    public class OutboxDirectory
    {
        private string _name = "Outbox";
        private string _appPath = string.Format("{0}\\PowerFolder\\Outlook\\",
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

        public OutboxDirectory()
        {
            Create(); 
        }

        private void Create()
        {
            if (!Directory.Exists(Path.Combine(_appPath, _name)))
            {
                Logging.Log.LogThis("Outbox folder does not exist, creating new one on path : '" + _appPath + "' with name : 'Outbox'", Logging.eloglevel.info);
                try 
                {
                    Directory.CreateDirectory(Path.Combine(_appPath, _name));
                }
                catch (IOException e1) 
                {
                    Logging.Log.LogThisError(e1);
                    return;
                }
            }
       
        }

        public List<FileInfo> GetDrafts()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(_appPath, _name));

            List<FileInfo> mails = new List<FileInfo>();

            if (dirInfo.GetFiles().Count() < 0) 
            {
                Logging.Log.LogThis("No mails found in Outbox to be sent.", Logging.eloglevel.info);
                return mails;
            }
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                try 
                {
                    mails.Add(file);
                }
                catch (Exception e)
                {
                    Logging.Log.LogThis("Unable to load current 'MailItem' with name : '" + file.Name + "' on path :" + file.FullName, Logging.eloglevel.error);
                    Logging.Log.LogThisError(e);
                }
            }
            return mails;
        }

        public bool ClearBox()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(_appPath, _name));

            if (dirInfo.GetFiles().Count() < 0)
            {
                Logging.Log.LogThis("No mails found in Outbox to be sent.", Logging.eloglevel.info);
                return true;
            }
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                if (!file.Extension.Contains("oft"))
                {
                    continue;
                }
                try
                {
                    file.Delete();
                }
                catch (Exception e)
                {
                    Logging.Log.LogThisError(e);
                    return false;
                }
            }
            return true;
        }
        public string GetPath()
        {
            return Path.Combine(_appPath, _name).ToString();
        }

        public string GetDirectoryName()
        {
            return _name;
        }
    }
}
