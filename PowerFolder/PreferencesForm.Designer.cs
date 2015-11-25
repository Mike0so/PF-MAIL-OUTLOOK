namespace PowerFolder
{
    partial class PreferencesForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreferencesForm));
            this.btn_ok = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.picturebox_header = new System.Windows.Forms.PictureBox();
            this.label_server = new System.Windows.Forms.Label();
            this.combobox_server_prefix = new System.Windows.Forms.ComboBox();
            this.textbox_server = new System.Windows.Forms.TextBox();
            this.textbox_username = new System.Windows.Forms.TextBox();
            this.textbox_password = new System.Windows.Forms.TextBox();
            this.label_username = new System.Windows.Forms.Label();
            this.label_password = new System.Windows.Forms.Label();
            this.linklabel_pw_recovery = new System.Windows.Forms.LinkLabel();
            this.lbl_networkConfig = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label_validTill2 = new System.Windows.Forms.Label();
            this.textbox_validTill = new System.Windows.Forms.TextBox();
            this.label_validTill1 = new System.Windows.Forms.Label();
            this.textbox_maxDownloads = new System.Windows.Forms.TextBox();
            this.label_maxDownloads = new System.Windows.Forms.Label();
            this.version_lbl = new System.Windows.Forms.Label();
            this.trackbar_minFilesize = new System.Windows.Forms.TrackBar();
            this.label_filesize = new System.Windows.Forms.Label();
            this.lbl_filesize2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.picturebox_header)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackbar_minFilesize)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_ok
            // 
            resources.ApplyResources(this.btn_ok, "btn_ok");
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.OnSave);
            // 
            // btn_cancel
            // 
            this.btn_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btn_cancel, "btn_cancel");
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // progressBar
            // 
            resources.ApplyResources(this.progressBar, "progressBar");
            this.progressBar.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.progressBar.MarqueeAnimationSpeed = 20;
            this.progressBar.Name = "progressBar";
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            // 
            // picturebox_header
            // 
            this.picturebox_header.Image = global::PowerFolder.Properties.Resources.pref_header;
            resources.ApplyResources(this.picturebox_header, "picturebox_header");
            this.picturebox_header.Name = "picturebox_header";
            this.picturebox_header.TabStop = false;
            this.picturebox_header.Click += new System.EventHandler(this.OnLogo_Click);
            // 
            // label_server
            // 
            resources.ApplyResources(this.label_server, "label_server");
            this.label_server.Name = "label_server";
            // 
            // combobox_server_prefix
            // 
            this.combobox_server_prefix.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combobox_server_prefix.FormattingEnabled = true;
            resources.ApplyResources(this.combobox_server_prefix, "combobox_server_prefix");
            this.combobox_server_prefix.Name = "combobox_server_prefix";
            // 
            // textbox_server
            // 
            resources.ApplyResources(this.textbox_server, "textbox_server");
            this.textbox_server.Name = "textbox_server";
            // 
            // textbox_username
            // 
            resources.ApplyResources(this.textbox_username, "textbox_username");
            this.textbox_username.Name = "textbox_username";
            // 
            // textbox_password
            // 
            resources.ApplyResources(this.textbox_password, "textbox_password");
            this.textbox_password.Name = "textbox_password";
            this.textbox_password.UseSystemPasswordChar = true;
            // 
            // label_username
            // 
            resources.ApplyResources(this.label_username, "label_username");
            this.label_username.Name = "label_username";
            // 
            // label_password
            // 
            resources.ApplyResources(this.label_password, "label_password");
            this.label_password.Name = "label_password";
            // 
            // linklabel_pw_recovery
            // 
            resources.ApplyResources(this.linklabel_pw_recovery, "linklabel_pw_recovery");
            this.linklabel_pw_recovery.Name = "linklabel_pw_recovery";
            this.linklabel_pw_recovery.TabStop = true;
            this.linklabel_pw_recovery.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnPasswordRecovery_Click);
            // 
            // lbl_networkConfig
            // 
            resources.ApplyResources(this.lbl_networkConfig, "lbl_networkConfig");
            this.lbl_networkConfig.ForeColor = System.Drawing.SystemColors.ActiveBorder;
            this.lbl_networkConfig.Name = "lbl_networkConfig";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.SystemColors.ActiveBorder;
            this.label1.Name = "label1";
            // 
            // label_validTill2
            // 
            resources.ApplyResources(this.label_validTill2, "label_validTill2");
            this.label_validTill2.Name = "label_validTill2";
            // 
            // textbox_validTill
            // 
            resources.ApplyResources(this.textbox_validTill, "textbox_validTill");
            this.textbox_validTill.Name = "textbox_validTill";
            // 
            // label_validTill1
            // 
            resources.ApplyResources(this.label_validTill1, "label_validTill1");
            this.label_validTill1.Name = "label_validTill1";
            // 
            // textbox_maxDownloads
            // 
            resources.ApplyResources(this.textbox_maxDownloads, "textbox_maxDownloads");
            this.textbox_maxDownloads.Name = "textbox_maxDownloads";
            // 
            // label_maxDownloads
            // 
            resources.ApplyResources(this.label_maxDownloads, "label_maxDownloads");
            this.label_maxDownloads.Name = "label_maxDownloads";
            // 
            // version_lbl
            // 
            resources.ApplyResources(this.version_lbl, "version_lbl");
            this.version_lbl.Name = "version_lbl";
            // 
            // trackbar_minFilesize
            // 
            this.trackbar_minFilesize.LargeChange = 1024;
            resources.ApplyResources(this.trackbar_minFilesize, "trackbar_minFilesize");
            this.trackbar_minFilesize.Maximum = 102400;
            this.trackbar_minFilesize.Name = "trackbar_minFilesize";
            this.trackbar_minFilesize.SmallChange = 512;
            this.trackbar_minFilesize.ValueChanged += new System.EventHandler(this.trackbar_filesize_ValueChanged);
            // 
            // label_filesize
            // 
            resources.ApplyResources(this.label_filesize, "label_filesize");
            this.label_filesize.Name = "label_filesize";
            // 
            // lbl_filesize2
            // 
            resources.ApplyResources(this.lbl_filesize2, "lbl_filesize2");
            this.lbl_filesize2.Name = "lbl_filesize2";
            // 
            // PreferencesForm
            // 
            this.AcceptButton = this.btn_ok;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btn_cancel;
            this.Controls.Add(this.lbl_filesize2);
            this.Controls.Add(this.label_filesize);
            this.Controls.Add(this.trackbar_minFilesize);
            this.Controls.Add(this.version_lbl);
            this.Controls.Add(this.label_validTill2);
            this.Controls.Add(this.textbox_validTill);
            this.Controls.Add(this.label_validTill1);
            this.Controls.Add(this.textbox_maxDownloads);
            this.Controls.Add(this.label_maxDownloads);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl_networkConfig);
            this.Controls.Add(this.linklabel_pw_recovery);
            this.Controls.Add(this.label_password);
            this.Controls.Add(this.label_username);
            this.Controls.Add(this.textbox_password);
            this.Controls.Add(this.textbox_username);
            this.Controls.Add(this.combobox_server_prefix);
            this.Controls.Add(this.textbox_server);
            this.Controls.Add(this.label_server);
            this.Controls.Add(this.picturebox_header);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_ok);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.Name = "PreferencesForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PreferencesForm_FormClosing);
            this.Load += new System.EventHandler(this.OnLoad);
            ((System.ComponentModel.ISupportInitialize)(this.picturebox_header)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackbar_minFilesize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.PictureBox picturebox_header;
        private System.Windows.Forms.Label label_server;
        private System.Windows.Forms.ComboBox combobox_server_prefix;
        private System.Windows.Forms.TextBox textbox_server;
        private System.Windows.Forms.TextBox textbox_username;
        private System.Windows.Forms.TextBox textbox_password;
        private System.Windows.Forms.Label label_username;
        private System.Windows.Forms.Label label_password;
        private System.Windows.Forms.LinkLabel linklabel_pw_recovery;
        private System.Windows.Forms.Label lbl_networkConfig;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_validTill2;
        private System.Windows.Forms.TextBox textbox_validTill;
        private System.Windows.Forms.Label label_validTill1;
        private System.Windows.Forms.TextBox textbox_maxDownloads;
        private System.Windows.Forms.Label label_maxDownloads;
        private System.Windows.Forms.Label version_lbl;
        private System.Windows.Forms.TrackBar trackbar_minFilesize;
        private System.Windows.Forms.Label label_filesize;
        private System.Windows.Forms.Label lbl_filesize2;
    }
}