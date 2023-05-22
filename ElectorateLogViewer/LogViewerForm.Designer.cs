namespace ElectorateLogViewer {
    partial class LogViewerForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogViewerForm));
            this.grpOrganisation = new System.Windows.Forms.GroupBox();
            this.txtOrganisationPublicKey = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.rtbLogs = new System.Windows.Forms.RichTextBox();
            this.btnRefreshLogs = new System.Windows.Forms.Button();
            this.txtOrganisationPrivateKey = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.grpOrganisation.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpOrganisation
            // 
            this.grpOrganisation.Controls.Add(this.txtOrganisationPrivateKey);
            this.grpOrganisation.Controls.Add(this.label2);
            this.grpOrganisation.Controls.Add(this.txtOrganisationPublicKey);
            this.grpOrganisation.Controls.Add(this.label1);
            this.grpOrganisation.Location = new System.Drawing.Point(12, 12);
            this.grpOrganisation.Name = "grpOrganisation";
            this.grpOrganisation.Size = new System.Drawing.Size(560, 77);
            this.grpOrganisation.TabIndex = 0;
            this.grpOrganisation.TabStop = false;
            this.grpOrganisation.Text = "Organisation";
            // 
            // txtOrganisationPublicKey
            // 
            this.txtOrganisationPublicKey.Location = new System.Drawing.Point(69, 19);
            this.txtOrganisationPublicKey.Name = "txtOrganisationPublicKey";
            this.txtOrganisationPublicKey.Size = new System.Drawing.Size(480, 20);
            this.txtOrganisationPublicKey.TabIndex = 5;
            this.txtOrganisationPublicKey.TextChanged += new System.EventHandler(this.txtOrganisationPublicKey_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Public Key";
            // 
            // rtbLogs
            // 
            this.rtbLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbLogs.Location = new System.Drawing.Point(12, 95);
            this.rtbLogs.Name = "rtbLogs";
            this.rtbLogs.Size = new System.Drawing.Size(766, 454);
            this.rtbLogs.TabIndex = 1;
            this.rtbLogs.Text = "";
            this.rtbLogs.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.rtbLogs_LinkClicked);
            // 
            // btnRefreshLogs
            // 
            this.btnRefreshLogs.Enabled = false;
            this.btnRefreshLogs.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefreshLogs.Image = ((System.Drawing.Image)(resources.GetObject("btnRefreshLogs.Image")));
            this.btnRefreshLogs.Location = new System.Drawing.Point(578, 18);
            this.btnRefreshLogs.Name = "btnRefreshLogs";
            this.btnRefreshLogs.Size = new System.Drawing.Size(200, 71);
            this.btnRefreshLogs.TabIndex = 2;
            this.btnRefreshLogs.Text = "Refresh Logs";
            this.btnRefreshLogs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnRefreshLogs.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnRefreshLogs.UseVisualStyleBackColor = true;
            this.btnRefreshLogs.Click += new System.EventHandler(this.btnRefreshLogs_Click);
            // 
            // txtOrganisationPrivateKey
            // 
            this.txtOrganisationPrivateKey.Location = new System.Drawing.Point(69, 45);
            this.txtOrganisationPrivateKey.Name = "txtOrganisationPrivateKey";
            this.txtOrganisationPrivateKey.Size = new System.Drawing.Size(480, 20);
            this.txtOrganisationPrivateKey.TabIndex = 7;
            this.txtOrganisationPrivateKey.TextChanged += new System.EventHandler(this.txtOrganisationPrivateKey_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Private Key";
            // 
            // LogViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.btnRefreshLogs);
            this.Controls.Add(this.rtbLogs);
            this.Controls.Add(this.grpOrganisation);
            this.Name = "LogViewerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Electorate Log Viewer";
            this.grpOrganisation.ResumeLayout(false);
            this.grpOrganisation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grpOrganisation;
        private System.Windows.Forms.TextBox txtOrganisationPublicKey;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox rtbLogs;
        private System.Windows.Forms.Button btnRefreshLogs;
        private System.Windows.Forms.TextBox txtOrganisationPrivateKey;
        private System.Windows.Forms.Label label2;
    }
}

