using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;

namespace PowerFolder
{
    public partial class RibbonSend
    {        
        public bool _track
        {
            private set;
            get;
        }

        private void RibbonSend_Load(object sender, RibbonUIEventArgs e)
        {

            _track = Configuration.ConfigurationManager.GetInstance().GetConfig().TrackEmails;
            stop_track_checkbox.Checked = _track;
            checkBox1.Checked = Configuration.ConfigurationManager.GetInstance().GetConfig().FileLinkDialogEachEmail;
        }

        private void stop_track_checkbox_Click(object sender, RibbonControlEventArgs e)
        {
            _track = stop_track_checkbox.Checked;
            Configuration.ConfigurationManager.GetInstance().GetConfig().TrackEmails = _track;

            Configuration.ConfigurationManager.GetInstance().SaveConfig(
                Configuration.ConfigurationManager.GetInstance().GetConfig());
        }

        private void tab_group_DialogLauncherClick(object sender, RibbonControlEventArgs e)
        {
            FileLinkDialog.GetInstance().ConfigurePasswordControls();
            FileLinkDialog.GetInstance().Show();
        }

        private void checkBox1_Click(object sender, RibbonControlEventArgs e)
        {
            Configuration.ConfigurationManager.GetInstance().GetConfig()
                .FileLinkDialogEachEmail = checkBox1.Checked;

            Configuration.ConfigurationManager.GetInstance().SaveConfig(
                Configuration.ConfigurationManager.GetInstance().GetConfig());
        }
    }
}
