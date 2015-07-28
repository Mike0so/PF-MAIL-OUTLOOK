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
        public string _linkPassword { get; private set; }
        public string _validTill { get; private set; }
        public string _maxDownloads { get; private set; }
        public bool _canceled { get; private set; }

        public Dictionary<string, string> linkParams;
        private FileLinkDialog()
        {
            InitializeComponent();

            label_password.Visible = true;
            textbox_password.Visible = true;

            linkParams = new Dictionary<string, string>();
        }


        public static FileLinkDialog GetInstance()
        {
            if (_instance == null)
            {
                return _instance = new FileLinkDialog();
            }
            return _instance;
        }

        private void btn_ok_Click(object sender, EventArgs e)
        {
            _canceled = false;
            const string _methodname = "[btn_ok_Click]";

                if (string.IsNullOrEmpty(textbox_password.Text))
                {
                    if (linkParams != null)
                    {
                        if (linkParams.Keys.Contains("password"))
                        {
                            linkParams.Remove("password");
                        }
                    }
                }
                else
                {
                    _linkPassword = textbox_password.Text;

                    if (linkParams.Keys.Contains("password"))
                    {
                        linkParams.Remove("password");
                    }
                        linkParams.Add("password", _linkPassword);
                }
            
            int parser = 0;

            if (!string.IsNullOrEmpty(textbox_validTill.Text))
            {
                try 
                {
                    parser = int.Parse(textbox_validTill.Text);
                }
                catch (FormatException) 
                {
                    MessageBox.Show("Please enter a number for the validity of the file link.",
                        Properties.Resources.application_title);
                    
                    return;
                }
                _validTill = textbox_validTill.Text;
                if (linkParams.Keys.Contains("valid"))
                {
                    linkParams.Remove("valid");
                }
                    linkParams.Add("valid", _validTill);
            }
            else
            {
                if (linkParams.Keys.Contains("valid"))
                {
                    linkParams.Remove("valid");
                }
            }
            if (!string.IsNullOrEmpty(textbox_maxDownloads.Text))
            {
                try 
                {
                    parser = int.Parse(textbox_maxDownloads.Text);
                }
                catch (FormatException) 
                {
                    MessageBox.Show("Please enter a number for the maximum download count of the file link.",
                        Properties.Resources.application_title);
                    return;
                }
                _maxDownloads = textbox_maxDownloads.Text;
                if (linkParams.Keys.Contains("downloads"))
                {
                    linkParams.Remove("downloads");
                }
                    linkParams.Add("downloads", _maxDownloads);
            }
            else
            {
                if (linkParams.Keys.Contains("downloads"))
                {
                    linkParams.Remove("downloads");
                }
            }

            Logger.LogThis(string.Format("{0} {1} [Saved link properties with params" + 
            "maxDownloads: {2}, validTill: {3}]",
                _classname,
                _methodname,
                textbox_maxDownloads.Text,
                textbox_validTill.Text), Logging.eloglevel.verbose);


            textbox_password.Visible = false;
            label_password.Visible = false;

            textbox_password.Text = string.Empty;
            textbox_maxDownloads.Text = string.Empty;
            textbox_validTill.Text = string.Empty;
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void btn_cancel_Click(object sender, EventArgs e)
        {
            _canceled = true;

            textbox_password.Visible = false;
            label_password.Visible = false;

            textbox_password.Text = string.Empty;
            textbox_maxDownloads.Text = string.Empty;
            textbox_validTill.Text = string.Empty;
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        //Deprecated
        public void ConfigurePasswordControls()
        {
            label_password.Visible = true;
            textbox_password.Visible = true;
            this.Update();
        }
    }
}
