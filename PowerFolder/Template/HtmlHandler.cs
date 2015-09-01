﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Logger = PowerFolder.Logging.Log;

namespace PowerFolder.Template
{
    /// <summary>
    /// 
    /// </summary>
    public class HtmlHandler
    {
        /// <summary>
        /// True if an error occures in this instance
        /// </summary>
        public bool _innerError { get; private set; }
        /// <summary>
        /// List of file links from the MailItem
        /// </summary>
        private List<string> _fileLinks;

        private string _classname = "[HtmlHandler]";

        private string TEMP_FOLDER = Path.Combine(Path.GetTempPath(), "HtmlFiles");
        private string TEMPLATE_FILE = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "AttachmentTemplate.html");

        public HtmlHandler(List<string> fileLinks)
        {
            this._fileLinks = fileLinks;
            _innerError = false;
            ClearTempFolder();
        }

        /// <summary>
        /// Create the Temporary Directory to work with Html Attachments
        /// </summary>
        /// <returns>True if the directory exists or it has been created successfuly otherwise false</returns>
        public bool CreateTempFolder()
        {
            const string _methodname = "[CreateTempFolder]";

            if (!Directory.Exists(TEMP_FOLDER))
            {
                try
                {
                    Directory.CreateDirectory(TEMP_FOLDER);
                    return true;
                }
                catch (IOException e)
                {
                    Logger.LogThis(string.Format("{0} {1} [Exception while creating the temp directory : {2}]", _classname, _methodname, e.Message), Logging.eloglevel.error);
                    return false;
                }
                
            }
            return true;
        }

        /// <summary>
        /// Deletes all files within the temporary folder
        /// </summary>
        /// <returns>True if all files have been cleared successfuly otherwise false</returns>
        public bool ClearTempFolder()
        {
            DirectoryInfo dirInfo = new DirectoryInfo(TEMP_FOLDER);
            FileInfo currentFile = null;
            const string _methodname = "[ClearTempFolder]";

            try
            {
                foreach (FileInfo fInfo in dirInfo.GetFiles())
                {
                    currentFile = fInfo;
                    fInfo.Delete();
                }
            }
            catch (IOException e) 
            {
                if (currentFile != null)
                {
                    Logger.LogThis(string.Format("{0} {1} [Exception while deleting file '{2}' : {3} ]", _classname, _methodname, currentFile.Name, e.Message), Logging.eloglevel.error);
                    return false;
                }
                else
                {
                    Logger.LogThis(string.Format("{0} {1} [Exception while clearing the temporary folder : {2}]", _classname, _methodname, e.Message), Logging.eloglevel.error);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Creates a HTML File for every FileLink added to the current instance
        /// </summary>
        /// <returns>a List containing the paths for the created files</returns>
        public List<string> CreateFiles()
        {
            const string _methodname = "[CreateFiles]";
            List<string> filePaths = new List<string>();
            try
            {
                string htmlContent = File.ReadAllText(TEMPLATE_FILE);

                if (string.IsNullOrEmpty(htmlContent))
                {
                    Logger.LogThis(string.Format("{0} {1} [It was not possible to read the html template file. using fileLinks instead.", _classname, _methodname), Logging.eloglevel.warn);
                    _innerError = true;
                    return null;
                }

                foreach (string s in _fileLinks)
                {
                    int lastIndexOfSlash = (s.LastIndexOf("/") + 1);

                    string fileName = s.Substring(lastIndexOfSlash, s.Length - lastIndexOfSlash);
                    string fileLink = s;

                    string newHtmlFileContent = htmlContent;

                    File.WriteAllText(Path.Combine(TEMP_FOLDER, fileName + ".html"), newHtmlFileContent
                        .Replace("$filename", fileName)
                        .Replace("$filelink", fileLink));

                    filePaths.Add(Path.Combine(TEMP_FOLDER, fileName + ".html"));
                }
                return filePaths;
            }
            catch (IOException e)
            {
                Logger.LogThis(string.Format("{0} {1} [Exception while reading the HtmlTemplate : {2}]", _classname, _methodname, e.Message), Logging.eloglevel.error);
                _innerError = true;
                return null;               
            }
        } 
    }
}
