using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using PowerFolder.Logging;

using ConfigManager = PowerFolder.Configuration.ConfigurationManager;
using System.Net;
using System.IO;

namespace PowerFolder.Http
{
    public class PFApi
    {
        private string _credentials;
        private int _timeout;

        private const string _classname = "[PFApi]";

        public PFApi()
        {
            _timeout = 300000;
            _credentials = "Basic " + Convert.ToBase64String(
                Encoding.UTF8.GetBytes(
                string.Format("{0}:{1}",
                ConfigManager.GetInstance().GetConfig().Username,
                Security.SecurityManager.Decrypt(ConfigManager.GetInstance().GetConfig().Password))));
        }

        /// <summary>
        /// Sends an BasicAuth request to the selected PowerFolder server.
        /// 
        /// </summary>
        /// <returns>The state of the authentication result</returns>
        public bool CanAuthenticate()
        {
            const string _methodname = "[CanAuthenticate]";

            Log.LogThis(string.Format("Authenticating User : {0}",
                ConfigManager.GetInstance().GetConfig().Username), eloglevel.info);

            try
            {
                HttpWebRequest request = WebRequest.Create(
                    string.Format("{0}/api/accounts?action=getInfo", 
                    ConfigManager.GetInstance().GetConfig().BaseUrl)) as HttpWebRequest;

                request.Timeout = _timeout;
                request.Method = "GET";
                request.Headers.Add("Authorization", _credentials);

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse) 
                {
                    Log.LogThis(string.Format("Successfuly authenticated User : {0}",
                        ConfigManager.GetInstance().GetConfig().Username), eloglevel.info);

                    return true;
                }
            }
            catch (WebException we)
            {
                Log.LogThis(string.Format("{0} {1} [Error while trying to authenticate User : {2} Exception : {3}]",
                    _classname, _methodname,
                    ConfigManager.GetInstance().GetConfig().Username,
                    we.Message), eloglevel.error);

                return false;
            }
            catch (Exception e) 
            {
                Log.LogThis(string.Format("{0} {1} [Error while trying to authenticate User : {2} Exception : {3}]",
                    _classname, _methodname,
                    ConfigManager.GetInstance().GetConfig().Username,
                    e.Message), eloglevel.error);
                
                return false;
            }
        }

        public PFResponse CollectAccountInfo()
        {
            const string _methodname = "[CollectAccountInfo]";

            PFResponse PFresponse = new PFResponse();

            Log.LogThis(string.Format("{0} {1} [Collecting account info for user : {2} url : {3}]",
                _classname,
                _methodname,
                ConfigManager.GetInstance().GetConfig().Username,
                string.Format("{0}/api/accounts?action=getInfo",ConfigManager.GetInstance().GetConfig().BaseUrl))
                , eloglevel.info);

            try
            {
                HttpWebRequest request = WebRequest.Create(
                    string.Format("{0}/api/accounts?action=getInfo",
                        ConfigManager.GetInstance().GetConfig().BaseUrl)) as HttpWebRequest;

                request.Timeout = _timeout;
                request.Method = "GET";
                request.Headers.Add("Authorization", _credentials);

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            if (reader != null)
                            {
                                string result = reader.ReadToEnd();

                                PFresponse.Message = result;
                                PFresponse.StatusCode = response.StatusCode;
                                Log.LogThis(string.Format("{0} {1} [Successfuly recieved server response]", _classname, _methodname), eloglevel.info);
                                return PFresponse;
                            }
                        }
                    }
                }
            }
            catch (WebException we)  
            {
                using (HttpWebResponse response = we.Response as HttpWebResponse)
                {
                    if (response != null)
                    {
                        PFresponse.ExceptionStatus = we.Status;
                        if (response.StatusCode != null)
                        {
                            PFresponse.StatusCode = response.StatusCode;
                        }
                    }
                    Log.LogThis(string.Format("{0} {1} [Exception : {2}]", _classname, _methodname, we.Message), eloglevel.error);
                    return PFresponse;
                }
         
            }
            catch (Exception e)
            {
                Log.LogThis(string.Format("{0} {1} [Exception : {2}]", _classname, _methodname, e.Message), eloglevel.error);
                return null;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderID">The PowerFolder folderID.</param>
        /// <returns>The wrapped HTTP response</returns>
        public PFResponse FolderExists(string folderID)
        {
            const string _methodname = "[FolderExists]";

            PFResponse PFresponse = new PFResponse();

            Log.LogThis(string.Format("{0} {1} [Checking for an existing folder with the ID : {2} ]",
                _classname,
                _methodname,
                folderID), eloglevel.info);

            try
            {
                HttpWebRequest request = WebRequest.Create(string.Format("{0}/api/folders/{1}?action=getInfo"
                    , ConfigManager.GetInstance().GetConfig().BaseUrl
                    , folderID)) as HttpWebRequest;

                request.Timeout = _timeout;
                request.Method = "GET";
                request.Headers.Add("Authorization", _credentials);

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            if (reader != null)
                            {
                                string result = reader.ReadToEnd();

                                PFresponse.Message = result;
                                PFresponse.StatusCode = response.StatusCode;
                                return PFresponse;
                            }
                        }
                    }
                }
            }
            catch (WebException we)
            {
                using (HttpWebResponse response = we.Response as HttpWebResponse)
                {
                    if (response != null)
                    {
                        PFresponse.ExceptionStatus = we.Status;
                        if (response.StatusCode != null)
                        {
                        }
                            PFresponse.StatusCode = response.StatusCode;
                    }
                    Log.LogThis(string.Format("{0} {1} [Exception : {2}]", _classname, _methodname, we.Message), eloglevel.error);
                    return PFresponse;
                }
            }
            catch (Exception e)
            {
                Log.LogThis(string.Format("{0} {1} [Exception : {2}]", _classname, _methodname, e.Message), eloglevel.error);
                return null;
            }
            return null;
        }

        public PFResponse CreateFolder(string folderID, string name)
        {
            const string _methodname = "[CreateFolder]";

            PFResponse PFresponse = new PFResponse();

            Log.LogThis(string.Format("{0} {1} [Creating 'MailAttachments' folder with ID : {2}, Name : {3}"
                , _classname
                , _methodname
                , folderID
                , name), eloglevel.info);

            try
            {
                HttpWebRequest request = WebRequest.Create(string.Format("{0}/api/folders?action=create&ID={1}&name={2}",
                    ConfigManager.GetInstance().GetConfig().BaseUrl,
                    folderID,
                    name)) as HttpWebRequest;

                request.Timeout = _timeout;
                request.Method = "GET";
                request.Headers.Add("Authorization", _credentials);

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            if (reader != null)
                            {
                                string result = reader.ReadToEnd();

                                PFresponse.Message = result;
                                PFresponse.StatusCode = response.StatusCode;

                                Log.LogThis(string.Format("{0} {1} [Successfuly created folder with ID : {2} Name : {3}",
                                    _classname,
                                    _methodname,
                                    folderID,
                                    name),
                                    eloglevel.info);

                                return PFresponse;
                            }
                        }
                    }
                }
            }
            catch (WebException we)
            {
                using (HttpWebResponse response = we.Response as HttpWebResponse)
                {
                    if (response != null)
                    {
                        PFresponse.ExceptionStatus = we.Status;
                        if (response.StatusCode != null)
                        {
                            PFresponse.StatusCode = response.StatusCode;
                        }
                    }
                    Log.LogThis(string.Format("{0} {1} [Exception : {2}]", _classname, _methodname, we.Message), eloglevel.error);

                    return PFresponse;
                }
            }
            catch (Exception e)
            {
                Log.LogThis(string.Format("{0} {1} [Exception : {2}]", _classname, _methodname, e.Message), eloglevel.error);
                return null;
            }
            return null;
        }

        /// <summary>
        /// Checks for an existing directory on a PowerFolder Server.
        /// 
        /// </summary>
        /// <param name="folderID">The parent folderID base64 encoded</param>
        /// <param name="directoryName">The name of the directory within the parent folder</param>
        /// <returns>The wrapped response</returns>
        public PFResponse DirectoryExists(string folderID, string directoryName)
        {
            const string _methodname = "[DirectoryExists]";

            PFResponse PFresponse = new PFResponse();

            Log.LogThis(string.Format("{0} {1} [Checking for an existing directory '{2}' within the folder : {3} ]",
             _classname,
            _methodname,
            directoryName,
            folderID), eloglevel.info);

            try
            {
                HttpWebRequest request = WebRequest.Create(string.Format("{0}/filesapi/{1}/{2}?action=exists",
                                    ConfigManager.GetInstance().GetConfig().BaseUrl,
                                    folderID,
                                    directoryName)) as HttpWebRequest;
                request.Timeout = _timeout;
                request.Method = "GET";
                request.Headers.Add("Authorization", _credentials);

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            if (reader != null)
                            {
                                string result = reader.ReadToEnd();

                                PFresponse.StatusCode = response.StatusCode;
                                PFresponse.Message = result;

                                Log.LogThis(string.Format("{0} {1} [The directory '{2}' within the folder {3} already exists.]",
                                    _classname,
                                    _methodname,
                                    directoryName,
                                    folderID), eloglevel.info);

                                return PFresponse;
                            }
                        }
                    }
                }
            }
            catch (WebException we)
            {
                using (HttpWebResponse response = we.Response as HttpWebResponse)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        if (response != null)
                        {
                            PFresponse.ExceptionStatus = we.Status;
                            if (response.StatusCode != null)
                            {
                                PFresponse.StatusCode = response.StatusCode;
                            }
                        }
                        Log.LogThis(string.Format("{0} {1} [Exception : {2}]", _classname, _methodname, we.Message), eloglevel.error);
                        return PFresponse;
                    }
                    PFresponse.StatusCode = response.StatusCode;
                    PFresponse.ExceptionStatus = we.Status;

                    Log.LogThis(string.Format("{0} {1} [Exception : {2}]", _classname, _methodname, we.Message), eloglevel.error);

                    return PFresponse;
                }
            }
            catch (Exception e)
            {
                Log.LogThis(string.Format("{0} {1} [Exception : {2}]", _classname, _methodname, e.Message), eloglevel.error);
                return null;
            }
            return null;
        }

        /// <summary>
        /// Creates a directory within a folder.
        /// 
        /// </summary>
        /// <param name="folderID">The parent folderID base64 encoded</param>
        /// <param name="directoryName">The name of the new directory</param>
        /// <returns>The wrapped response</returns>
        public PFResponse CreateDirectory(string folderID, string directoryName)
        {
            const string _methodname = "[CreateDirectory]";

            PFResponse PFresponse = new PFResponse();

            Log.LogThis(string.Format("{0} {1} [Creating a directory named '{2}' within the folder with ID {3} ]",
            _classname,
            _methodname,
            directoryName,
            folderID), eloglevel.info);

            try
            {
                HttpWebRequest request = WebRequest.Create(string.Format("{0}/filesapi/{1}?action=createsubdir&type=dir&dirName={2}",
                                    ConfigManager.GetInstance().GetConfig().BaseUrl,
                                    folderID,
                                    directoryName)) as HttpWebRequest;
                request.Timeout = _timeout;
                request.Method = "GET";
                request.Headers.Add("Authorization", _credentials);

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            if (reader != null)
                            {
                                string result = reader.ReadToEnd();

                                PFresponse.Message = result;
                                PFresponse.StatusCode = response.StatusCode;

                                Log.LogThis(string.Format("{0} {1} [Successfuly created directory '{2}' within the folder {3}]",
                                _classname,
                                _methodname,
                                directoryName,
                                folderID), eloglevel.info);

                                return PFresponse;
                            }
                        } 
                    }
                }
            }
            catch (WebException we)
            {
                using (HttpWebResponse response = we.Response as HttpWebResponse)
                {
                    if (response != null)
                    {
                        PFresponse.ExceptionStatus = we.Status;
                        if (response.StatusCode != null)
                        {
                            PFresponse.StatusCode = response.StatusCode;
                        }
                    }
                    PFresponse.ExceptionStatus = we.Status;
                    Log.LogThis(string.Format("{0} {1} [Exception : {2}]", _classname, _methodname, we.Message), eloglevel.error);

                    return PFresponse;
                }
            }
            catch (Exception e)
            {
                Log.LogThis(string.Format("{0} {1} [Exception : {2}]", _classname, _methodname, e.Message), eloglevel.error);
                return null;
            }
            return null;
        }

        public PFResponse UploadFiles(string folderID, string directoryName, Microsoft.Office.Interop.Outlook.Attachments attachments)
        {
            const string _methodname = "[UploadFiles]";
            string workingDirectory = Guid.NewGuid().ToString().Replace("-", "");

            PFResponse PFresponse = new PFResponse();

            Log.LogThis(string.Format("{0} {1} [Starting to edit and upload files to the server to the directory '{2}' within the folder '{3}']",
            _classname,
            _methodname,
            directoryName,
            folderID), eloglevel.info);

            try
            {
                if (!Directory.Exists(Path.Combine(Path.GetTempPath(), workingDirectory))) 
                {
                    Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), workingDirectory));
                    Log.LogThis(string.Format("{0} {1} [Creating temporary directory]",
                        _classname,
                        _methodname), eloglevel.info);
                }

                foreach (Microsoft.Office.Interop.Outlook.Attachment attachment in attachments)
                {
                    attachment.SaveAsFile(Path.Combine(Path.GetTempPath(),
                        workingDirectory, attachment.FileName));

                    using (FileStream fileStream = new FileStream(Path.Combine(
                        Path.GetTempPath(), workingDirectory, attachment.FileName),
                        FileMode.Open, FileAccess.Read))
                    {
                        byte[] binarys = PowerFolder.Utils.PFUtils.ReadFileChunked(fileStream, 2097152);

                        Dictionary<string, object> postParams = new Dictionary<string, object>();
                        postParams.Add("path", directoryName);
                        postParams.Add("file", new FormUpload.FileParameter(binarys, attachment.FileName, "application/octet-stream"));

                        HttpWebResponse response = FormUpload.MultipartFormDataPost(string.Format("{0}/upload/{1}",
                            ConfigManager.GetInstance().GetConfig().BaseUrl,
                            folderID), postParams);

                        PFresponse.StatusCode = response.StatusCode;
                        PFresponse.Message = response.StatusDescription;

                        Log.LogThis(string.Format("{0} {1} [Uploading file '{2}' to the server]",
                            _classname,
                            _methodname,
                            attachment.FileName), eloglevel.info);

                    }
                    File.Delete(Path.Combine(Path.GetTempPath(), workingDirectory, attachment.FileName));
                }
                Directory.Delete(Path.Combine(Path.GetTempPath(), workingDirectory));
                return PFresponse;
            }
            catch (WebException we)
            {
                using (HttpWebResponse response = we.Response as HttpWebResponse)
                {
                    if (response != null)
                    {
                        PFresponse.ExceptionStatus = we.Status;
                        if (response.StatusCode != null)
                        {
                            PFresponse.StatusCode = response.StatusCode;
                        }
                    }
                    PFresponse.ExceptionStatus = we.Status;
                    Log.LogThis(string.Format("{0} {1} [Exception : {2}]", _classname, _methodname, we.Message), eloglevel.error);

                    return PFresponse;
                }
            }
            catch (Exception e)
            {
                Log.LogThis(string.Format("{0} {1} [Exception : {2}]", _classname, _methodname, e.Message), eloglevel.error);
                return null;
            }
        }

        public PFResponse CreateFileLink(string folderID, string directoryName,
            Microsoft.Office.Interop.Outlook.Attachment attachment, Dictionary<string, string> linkParams)
        {
            const string _methodname = "[CreateFileLink]";

            PFResponse PFresponse = new PFResponse();

            Log.LogThis(string.Format("{0} {1} [Creating a File-Link for File '{2}']",
            _classname,
            _methodname,
            attachment.FileName), eloglevel.info);

            try
            {
                Log.LogThis(string.Format("{0} {1} [Checking for given Parameters]",
                    _classname, _methodname)
                    , eloglevel.verbose);

                /* Build the url dynamic depending on the params the user selected to configure */
                string url = string.Format("{0}/getlink/{1}/{2}/{3}?action=store",
                    ConfigManager.GetInstance().GetConfig().BaseUrl,
                    folderID,
                    directoryName,
                    attachment.DisplayName);

                Log.LogThis(string.Format("{0} {1} [Current URL : {2}]",
                    _classname,
                    _methodname,
                    url), eloglevel.verbose);

                StringBuilder sb = new StringBuilder();
                sb.Append(url);


                /* Read the params out of the collection */
                if (linkParams.Count > 0)
                {
                    if(linkParams.Keys.Contains("password")) {
                        string linkPassword = linkParams["password"];
                        sb.Append("&password=" + linkPassword);
                    }
                    if (linkParams.Keys.Contains("valid"))
                    {
                        string validity = linkParams["valid"];
                        int validInDays = int.Parse(validity);

                        DateTime validDateTime = DateTime.Now.AddDays(validInDays);
                        sb.Append("&linkValidTill=" + validDateTime.ToString("dd MMM yyyy"));
                    }
                    if (linkParams.Keys.Contains("downloads"))
                    {
                        string downloadCount = linkParams["downloads"];
                        sb.Append("&maxDownloads=" + downloadCount);
                    }
                }
                sb.Append("&json=1");

                Log.LogThis(string.Format("{0} {1} [Finished File-Link request url]",
                _classname,
                _methodname), eloglevel.verbose);
                
                HttpWebRequest request = WebRequest.Create(sb.ToString()) as HttpWebRequest;
                request.Timeout = _timeout;
                request.Method = "GET";
                request.Headers.Add("Authorization", _credentials);

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            if (reader != null)
                            {
                                string result = reader.ReadToEnd();

                                PFresponse.StatusCode = response.StatusCode;
                                PFresponse.Message = result;

                                Log.LogThis(string.Format("{0} {1} [{2}]",
                                    _classname,
                                    _methodname,
                                    result), eloglevel.info);

                                return PFresponse;
                            }
                        }
                    }
                }
            }
            catch (WebException we)
            {
                using (HttpWebResponse response = we.Response as HttpWebResponse)
                {
                    if (response != null)
                    {
                        if (response.StatusCode != null)
                        {
                            PFresponse.StatusCode = response.StatusCode;
                        }
                        PFresponse.ExceptionStatus = we.Status;
                        Log.LogThis(string.Format("{0} {1} [Exception : {2}]", _classname, _methodname, we.Message), eloglevel.error);

                        return PFresponse;
                    }
                }
            }
            catch (Exception e)
            {
                Log.LogThis(string.Format("{0} {1} [Exception : {2}]", _classname, _methodname, e.Message), eloglevel.error);
                return null;
            }
            return null;
        }

        public PFResponse RecieveFileLink(string folderID, string directoryName, Microsoft.Office.Interop.Outlook.Attachment attachment)
        {
            const string _methodname = "[RecieveFileLink]";

            PFResponse PFresponse = new PFResponse();

            Log.LogThis(string.Format("{0} {1} [Requesting a File-Link for File '{2}']",
            _classname,
            _methodname,
            attachment.FileName), eloglevel.info);

            try
            {
                HttpWebRequest request = WebRequest.Create(string.Format("{0}/getlink/{1}/{2}/{3}?json=1",
                    ConfigManager.GetInstance().GetConfig().BaseUrl,
                    folderID,
                    directoryName,
                    attachment.DisplayName)) as HttpWebRequest;
                request.Timeout = _timeout;
                request.Method = "GET";
                request.Headers.Add("Authorization", _credentials);

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            if (reader != null)
                            {
                                string result = reader.ReadToEnd();

                                PFresponse.StatusCode = response.StatusCode;
                                PFresponse.Message = result;

                                Log.LogThis(string.Format("{0} {1} [Recieved File-Link '{2}']",
                                    _classname,
                                    _methodname,
                                    result),
                                    eloglevel.info);
                                return PFresponse;
                            }
                        }
                    }
                }
            }
            catch (WebException we)
            {
                using (HttpWebResponse response = we.Response as HttpWebResponse)
                {
                    if (response != null)
                    {
                        PFresponse.ExceptionStatus = we.Status;
                        if (response.StatusCode != null)
                        {
                            PFresponse.StatusCode = response.StatusCode;
                        }
                    }
                    Log.LogThis(string.Format("{0} {1} [Exception : {2}]", _classname, _methodname, we.Message), eloglevel.error);

                    return PFresponse;
                }
            }
            catch (Exception e)
            {
                Log.LogThis(string.Format("{0} {1} [Exception : {2}]", _classname, _methodname, e.Message), eloglevel.error);
                return null;
            }
            return null;
        }
    }
}
