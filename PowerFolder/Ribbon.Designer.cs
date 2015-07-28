namespace PowerFolder
{
    partial class Ribbon : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Ribbon()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">"true", wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls "false".</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für Designerunterstützung -
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Ribbon));
            this.mainTab = this.Factory.CreateRibbonTab();
            this.group = this.Factory.CreateRibbonGroup();
            this.button_preferences = this.Factory.CreateRibbonButton();
            this.update_btn = this.Factory.CreateRibbonButton();
            this.button_pfmenu = this.Factory.CreateRibbonMenu();
            this.pf_btn_homepage = this.Factory.CreateRibbonButton();
            this.pf_btn_documentation = this.Factory.CreateRibbonButton();
            this.btn_register = this.Factory.CreateRibbonButton();
            this.mainTab.SuspendLayout();
            this.group.SuspendLayout();
            // 
            // mainTab
            // 
            this.mainTab.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.mainTab.Groups.Add(this.group);
            resources.ApplyResources(this.mainTab, "mainTab");
            this.mainTab.Name = "mainTab";
            // 
            // group
            // 
            this.group.Items.Add(this.button_preferences);
            this.group.Items.Add(this.update_btn);
            this.group.Items.Add(this.button_pfmenu);
            this.group.Name = "group";
            // 
            // button_preferences
            // 
            this.button_preferences.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.button_preferences.Image = global::PowerFolder.Properties.Resources.preferences_icon;
            resources.ApplyResources(this.button_preferences, "button_preferences");
            this.button_preferences.Name = "button_preferences";
            this.button_preferences.ShowImage = true;
            this.button_preferences.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.button_preferences_Click);
            // 
            // update_btn
            // 
            this.update_btn.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.update_btn.Image = global::PowerFolder.Properties.Resources.agt_update_misc;
            resources.ApplyResources(this.update_btn, "update_btn");
            this.update_btn.Name = "update_btn";
            this.update_btn.ShowImage = true;
            this.update_btn.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.update_btn_Click);
            // 
            // button_pfmenu
            // 
            this.button_pfmenu.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            resources.ApplyResources(this.button_pfmenu, "button_pfmenu");
            this.button_pfmenu.Items.Add(this.pf_btn_homepage);
            this.button_pfmenu.Items.Add(this.pf_btn_documentation);
            this.button_pfmenu.Items.Add(this.btn_register);
            this.button_pfmenu.Name = "button_pfmenu";
            this.button_pfmenu.ShowImage = true;
            // 
            // pf_btn_homepage
            // 
            resources.ApplyResources(this.pf_btn_homepage, "pf_btn_homepage");
            this.pf_btn_homepage.Name = "pf_btn_homepage";
            this.pf_btn_homepage.ShowImage = true;
            this.pf_btn_homepage.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.pf_btn_homepage_Click);
            // 
            // pf_btn_documentation
            // 
            resources.ApplyResources(this.pf_btn_documentation, "pf_btn_documentation");
            this.pf_btn_documentation.Name = "pf_btn_documentation";
            this.pf_btn_documentation.ShowImage = true;
            this.pf_btn_documentation.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.pf_btn_documentation_Click);
            // 
            // btn_register
            // 
            resources.ApplyResources(this.btn_register, "btn_register");
            this.btn_register.Name = "btn_register";
            this.btn_register.ShowImage = true;
            this.btn_register.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btn_register_Click);
            // 
            // Ribbon
            // 
            this.Name = "Ribbon";
            this.RibbonType = "Microsoft.Outlook.Explorer";
            this.Tabs.Add(this.mainTab);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon_Load);
            this.mainTab.ResumeLayout(false);
            this.mainTab.PerformLayout();
            this.group.ResumeLayout(false);
            this.group.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab mainTab;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup group;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton button_preferences;
        internal Microsoft.Office.Tools.Ribbon.RibbonMenu button_pfmenu;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton pf_btn_homepage;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton pf_btn_documentation;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btn_register;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton update_btn;
    }

    partial class ThisRibbonCollection
    {
        internal Ribbon Ribbon
        {
            get { return this.GetRibbon<Ribbon>(); }
        }
    }
}
