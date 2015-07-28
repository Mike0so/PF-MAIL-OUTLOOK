using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PowerFolder.Http
{
    public class PFResponse
    {
        public string Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public WebExceptionStatus ExceptionStatus { get; set; }

    }

    public static class PFResponseHelper
    {
        /// <summary>
        /// Validates the current PFResponse.
        /// 
        /// It will inform the current user about one of the following problems :
        /// 
        /// StatusCode = 401 || 403 false
        /// Timeout || ProtocolError || ConnectFailure false
        /// </summary>
        /// <param name="response">Current PFResponse</param>
        /// <param name="item">Outlook.Mail Item that should be handled</param>
        /// <returns></returns>
        public static bool IsValidResponse(this PFResponse response, Microsoft.Office.Interop.Outlook.MailItem item)
        {
            
            if (response.StatusCode == HttpStatusCode.Unauthorized ||
                response.StatusCode == HttpStatusCode.Forbidden)
            {
                MessageBox.Show(Properties.Resources.http_unauthorized,
                Properties.Resources.application_title);
                item.Display();
                return false;
            }
            if (response.ExceptionStatus != WebExceptionStatus.Success)
            {
                if (response.ExceptionStatus == WebExceptionStatus.Timeout)
                {
                   MessageBox.Show(Properties.Resources.http_timeout,
                        Properties.Resources.application_title);
                   item.Display();
                   return false;
                }
                if (response.ExceptionStatus == WebExceptionStatus.ProtocolError)
                {
                    if (response.StatusCode == HttpStatusCode.NotFound)
                    {
                        return true;
                    }
                    MessageBox.Show(Properties.Resources.http_connect_failure,
                        Properties.Resources.application_title);
                    item.Display();
                    return false;
                }
                if (response.ExceptionStatus == WebExceptionStatus.ConnectFailure)
                {
                    MessageBox.Show(Properties.Resources.http_connect_failure,
                        Properties.Resources.application_title);
                    item.Display();
                    return false;
                }
                return false;
            }
            return true;
        }

        public static bool IsJSONResponse(this PFResponse response)
        {
            if (!string.IsNullOrEmpty(response.Message))
            {
                if (response.Message.StartsWith("{"))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
