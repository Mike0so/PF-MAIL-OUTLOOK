namespace PowerFolder
{
    partial class FileLinkDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileLinkDialog));
            this.textbox_password = new System.Windows.Forms.TextBox();
            this.label_password = new System.Windows.Forms.Label();
            this.label_maxDownloads = new System.Windows.Forms.Label();
            this.textbox_maxDownloads = new System.Windows.Forms.TextBox();
            this.label_validTill1 = new System.Windows.Forms.Label();
            this.textbox_validTill = new System.Windows.Forms.TextBox();
            this.label_validTill2 = new System.Windows.Forms.Label();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.btn_ok = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // textbox_password
            // 
            resources.ApplyResources(this.textbox_password, "textbox_password");
            this.textbox_password.Name = "textbox_password";
            this.textbox_password.UseSystemPasswordChar = true;
            // 
            // label_password
            // 
            resources.ApplyResources(this.label_password, "label_password");
            this.label_password.Name = "label_password";
            // 
            // label_maxDownloads
            // 
            resources.ApplyResources(this.label_maxDownloads, "label_maxDownloads");
            this.label_maxDownloads.Name = "label_maxDownloads";
            // 
            // textbox_maxDownloads
            // 
            resources.ApplyResources(this.textbox_maxDownloads, "textbox_maxDownloads");
            this.textbox_maxDownloads.Name = "textbox_maxDownloads";
            // 
            // label_validTill1
            // 
            resources.ApplyResources(this.label_validTill1, "label_validTill1");
            this.label_validTill1.Name = "label_validTill1";
            // 
            // textbox_validTill
            // 
            resources.ApplyResources(this.textbox_validTill, "textbox_validTill");
            this.textbox_validTill.Name = "textbox_validTill";
            // 
            // label_validTill2
            // 
            resources.ApplyResources(this.label_validTill2, "label_validTill2");
            this.label_validTill2.Name = "label_validTill2";
            // 
            // btn_cancel
            // 
            this.btn_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btn_cancel, "btn_cancel");
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // btn_ok
            // 
            resources.ApplyResources(this.btn_ok, "btn_ok");
            this.btn_ok.Name = "btn_ok";
            this.btn_ok.UseVisualStyleBackColor = true;
            this.btn_ok.Click += new System.EventHandler(this.btn_ok_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::PowerFolder.Properties.Resources.pref_header;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // FileLinkDialog
            // 
            this.AcceptButton = this.btn_ok;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.CancelButton = this.btn_cancel;
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btn_ok);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.label_validTill2);
            this.Controls.Add(this.textbox_validTill);
            this.Controls.Add(this.label_validTill1);
            this.Controls.Add(this.textbox_maxDownloads);
            this.Controls.Add(this.label_maxDownloads);
            this.Controls.Add(this.label_password);
            this.Controls.Add(this.textbox_password);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "FileLinkDialog";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FileLinkDialog_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textbox_password;
        private System.Windows.Forms.Label label_password;
        private System.Windows.Forms.Label label_maxDownloads;
        private System.Windows.Forms.TextBox textbox_maxDownloads;
        private System.Windows.Forms.Label label_validTill1;
        private System.Windows.Forms.TextBox textbox_validTill;
        private System.Windows.Forms.Label label_validTill2;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.Button btn_ok;
        private System.Windows.Forms.PictureBox pictureBox1;

    }
}