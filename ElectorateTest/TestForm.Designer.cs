namespace ElectorateTest {
    partial class TestForm {
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
            this.btnAddContractSource = new System.Windows.Forms.Button();
            this.btnSystemBalance = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnOrganisationBalance = new System.Windows.Forms.Button();
            this.btnSwapOrganisationFunding = new System.Windows.Forms.Button();
            this.btnCreateOrganisation = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtOrganisationPrivateKey = new System.Windows.Forms.TextBox();
            this.txtOrganisationPublicKey = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnVoterBalance = new System.Windows.Forms.Button();
            this.btnVote = new System.Windows.Forms.Button();
            this.btnFundVoter = new System.Windows.Forms.Button();
            this.btnRegisterVoter = new System.Windows.Forms.Button();
            this.btnListVoters = new System.Windows.Forms.Button();
            this.btnCreateVoter = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnRemoveOption = new System.Windows.Forms.Button();
            this.btnLogVote = new System.Windows.Forms.Button();
            this.btnLogAddVoter = new System.Windows.Forms.Button();
            this.btnLogLockOptions = new System.Windows.Forms.Button();
            this.btnListBallotVoters = new System.Windows.Forms.Button();
            this.btnDeallocateBallotVoter = new System.Windows.Forms.Button();
            this.btnAllocateBallotVoter = new System.Windows.Forms.Button();
            this.btnLockOptions = new System.Windows.Forms.Button();
            this.btnAddOption = new System.Windows.Forms.Button();
            this.btnListBallots = new System.Windows.Forms.Button();
            this.btnCreateBallot = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtBallotAddress = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnListOrganisations = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddContractSource
            // 
            this.btnAddContractSource.Location = new System.Drawing.Point(6, 48);
            this.btnAddContractSource.Name = "btnAddContractSource";
            this.btnAddContractSource.Size = new System.Drawing.Size(130, 23);
            this.btnAddContractSource.TabIndex = 1;
            this.btnAddContractSource.Text = "Add Contract Source";
            this.btnAddContractSource.UseVisualStyleBackColor = true;
            this.btnAddContractSource.Click += new System.EventHandler(this.btnAddContractSource_Click);
            // 
            // btnSystemBalance
            // 
            this.btnSystemBalance.Location = new System.Drawing.Point(6, 19);
            this.btnSystemBalance.Name = "btnSystemBalance";
            this.btnSystemBalance.Size = new System.Drawing.Size(130, 23);
            this.btnSystemBalance.TabIndex = 2;
            this.btnSystemBalance.Text = "Balance";
            this.btnSystemBalance.UseVisualStyleBackColor = true;
            this.btnSystemBalance.Click += new System.EventHandler(this.btnSystemBalance_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnSystemBalance);
            this.groupBox1.Controls.Add(this.btnAddContractSource);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(377, 78);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "System";
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(12, 339);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(760, 140);
            this.txtLog.TabIndex = 5;
            this.txtLog.WordWrap = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnListOrganisations);
            this.groupBox2.Controls.Add(this.btnOrganisationBalance);
            this.groupBox2.Controls.Add(this.btnSwapOrganisationFunding);
            this.groupBox2.Controls.Add(this.btnCreateOrganisation);
            this.groupBox2.Location = new System.Drawing.Point(395, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(377, 78);
            this.groupBox2.TabIndex = 6;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Organisations";
            // 
            // btnOrganisationBalance
            // 
            this.btnOrganisationBalance.Location = new System.Drawing.Point(172, 19);
            this.btnOrganisationBalance.Name = "btnOrganisationBalance";
            this.btnOrganisationBalance.Size = new System.Drawing.Size(160, 23);
            this.btnOrganisationBalance.TabIndex = 2;
            this.btnOrganisationBalance.Text = "Organisation Balance";
            this.btnOrganisationBalance.UseVisualStyleBackColor = true;
            this.btnOrganisationBalance.Click += new System.EventHandler(this.btnOrganisationBalance_Click);
            // 
            // btnSwapOrganisationFunding
            // 
            this.btnSwapOrganisationFunding.Location = new System.Drawing.Point(172, 48);
            this.btnSwapOrganisationFunding.Name = "btnSwapOrganisationFunding";
            this.btnSwapOrganisationFunding.Size = new System.Drawing.Size(160, 23);
            this.btnSwapOrganisationFunding.TabIndex = 1;
            this.btnSwapOrganisationFunding.Text = "Swap Organisation Funding";
            this.btnSwapOrganisationFunding.UseVisualStyleBackColor = true;
            this.btnSwapOrganisationFunding.Click += new System.EventHandler(this.btnSwapOrganisationFunding_Click);
            // 
            // btnCreateOrganisation
            // 
            this.btnCreateOrganisation.Location = new System.Drawing.Point(6, 19);
            this.btnCreateOrganisation.Name = "btnCreateOrganisation";
            this.btnCreateOrganisation.Size = new System.Drawing.Size(160, 23);
            this.btnCreateOrganisation.TabIndex = 0;
            this.btnCreateOrganisation.Text = "Create Organisation";
            this.btnCreateOrganisation.UseVisualStyleBackColor = true;
            this.btnCreateOrganisation.Click += new System.EventHandler(this.btnCreateOrganisation_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Private Key";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Public Key";
            // 
            // txtOrganisationPrivateKey
            // 
            this.txtOrganisationPrivateKey.Location = new System.Drawing.Point(86, 45);
            this.txtOrganisationPrivateKey.Name = "txtOrganisationPrivateKey";
            this.txtOrganisationPrivateKey.Size = new System.Drawing.Size(667, 20);
            this.txtOrganisationPrivateKey.TabIndex = 3;
            // 
            // txtOrganisationPublicKey
            // 
            this.txtOrganisationPublicKey.Location = new System.Drawing.Point(86, 19);
            this.txtOrganisationPublicKey.Name = "txtOrganisationPublicKey";
            this.txtOrganisationPublicKey.Size = new System.Drawing.Size(667, 20);
            this.txtOrganisationPublicKey.TabIndex = 2;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnVoterBalance);
            this.groupBox3.Controls.Add(this.btnVote);
            this.groupBox3.Controls.Add(this.btnFundVoter);
            this.groupBox3.Controls.Add(this.btnRegisterVoter);
            this.groupBox3.Controls.Add(this.btnListVoters);
            this.groupBox3.Controls.Add(this.btnCreateVoter);
            this.groupBox3.Location = new System.Drawing.Point(512, 96);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(260, 112);
            this.groupBox3.TabIndex = 7;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Voters";
            // 
            // btnVoterBalance
            // 
            this.btnVoterBalance.Location = new System.Drawing.Point(133, 21);
            this.btnVoterBalance.Name = "btnVoterBalance";
            this.btnVoterBalance.Size = new System.Drawing.Size(120, 23);
            this.btnVoterBalance.TabIndex = 5;
            this.btnVoterBalance.Text = "Voter Balance";
            this.btnVoterBalance.UseVisualStyleBackColor = true;
            this.btnVoterBalance.Click += new System.EventHandler(this.btnVoterBalance_Click);
            // 
            // btnVote
            // 
            this.btnVote.Location = new System.Drawing.Point(133, 78);
            this.btnVote.Name = "btnVote";
            this.btnVote.Size = new System.Drawing.Size(120, 23);
            this.btnVote.TabIndex = 4;
            this.btnVote.Text = "Vote";
            this.btnVote.UseVisualStyleBackColor = true;
            this.btnVote.Click += new System.EventHandler(this.btnVote_Click);
            // 
            // btnFundVoter
            // 
            this.btnFundVoter.Location = new System.Drawing.Point(133, 50);
            this.btnFundVoter.Name = "btnFundVoter";
            this.btnFundVoter.Size = new System.Drawing.Size(120, 23);
            this.btnFundVoter.TabIndex = 3;
            this.btnFundVoter.Text = "Fund Voter";
            this.btnFundVoter.UseVisualStyleBackColor = true;
            this.btnFundVoter.Click += new System.EventHandler(this.btnFundVoter_Click);
            // 
            // btnRegisterVoter
            // 
            this.btnRegisterVoter.Location = new System.Drawing.Point(7, 78);
            this.btnRegisterVoter.Name = "btnRegisterVoter";
            this.btnRegisterVoter.Size = new System.Drawing.Size(120, 23);
            this.btnRegisterVoter.TabIndex = 2;
            this.btnRegisterVoter.Text = "Register To Vote";
            this.btnRegisterVoter.UseVisualStyleBackColor = true;
            this.btnRegisterVoter.Click += new System.EventHandler(this.btnRegisterVoter_Click);
            // 
            // btnListVoters
            // 
            this.btnListVoters.Location = new System.Drawing.Point(7, 50);
            this.btnListVoters.Name = "btnListVoters";
            this.btnListVoters.Size = new System.Drawing.Size(120, 23);
            this.btnListVoters.TabIndex = 1;
            this.btnListVoters.Text = "List Voters";
            this.btnListVoters.UseVisualStyleBackColor = true;
            this.btnListVoters.Click += new System.EventHandler(this.btnListVoters_Click);
            // 
            // btnCreateVoter
            // 
            this.btnCreateVoter.Location = new System.Drawing.Point(7, 20);
            this.btnCreateVoter.Name = "btnCreateVoter";
            this.btnCreateVoter.Size = new System.Drawing.Size(120, 23);
            this.btnCreateVoter.TabIndex = 0;
            this.btnCreateVoter.Text = "Create Voter";
            this.btnCreateVoter.UseVisualStyleBackColor = true;
            this.btnCreateVoter.Click += new System.EventHandler(this.btnCreateVoter_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnRemoveOption);
            this.groupBox4.Controls.Add(this.btnLogVote);
            this.groupBox4.Controls.Add(this.btnLogAddVoter);
            this.groupBox4.Controls.Add(this.btnLogLockOptions);
            this.groupBox4.Controls.Add(this.btnListBallotVoters);
            this.groupBox4.Controls.Add(this.btnDeallocateBallotVoter);
            this.groupBox4.Controls.Add(this.btnAllocateBallotVoter);
            this.groupBox4.Controls.Add(this.btnLockOptions);
            this.groupBox4.Controls.Add(this.btnAddOption);
            this.groupBox4.Controls.Add(this.btnListBallots);
            this.groupBox4.Controls.Add(this.btnCreateBallot);
            this.groupBox4.Location = new System.Drawing.Point(12, 96);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(494, 112);
            this.groupBox4.TabIndex = 8;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Ballots";
            // 
            // btnRemoveOption
            // 
            this.btnRemoveOption.Location = new System.Drawing.Point(129, 48);
            this.btnRemoveOption.Name = "btnRemoveOption";
            this.btnRemoveOption.Size = new System.Drawing.Size(116, 23);
            this.btnRemoveOption.TabIndex = 11;
            this.btnRemoveOption.Text = "Remove Option";
            this.btnRemoveOption.UseVisualStyleBackColor = true;
            this.btnRemoveOption.Click += new System.EventHandler(this.btnRemoveOption_Click);
            // 
            // btnLogVote
            // 
            this.btnLogVote.Location = new System.Drawing.Point(372, 77);
            this.btnLogVote.Name = "btnLogVote";
            this.btnLogVote.Size = new System.Drawing.Size(116, 23);
            this.btnLogVote.TabIndex = 10;
            this.btnLogVote.Text = "Log: Vote";
            this.btnLogVote.UseVisualStyleBackColor = true;
            this.btnLogVote.Click += new System.EventHandler(this.btnLogVote_Click);
            // 
            // btnLogAddVoter
            // 
            this.btnLogAddVoter.Location = new System.Drawing.Point(372, 48);
            this.btnLogAddVoter.Name = "btnLogAddVoter";
            this.btnLogAddVoter.Size = new System.Drawing.Size(116, 23);
            this.btnLogAddVoter.TabIndex = 9;
            this.btnLogAddVoter.Text = "Log: Add Voter";
            this.btnLogAddVoter.UseVisualStyleBackColor = true;
            this.btnLogAddVoter.Click += new System.EventHandler(this.btnLogAddVoter_Click);
            // 
            // btnLogLockOptions
            // 
            this.btnLogLockOptions.Location = new System.Drawing.Point(372, 19);
            this.btnLogLockOptions.Name = "btnLogLockOptions";
            this.btnLogLockOptions.Size = new System.Drawing.Size(116, 23);
            this.btnLogLockOptions.TabIndex = 8;
            this.btnLogLockOptions.Text = "Log: Lock Options";
            this.btnLogLockOptions.UseVisualStyleBackColor = true;
            this.btnLogLockOptions.Click += new System.EventHandler(this.btnLogLockOptions_Click);
            // 
            // btnListBallotVoters
            // 
            this.btnListBallotVoters.Location = new System.Drawing.Point(251, 78);
            this.btnListBallotVoters.Name = "btnListBallotVoters";
            this.btnListBallotVoters.Size = new System.Drawing.Size(115, 23);
            this.btnListBallotVoters.TabIndex = 6;
            this.btnListBallotVoters.Text = "List Allocated Voters";
            this.btnListBallotVoters.UseVisualStyleBackColor = true;
            this.btnListBallotVoters.Click += new System.EventHandler(this.btnListBallotVoters_Click);
            // 
            // btnDeallocateBallotVoter
            // 
            this.btnDeallocateBallotVoter.Location = new System.Drawing.Point(251, 48);
            this.btnDeallocateBallotVoter.Name = "btnDeallocateBallotVoter";
            this.btnDeallocateBallotVoter.Size = new System.Drawing.Size(115, 23);
            this.btnDeallocateBallotVoter.TabIndex = 5;
            this.btnDeallocateBallotVoter.Text = "Deallocate Voter";
            this.btnDeallocateBallotVoter.UseVisualStyleBackColor = true;
            this.btnDeallocateBallotVoter.Click += new System.EventHandler(this.btnDeallocateBallotVoter_Click);
            // 
            // btnAllocateBallotVoter
            // 
            this.btnAllocateBallotVoter.Location = new System.Drawing.Point(251, 19);
            this.btnAllocateBallotVoter.Name = "btnAllocateBallotVoter";
            this.btnAllocateBallotVoter.Size = new System.Drawing.Size(115, 23);
            this.btnAllocateBallotVoter.TabIndex = 4;
            this.btnAllocateBallotVoter.Text = "Allocate Voter";
            this.btnAllocateBallotVoter.UseVisualStyleBackColor = true;
            this.btnAllocateBallotVoter.Click += new System.EventHandler(this.btnAllocateBallotVoter_Click);
            // 
            // btnLockOptions
            // 
            this.btnLockOptions.Location = new System.Drawing.Point(129, 78);
            this.btnLockOptions.Name = "btnLockOptions";
            this.btnLockOptions.Size = new System.Drawing.Size(116, 23);
            this.btnLockOptions.TabIndex = 3;
            this.btnLockOptions.Text = "Lock Options";
            this.btnLockOptions.UseVisualStyleBackColor = true;
            this.btnLockOptions.Click += new System.EventHandler(this.btnLockOptions_Click);
            // 
            // btnAddOption
            // 
            this.btnAddOption.Location = new System.Drawing.Point(129, 19);
            this.btnAddOption.Name = "btnAddOption";
            this.btnAddOption.Size = new System.Drawing.Size(116, 23);
            this.btnAddOption.TabIndex = 2;
            this.btnAddOption.Text = "Add Option";
            this.btnAddOption.UseVisualStyleBackColor = true;
            this.btnAddOption.Click += new System.EventHandler(this.btnAddOption_Click);
            // 
            // btnListBallots
            // 
            this.btnListBallots.Location = new System.Drawing.Point(7, 48);
            this.btnListBallots.Name = "btnListBallots";
            this.btnListBallots.Size = new System.Drawing.Size(116, 23);
            this.btnListBallots.TabIndex = 1;
            this.btnListBallots.Text = "List Ballots";
            this.btnListBallots.UseVisualStyleBackColor = true;
            this.btnListBallots.Click += new System.EventHandler(this.btnListBallots_Click);
            // 
            // btnCreateBallot
            // 
            this.btnCreateBallot.Location = new System.Drawing.Point(7, 19);
            this.btnCreateBallot.Name = "btnCreateBallot";
            this.btnCreateBallot.Size = new System.Drawing.Size(116, 23);
            this.btnCreateBallot.TabIndex = 0;
            this.btnCreateBallot.Text = "Create Ballot";
            this.btnCreateBallot.UseVisualStyleBackColor = true;
            this.btnCreateBallot.Click += new System.EventHandler(this.btnCreateBallot_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txtBallotAddress);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.txtOrganisationPublicKey);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.txtOrganisationPrivateKey);
            this.groupBox5.Location = new System.Drawing.Point(12, 214);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(760, 105);
            this.groupBox5.TabIndex = 9;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Inputs";
            // 
            // txtBallotAddress
            // 
            this.txtBallotAddress.Location = new System.Drawing.Point(86, 72);
            this.txtBallotAddress.Name = "txtBallotAddress";
            this.txtBallotAddress.Size = new System.Drawing.Size(667, 20);
            this.txtBallotAddress.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 75);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Ballot Address";
            // 
            // btnListOrganisations
            // 
            this.btnListOrganisations.Location = new System.Drawing.Point(6, 48);
            this.btnListOrganisations.Name = "btnListOrganisations";
            this.btnListOrganisations.Size = new System.Drawing.Size(160, 23);
            this.btnListOrganisations.TabIndex = 3;
            this.btnListOrganisations.Text = "List Organisations";
            this.btnListOrganisations.UseVisualStyleBackColor = true;
            this.btnListOrganisations.Click += new System.EventHandler(this.btnListOrganisations_Click);
            // 
            // TestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 491);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.groupBox1);
            this.MaximumSize = new System.Drawing.Size(800, 529);
            this.MinimumSize = new System.Drawing.Size(800, 529);
            this.Name = "TestForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Electorate";
            this.Shown += new System.EventHandler(this.TestForm_Shown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnAddContractSource;
        private System.Windows.Forms.Button btnSystemBalance;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnCreateOrganisation;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnCreateVoter;
        private System.Windows.Forms.Button btnListVoters;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btnCreateBallot;
        private System.Windows.Forms.Button btnSwapOrganisationFunding;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtOrganisationPrivateKey;
        private System.Windows.Forms.TextBox txtOrganisationPublicKey;
        private System.Windows.Forms.Button btnListBallots;
        private System.Windows.Forms.Button btnAddOption;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox txtBallotAddress;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnLockOptions;
        private System.Windows.Forms.Button btnAllocateBallotVoter;
        private System.Windows.Forms.Button btnDeallocateBallotVoter;
        private System.Windows.Forms.Button btnListBallotVoters;
        private System.Windows.Forms.Button btnRegisterVoter;
        private System.Windows.Forms.Button btnVote;
        private System.Windows.Forms.Button btnFundVoter;
        private System.Windows.Forms.Button btnVoterBalance;
        private System.Windows.Forms.Button btnOrganisationBalance;
        private System.Windows.Forms.Button btnLogVote;
        private System.Windows.Forms.Button btnLogAddVoter;
        private System.Windows.Forms.Button btnLogLockOptions;
        private System.Windows.Forms.Button btnRemoveOption;
        private System.Windows.Forms.Button btnListOrganisations;
    }
}

