using Electorate;
using Electorate.Blockchain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace ElectorateLogViewer {
    public partial class LogViewerForm : Form {
        //================================================================================
        private ElectorateInstance              mElectorate;

        
        //================================================================================
        //--------------------------------------------------------------------------------
        public LogViewerForm() {
            // Initialise
            InitializeComponent();

            // Electorate
            mElectorate = new ElectorateInstance("Data Source=ABCDE\\SQL2017;Initial Catalog=Voting;Integrated Security=SSPI;MultipleActiveResultSets=True",
                                                 ElectorateInstance.BlockchainType.ETHEREUM, "https://rinkeby.infura.io",
                                                 new BlockchainKey("", "5a05be103cee074c3275772136cfb1b89491d89638b92df940a956da2143ba93"));           
        }

        
        // ORGANISATION ================================================================================
        //--------------------------------------------------------------------------------
        private void txtOrganisationPublicKey_TextChanged(object sender, EventArgs e) {
            btnRefreshLogs.Enabled = (txtOrganisationPublicKey.Text.Length > 0) && (txtOrganisationPrivateKey.Text.Length > 0);
        }
        
        //--------------------------------------------------------------------------------
        private void txtOrganisationPrivateKey_TextChanged(object sender, EventArgs e) {
            btnRefreshLogs.Enabled = (txtOrganisationPublicKey.Text.Length > 0) && (txtOrganisationPrivateKey.Text.Length > 0);
        }
        

        // LOGS ================================================================================
        //--------------------------------------------------------------------------------
        private async void btnRefreshLogs_Click(object sender, EventArgs e) {
            try {
                // Organisation key
                BlockchainKey organisationKey = new BlockchainKey(txtOrganisationPublicKey.Text, txtOrganisationPrivateKey.Text);

                // Output
                string output = "";

                // Ballots
                Ballot[] ballots = mElectorate.Ballots(organisationKey);
                foreach (Ballot b in ballots) {
                    output += b.Name + " (" + b.Address.Address + ")\r\n";

                    // Logs
                    BlockchainLogEntry[] lockOptionLogs = await mElectorate.BallotLockOptionsLogs(organisationKey, b.Address);
                    BlockchainLogEntry[] addVoterLogs = await mElectorate.BallotAddVoterLogs(organisationKey, b.Address);
                    BlockchainLogEntry[] voteLogs = await mElectorate.BallotVoteLogs(organisationKey, b.Address);
                    BlockchainLogEntry[] logs = lockOptionLogs.Concat(addVoterLogs).Concat(voteLogs).ToArray();

                    // Logs - sort
                    Array.Sort(logs, delegate (BlockchainLogEntry log1, BlockchainLogEntry log2) { return log1.Data("block_number").CompareTo(log2.Data("block_number")); });

                    // Logs - output
                    foreach (BlockchainLogEntry l in logs) {
                        // Type
                        switch (l.EventName) {
                            case "OnLockOptions": output += "    Lock Options\r\n"; break;
                            case "OnAddVoter": output += "    Add Voter\r\n"; break;
                            case "OnVote": output += "    Vote\r\n"; break;
                        }

                        // Data
                        output += "        block: " + l.Data("block_number") + ", tx: " + l.Data("transaction_hash") + "\r\n";

                        if (l.EventName.Equals("OnAddVoter"))
                            output += "        voter count: " + l.Data("voter_count") + ", voter address: " + l.Data("voter_address") + "\r\n";
                        else if (l.EventName.Equals("OnVote"))
                            output += "        voter address: " + l.Data("voter_address") + ", option index: " + l.Data("option_index") + "\r\n";

                        // Link
                        output += @"        https://rinkeby.etherscan.io/tx/" + l.Data("transaction_hash") + "\r\n";
                    }
                }

                // Output
                rtbLogs.Text = output;
            }
            catch (Exception ex) {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        
        //--------------------------------------------------------------------------------
        private void rtbLogs_LinkClicked(object sender, LinkClickedEventArgs e) {
            if (!string.IsNullOrWhiteSpace(e.LinkText) && e.LinkText.StartsWith("http"))
                Process.Start(e.LinkText);
        }
    }
}
