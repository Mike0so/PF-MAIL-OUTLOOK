using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PowerFolder
{
    public partial class Ribbon
    {
        private void Ribbon_Load(object sender, RibbonUIEventArgs e) { }

        private void pf_btn_homepage_Click(object sender, RibbonControlEventArgs e)
        {
            Process.Start("https://www.powerfolder.com");
        }

        private void pf_btn_documentation_Click(object sender, RibbonControlEventArgs e)
        {
            Process.Start("https://wiki.powerfolder.com/display/PFC/PowerFolder+Outlook+Add-In+%28Beta%29+Documentation");
        }

        private void button_preferences_Click(object sender, RibbonControlEventArgs e)
        {
            PreferencesForm.GetInstance().Show();
        }

        private void btn_register_Click(object sender, RibbonControlEventArgs e)
        {
            Process.Start(Configuration.ConfigurationManager.GetInstance().GetConfig().BaseUrl + "/register");
        }

        private void update_btn_Click(object sender, RibbonControlEventArgs e)
        {
            Update.Updater updater = new Update.Updater(true);
            Task result = updater.CheckVersionAsync(true);
        }


    }
}
