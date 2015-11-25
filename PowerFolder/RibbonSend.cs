using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace PowerFolder
{
    public partial class RibbonSend
    {   
        /// <summary>
        /// Defines if Attachments will be handled by add-in or outlook
        /// </summary>
        public bool _track
        {
            private set;
            get;
        }

        private void RibbonSend_Load(object sender, RibbonUIEventArgs e)
        {

            _track = Configuration.ConfigurationManager.GetInstance().GetTrackEmails();

            stop_track_checkbox.Checked = _track;

            checkBox1.Checked = Configuration.ConfigurationManager.GetInstance().GetShowFileLinkDialog();
        }

        private void stop_track_checkbox_Click(object sender, RibbonControlEventArgs e)
        {
            _track = stop_track_checkbox.Checked;
            Configuration.ConfigurationManager.GetInstance().SetTrackEmails(_track);
            Configuration.ConfigurationManager.GetInstance().SaveConfig();
        }

        private void tab_group_DialogLauncherClick(object sender, RibbonControlEventArgs e)
        {
            FileLinkDialog.GetInstance().Show();
        }

        private void checkBox1_Click(object sender, RibbonControlEventArgs e)
        {
            Configuration.ConfigurationManager.GetInstance().SetShowFileLinkDialog(checkBox1.Checked);
            Configuration.ConfigurationManager.GetInstance().SaveConfig();
        }
    }
}
