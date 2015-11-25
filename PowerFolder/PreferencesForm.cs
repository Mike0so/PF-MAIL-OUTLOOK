using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PowerFolder.Configuration;
using PowerFolder.Http;
using System.Diagnostics;
using PowerFolder.Utils;

namespace PowerFolder
{
    public partial class PreferencesForm : Form
    {
        /// <summary>
        /// Delegate for coupled progressbar
        /// </summary>
        /// <param name="state">Enabled</param>
        delegate void HandleProgressBarDelegate(bool state);

        /// <summary>
        /// Configuration instance
        /// </summary>
        private ConfigurationManager _configManager;

        /// <summary>
        /// Instance of this form
        /// </summary>
        private static PreferencesForm _instance;

        private PreferencesForm()
        {
            InitializeComponent();

            _configManager = Configuration.ConfigurationManager.GetInstance();

            combobox_server_prefix.Items.Add("http://");
            combobox_server_prefix.Items.Add("https://");

            version_lbl.Text = string.Format("Version : {0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 5));
        }

        /// <summary>
        /// Get the current running instance
        /// </summary>
        /// <returns></returns>
        public static PreferencesForm GetInstance()
        {
            if (_instance == null)
            {
                return _instance = new PreferencesForm();
            }
            return _instance;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            combobox_server_prefix.SelectedItem = _configManager.GetBaseUrlPrefix();

            textbox_server.Text = _configManager.GetBaseUrl();
            textbox_username.Text = _configManager.GetUsername();

            if (!string.IsNullOrEmpty(_configManager.GetPassword()))
            {
                textbox_password.Text = _configManager.GetPassword();
            }

            int defaultDownloads = _configManager.GetDefaultDownloads();
            int defaultValidity = _configManager.GetDefaultValidity();

            if (defaultDownloads > 0)
            {
                textbox_maxDownloads.Text = _configManager.GetDefaultDownloads().ToString();
            }
            if (defaultValidity > 0)
            {
                textbox_validTill.Text = _configManager.GetDefaultValidity().ToString();
            }

            trackbar_minFilesize.Value = int.Parse((_configManager.GetMinFileSize() / 1024).ToString());
            lbl_filesize2.Text = PFUtils.FormatBytes(trackbar_minFilesize.Value * 1024);
        }

        private void OnLogo_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("www.powerfolder.com");
        }

        private void OnPasswordRecovery_Click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(string.IsNullOrEmpty(textbox_server.Text) ||
                string.IsNullOrEmpty(textbox_username.Text))
            {
                MessageBox.Show(Properties.Resources.config_password_recovery, Properties.Resources.application_title);
                return;
            }
            Process.Start(string.Format("{0}{1}/password_recover?Username={2}", combobox_server_prefix.SelectedItem.ToString(), textbox_server.Text, textbox_username.Text));
        }

        private void HandleProgressBar(bool state)
        {
            if (progressBar.InvokeRequired)
            {
                HandleProgressBarDelegate del = new HandleProgressBarDelegate(HandleProgressBar);
                this.Invoke(del, new object[] { state });
            }
            else
            {
                progressBar.Visible = state;
                progressBar.Enabled = state;
            }
        }

        private void OnSave(object sender, EventArgs e)
        {
            HandleProgressBar(true);

            if (!ComponentsFilled())
            {
                HandleProgressBar(false);
                MessageBox.Show(Properties.Resources.config_empty, Properties.Resources.application_title);
                return;
            }

            _configManager.SetUsername(textbox_username.Text);
            _configManager.SetBaseURL(combobox_server_prefix.SelectedItem.ToString(), textbox_server.Text);
            _configManager.SetPassword(textbox_password.Text);

            string maxDownloads = textbox_maxDownloads.Text;
            string validity = textbox_validTill.Text;

            try
            {
                _configManager.SetDefaultDownloads(maxDownloads);
            }
            catch (FormatException)
            {
                MessageBox.Show("The maximum download count of a file link must be a number", Properties.Resources.application_title);
                return;
            }

            try
            {
                _configManager.SetDefaultDownloads(validity);
            }
            catch (FormatException)
            {
                MessageBox.Show("The validity of a file link must be a number", Properties.Resources.application_title);
                return;
            }

            _configManager.SetMinFileSize((trackbar_minFilesize.Value * 1024).ToString());
            _configManager.SaveConfig();

            PFApi apiCall = new PFApi();

            if (!apiCall.TryToAuthenticate())
            {
                HandleProgressBar(false);
                MessageBox.Show(Properties.Resources.http_unauthorized, Properties.Resources.application_title);
                return;
            }
            HandleProgressBar(false);
            this.Hide();
        }

        private bool ComponentsFilled()
        {
            if(string.IsNullOrEmpty(textbox_server.Text) ||
                string.IsNullOrEmpty(textbox_username.Text) ||
                string.IsNullOrEmpty(textbox_password.Text))
            {
                return false;
            }
            return true;
        }

        private void PreferencesForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            HandleProgressBar(false);
            this.Hide();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void trackbar_filesize_ValueChanged(object sender, EventArgs e)
        {
            lbl_filesize2.Text = PFUtils.FormatBytes(trackbar_minFilesize.Value * 1024);
        }
    }
}
