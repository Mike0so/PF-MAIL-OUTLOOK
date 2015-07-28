using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using PowerFolder.Configuration;
using PowerFolder.Http;
using System.Diagnostics;

namespace PowerFolder
{
    public partial class PreferencesForm : Form
    {
        delegate void HandleProgressBarDelegate(bool state);

        private ConfigurationManager _config;
        private static PreferencesForm _instance;

        public PreferencesForm()
        {
            InitializeComponent();

            _config = Configuration.ConfigurationManager.GetInstance();

            combobox_server_prefix.Items.Add("http://");
            combobox_server_prefix.Items.Add("https://");

            version_lbl.Text = string.Format("Current Version : {0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString().Substring(0, 5));
        }

        public static PreferencesForm GetInstance()
        {
            if (_instance == null)
            {
                return _instance = new PreferencesForm();
            }
            return _instance;
        }

        private void PreferencesForm_Load(object sender, EventArgs e)
        {
            string[] seperator = new string[] { "://" };
            string[] url = _config.GetConfig().BaseUrl
                .Split(seperator, StringSplitOptions.RemoveEmptyEntries);

            combobox_server_prefix.SelectedItem = string.Format("{0}://", url[0]);

            textbox_server.Text = url[1];
            textbox_username.Text = _config.GetConfig().Username;

            if (!string.IsNullOrEmpty(_config.GetConfig().Password))
            {
                textbox_password.Text = Security.SecurityManager.Decrypt(
                    _config.GetConfig().Password);
            }

            textbox_maxDownloads.Text = _config.GetConfig().FileLinkDownloadCount;
            textbox_validTill.Text = _config.GetConfig().FileLinkValidFor;
            checkbox_useDefaultConfig.Checked = _config.GetConfig().UseDefaultFileLinkConfig;
        }

        private void picturebox_header_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("www.powerfolder.com");
        }

        private void linklabel_pw_recovery_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if(string.IsNullOrEmpty(textbox_server.Text) ||
                string.IsNullOrEmpty(textbox_username.Text))
            {
                MessageBox.Show(Properties.Resources.config_password_recovery, Properties.Resources.application_title);
                return;
            }
            Process.Start(string.Format("{0}/login?Username={1}", textbox_server.Text, textbox_username.Text));
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

        private void btn_ok_Click(object sender, EventArgs e)
        {
            HandleProgressBar(true);

            if (!ComponentsFilled())
            {
                MessageBox.Show(Properties.Resources.config_empty, Properties.Resources.application_title);                
                return;
            }

            _config.GetConfig().Username = textbox_username.Text;
            _config.GetConfig().BaseUrl = string.Format("{0}{1}", combobox_server_prefix.SelectedItem, textbox_server.Text);
            _config.GetConfig().UseDefaultFileLinkConfig = bool.Parse(checkbox_useDefaultConfig.Checked.ToString());
            _config.GetConfig().Password = Security.SecurityManager.Encrypt(textbox_password.Text);

            int parser = 0;

            if (!string.IsNullOrEmpty(textbox_maxDownloads.Text))
            {
                try
                {
                    parser = int.Parse(textbox_maxDownloads.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show("The maximum download count of a file link must be a number", Properties.Resources.application_title);
                    return;
                }
                _config.GetConfig().FileLinkDownloadCount = textbox_maxDownloads.Text;
            }

            if (!string.IsNullOrEmpty(textbox_validTill.Text)) {
                try
                {
                    parser = int.Parse(textbox_validTill.Text);
                }
                catch (FormatException)
                {
                    MessageBox.Show("The validity of a file link must be a number", Properties.Resources.application_title);
                    return;
                }
                _config.GetConfig().FileLinkValidFor = textbox_validTill.Text;
            }

            _config.SaveConfig(_config.GetConfig());

            PFApi apiCall = new PFApi();

            if (!apiCall.CanAuthenticate())
            {
                HandleProgressBar(false);
                MessageBox.Show(Properties.Resources.config_unvalid_credentials, Properties.Resources.application_title);
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

    }
}
