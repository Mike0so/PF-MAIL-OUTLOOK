using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Config = PowerFolder.Configuration.ConfigurationManager;
using Logger = PowerFolder.Logging.Log;

namespace PowerFolder
{

    public partial class FileLinkDialog : Form
    {
        private static FileLinkDialog _instance;

        private const string _classname = "[FileLinkDialog]";

        public bool _canceled { get; private set; }

        public Dictionary<string, string> linkParams;


        private FileLinkDialog()
        {
            InitializeComponent();

            linkParams = new Dictionary<string, string>();
        }


        #region Listener
        private void btn_ok_Click(object sender, EventArgs e)
        {
            _canceled = false;
            const string _methodname = "[btn_ok_Click]";

            if (!string.IsNullOrEmpty(textbox_maxDownloads.Text))
            {
                if (Utils.PFUtils.TryToParseInteger(textbox_maxDownloads.Text))
                {
                    SetFileLinkParam(FileLinkContraints.MAX_DOWNLOAD, textbox_maxDownloads.Text);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.filelink_error_maxDownload);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(textbox_password.Text))
            {
                SetFileLinkParam(FileLinkContraints.PASSWORD, textbox_password.Text);
            }

            if (!string.IsNullOrEmpty(textbox_validTill.Text))
            {
                if (Utils.PFUtils.TryToParseInteger(textbox_validTill.Text))
                {
                    SetFileLinkParam(FileLinkContraints.VALIDITY, textbox_validTill.Text);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.filelink_error_validity);
                    return;
                }
            }

            Logger.LogThis(string.Format("{0} {1} [Saved link properties with params" + 
            "maxDownloads: {2}, validTill: {3}]",
                _classname,
                _methodname,
                textbox_maxDownloads.Text,
                textbox_validTill.Text), Logging.eloglevel.verbose);

            if (checkbox_dont_ask.Checked)
            {
                Config.GetInstance().GetConfig().FileLinkDialogEachEmail = false;
                Config.GetInstance().SaveConfig(Config.GetInstance().GetConfig());
            }

            ClearControls();

            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            _canceled = true;

            ClearControls();

            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void FileLinkDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            _canceled = true;

            ClearControls();

            this.Hide();
        }

        private void FileLinkDialog_Load(object sender, EventArgs e)
        {
            Configuration.Configuration config = Config.GetInstance().GetConfig();

            /* Add default values to file link dialog */
            if (config.GetDefaultValues().Count > 0)
            {
                if (!string.IsNullOrEmpty(config.FileLinkValidFor))
                {
                    textbox_validTill.Text = config.FileLinkValidFor;
                }
                if (!string.IsNullOrEmpty(config.FileLinkDownloadCount))
                {
                    textbox_maxDownloads.Text = config.FileLinkDownloadCount;
                }
            }
        }
        #endregion

        /// <summary>
        /// Add a new file link parameter to the FileLinkCollection.
        /// If the constaint already exists the old value will be overwritten by the new one.
        /// </summary>
        /// <param name="constrait">The file link parameter</param>
        /// <param name="value">The value to be add</param>
        public void SetFileLinkParam(string fileLinkContraint, string value)
        {
            switch (fileLinkContraint)
            {
                case FileLinkContraints.PASSWORD:
                    if (linkParams.ContainsKey(FileLinkContraints.PASSWORD))
                    {
                        linkParams.Remove(FileLinkContraints.PASSWORD);
                    }
                    linkParams.Add(FileLinkContraints.PASSWORD, value);
                    break;
                case FileLinkContraints.MAX_DOWNLOAD:
                    if (linkParams.ContainsKey(FileLinkContraints.MAX_DOWNLOAD))
                    {
                        linkParams.Remove(FileLinkContraints.MAX_DOWNLOAD);
                    }
                    linkParams.Add(FileLinkContraints.MAX_DOWNLOAD, value);
                    break;
                case FileLinkContraints.VALIDITY:
                    if (linkParams.ContainsKey(FileLinkContraints.VALIDITY))
                    {
                        linkParams.Remove(FileLinkContraints.VALIDITY);
                    }
                    linkParams.Add(FileLinkContraints.VALIDITY, value);
                    break;
                default:
                    break;
            }
        }

        public Dictionary<string, string> GetFileLinkParams(bool clearDictionary = false)
        {
            Dictionary<string, string> paramters = new Dictionary<string, string>();

            foreach (string key in linkParams.Keys)
            {
                string value = linkParams[key];
                paramters.Add(key, value);
            }
            if (clearDictionary)
            {
                linkParams.Clear();
            }
            return paramters;
        }

        public void ClearControls()
        {
            const string _methodname = "[ClearControls]";
            try
            {
                foreach (TextBox txtBox in this.Controls.OfType<TextBox>())
                {
                    txtBox.Text = string.Empty;
                }
            }
            catch (Exception e)
            {
                textbox_maxDownloads.Text = string.Empty;
                textbox_password.Text = string.Empty;
                textbox_validTill.Text = string.Empty;
                Logger.LogThis(string.Format("{0} {1} [Exception : {2}]", _classname, _methodname, e.Message), Logging.eloglevel.error);                
            }
        }

        public static FileLinkDialog GetInstance()
        {
            if (_instance == null)
            {
                return _instance = new FileLinkDialog();
            }
            return _instance;
        }

    }

    public static class FileLinkContraints
    {
        public const string MAX_DOWNLOAD = "downloads";
        public const string PASSWORD = "password";
        public const string VALIDITY = "valid";
    }
}
