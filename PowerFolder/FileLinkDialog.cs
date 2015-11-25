using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using _configManager = PowerFolder.Configuration.ConfigurationManager;
using Logger = PowerFolder.Logging.Log;

namespace PowerFolder
{
    /// <summary>
    /// This form is used to configure parameters for file link generation.
    /// </summary>
    public partial class FileLinkDialog : Form
    {
        /// <summary>
        /// Name of this class
        /// </summary>
        private const string _classname = "[FileLinkDialog]";

        /// <summary>
        /// Little hack
        /// </summary>
        public bool _canceled { get; private set; }

        /// <summary>
        /// Current running instance
        /// </summary>
        private static FileLinkDialog _instance;

        /// <summary>
        /// Container holding all entered options to generate FileLinks
        /// </summary>
        public Dictionary<string, string> linkParams;

        private FileLinkDialog()
        {
            InitializeComponent();

            linkParams = new Dictionary<string, string>();
        }


        #region Listener
        private void OnSave_Click(object sender, EventArgs e)
        {
            _canceled = false;
            const string _methodname = "[btn_ok_Click]";

            if (!string.IsNullOrEmpty(textbox_maxDownloads.Text))
            {
                if (Utils.PFUtils.TryToParseInteger(textbox_maxDownloads.Text))
                {
                    SetFileLinkParameter(FileLinkContraints.MAX_DOWNLOAD, textbox_maxDownloads.Text);
                }
                else
                {
                    MessageBox.Show(Properties.Resources.filelink_error_maxDownload);
                    return;
                }
            }

            if (!string.IsNullOrEmpty(textbox_password.Text))
            {
                SetFileLinkParameter(FileLinkContraints.PASSWORD, textbox_password.Text);
            }

            if (!string.IsNullOrEmpty(textbox_validTill.Text))
            {
                if (Utils.PFUtils.TryToParseInteger(textbox_validTill.Text))
                {
                    SetFileLinkParameter(FileLinkContraints.VALIDITY, textbox_validTill.Text);
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
                _configManager.GetInstance().SetShowFileLinkDialog(false);
                _configManager.GetInstance().SaveConfig();
            }

            ClearControls();

            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void OnCancel_Click(object sender, EventArgs e)
        {
            _canceled = true;

            ClearControls();

            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void OnForm_Closing(object sender, FormClosingEventArgs e)
        {
            _canceled = true;

            ClearControls();

            this.Hide();
        }

        private void OnForm_Loading(object sender, EventArgs e)
        {
            /* Add default values to file link dialog */
            if (_configManager.GetInstance().GetFileLinkDefaultValues().Count > 0)
            {
                try
                {
                    if (_configManager.GetInstance().GetDefaultValidity() > 0)
                    {
                        textbox_validTill.Text = _configManager.GetInstance().GetDefaultValidity().ToString();
                    }
                    if (_configManager.GetInstance().GetDefaultDownloads() > 0)
                    {
                        textbox_maxDownloads.Text = _configManager.GetInstance().GetDefaultDownloads().ToString();
                    }
                }
                catch (FormatException)
                {
                    textbox_maxDownloads.Text = string.Empty;
                    textbox_validTill.Text = string.Empty;
                }
                catch (Exception e1)
                {
                    Logger.LogThisError(e1);
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
        public void SetFileLinkParameter(string fileLinkContraint, string value)
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

        public Dictionary<string, string> GetFileLinkParameters(bool clearDictionary = false)
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

        /// <summary>
        /// Clears the input of all <para>TextBox</para> controls
        /// </summary>
        public void ClearControls()
        {
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
                Logger.LogThisError(e);                
            }
        }

        /// <summary>
        /// Get the current instance
        /// </summary>
        /// <returns>Instance of FileLinkDialog</returns>
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
