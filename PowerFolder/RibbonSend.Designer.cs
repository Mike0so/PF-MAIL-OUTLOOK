namespace PowerFolder
{
    partial class RibbonSend : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public RibbonSend()
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
            Microsoft.Office.Tools.Ribbon.RibbonDialogLauncher ribbonDialogLauncherImpl1 = this.Factory.CreateRibbonDialogLauncher();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RibbonSend));
            this.sendTab = this.Factory.CreateRibbonTab();
            this.tab_group = this.Factory.CreateRibbonGroup();
            this.stop_track_checkbox = this.Factory.CreateRibbonCheckBox();
            this.checkBox1 = this.Factory.CreateRibbonCheckBox();
            this.sendTab.SuspendLayout();
            this.tab_group.SuspendLayout();
            // 
            // sendTab
            // 
            this.sendTab.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.sendTab.ControlId.OfficeId = "TabNewMailMessage";
            this.sendTab.Groups.Add(this.tab_group);
            resources.ApplyResources(this.sendTab, "sendTab");
            this.sendTab.Name = "sendTab";
            // 
            // tab_group
            // 
            ribbonDialogLauncherImpl1.Image = global::PowerFolder.Properties.Resources.Icon48x481;
            ribbonDialogLauncherImpl1.ImageName = "File-Link Preferences";
            resources.ApplyResources(ribbonDialogLauncherImpl1, "ribbonDialogLauncherImpl1");
            this.tab_group.DialogLauncher = ribbonDialogLauncherImpl1;
            this.tab_group.Items.Add(this.stop_track_checkbox);
            this.tab_group.Items.Add(this.checkBox1);
            resources.ApplyResources(this.tab_group, "tab_group");
            this.tab_group.Name = "tab_group";
            this.tab_group.DialogLauncherClick += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.tab_group_DialogLauncherClick);
            // 
            // stop_track_checkbox
            // 
            this.stop_track_checkbox.Checked = true;
            resources.ApplyResources(this.stop_track_checkbox, "stop_track_checkbox");
            this.stop_track_checkbox.Name = "stop_track_checkbox";
            this.stop_track_checkbox.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.stop_track_checkbox_Click);
            // 
            // checkBox1
            // 
            resources.ApplyResources(this.checkBox1, "checkBox1");
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.checkBox1_Click);
            // 
            // RibbonSend
            // 
            this.Name = "RibbonSend";
            this.RibbonType = "Microsoft.Outlook.Mail.Compose";
            this.Tabs.Add(this.sendTab);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.RibbonSend_Load);
            this.sendTab.ResumeLayout(false);
            this.sendTab.PerformLayout();
            this.tab_group.ResumeLayout(false);
            this.tab_group.PerformLayout();

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab sendTab;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup tab_group;
        internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox stop_track_checkbox;
        internal Microsoft.Office.Tools.Ribbon.RibbonCheckBox checkBox1;
    }

    partial class ThisRibbonCollection
    {
        internal RibbonSend RibbonSend
        {
            get { return this.GetRibbon<RibbonSend>(); }
        }
    }
}
