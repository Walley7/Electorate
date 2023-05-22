using Electorate;
using Electorate.Blockchain;
using Electorate.Blockchain.Ethereum;
using Electorate.Exceptions;
using Microsoft.VisualBasic;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using SolcNet.CompileErrors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace ElectorateTest {

    public partial class TestForm : Form {
        //================================================================================
        private ElectorateInstance              mElectorate;


        //================================================================================
        //--------------------------------------------------------------------------------
        public TestForm() {
            InitializeComponent();

            // Electorate
            //mElectorate = new ElectorateInstance("Data Source=ABCDE\\SQL2017;Initial Catalog=Voting;Integrated Security=SSPI;MultipleActiveResultSets=True",
            //                                     ElectorateInstance.BlockchainType.ETHEREUM, "http://localhost:7545",
            //                                     new BlockchainKey("", "40402e53ef9e7c155073d56b16978afcb4269e309c14397af162929cf52bf62e"));
            //mElectorate = new ElectorateInstance("Data Source=ABCDE\\SQL2017;Initial Catalog=Voting;Integrated Security=SSPI;MultipleActiveResultSets=True",
            //                                     ElectorateInstance.BlockchainType.ETHEREUM, "https://rinkeby.infura.io",
            //                                     new BlockchainKey("", "5a05be103cee074c3275772136cfb1b89491d89638b92df940a956da2143ba93"));           
            //mElectorate = new ElectorateInstance("Data Source=CSILT21\\SALESLOGIX2012;Initial Catalog=Electorate;User ID=sysdba;Password=masterkey;Persist Security Info=True;Integrated Security=SSPI;MultipleActiveResultSets=True",
            //                                     ElectorateInstance.BlockchainType.ETHEREUM, "http://192.168.241.129:7545",
            //                                     new BlockchainKey("", "f3f9036fd91ef36ea82809fb5e53e597ea14c6e8967eeb8744ee803c3816e7c0"));           
            //mElectorate = new ElectorateInstance("Data Source=CSILT21\\SALESLOGIX2012;Initial Catalog=Electorate;User ID=sysdba;Password=masterkey;Persist Security Info=True;Integrated Security=SSPI;MultipleActiveResultSets=True",
            //                                     ElectorateInstance.BlockchainType.ETHEREUM, "https://rinkeby.infura.io",
            //                                     new BlockchainKey("", "5a05be103cee074c3275772136cfb1b89491d89638b92df940a956da2143ba93"));
            mElectorate = new ElectorateInstance("Data Source=LAPTOP-B67691TE\\SQLEXPRESS;Initial Catalog=Electorate1;User ID=sa;Password=sa;Persist Security Info=True;Integrated Security=SSPI;MultipleActiveResultSets=True",
                                                 ElectorateInstance.BlockchainType.ETHEREUM, "https://rinkeby.infura.io",
                                                 new BlockchainKey("", "088A1428F9FA7FE1A016EB9802E8E20634737147C5F7F851346D83015FF13446"));
        }
        
        //--------------------------------------------------------------------------------
        private void TestForm_Shown(object sender, EventArgs e) {
            txtOrganisationPublicKey.Text = "044428FA447CA5568984216FCBE8CC1235E89650B28F1393F48B1BB41F3ECDAE3A0D5D49844AFFE8B3A17E842822BD4F5DC146173922B2205F84B517E9B65683D6";
            txtOrganisationPrivateKey.Text = "0774262E5CE695CC5E217A9B95D16EC8AEA97831F9AB7FD8F83004657B2C4FB2";
            txtBallotAddress.Text = "0x5e8134a9c943c0fe68aa8ce99f9dff2d69a4ef46";
            //txtOrganisationPublicKey.Text = "04384D770BBC64756D02D1927411295325A272F687F69DBA080289EDBC52C9956384B09EA592A6A17621249BC43BF05BD48A7498BA9F234A716299DD66182C20E8";
            //txtOrganisationPrivateKey.Text = "5773BA2114AED8FF39C59FB92289FF2DBAE79CFBE5EAD0E12A118A965C307C33";
        }

        
        // SYSTEM ================================================================================
        //--------------------------------------------------------------------------------
        private async void btnSystemBalance_Click(object sender, EventArgs e) {
            decimal balance = await mElectorate.SystemBalance();
            txtLog.Text = "SYSTEM BALANCE: " + balance + "\r\n" + txtLog.Text;
        }
        
        //--------------------------------------------------------------------------------
        private void btnAddContractSource_Click(object sender, EventArgs e) {
            // Add
            string output = "ADD CONTRACT SOURCE: ";
            try {
                // Compile / add
                string[] warnings;
                mElectorate.AddEthereumContractSource("Ethereum", "Ballot.sol", "Ballot", "0.6", out warnings);
                
                // Contract
                EthereumContractSource contractSource = mElectorate.EthereumContractSource("Ballot", "0.6");
                output += "'" + contractSource.Contract + "', '" + contractSource.Version + "'"; // contractSource.ABI, contractSource.ByteCode

                // Warnings
                foreach (string w in warnings) {
                    output += "\r\n" + w;
                }
            }
            catch (CompilerException ex) { output += ex.Message; }
            catch (SqlException ex) { output += "Database exception: " + ex.Message + "."; }

            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }
        
        
        // ORGANISATIONS ================================================================================
        //--------------------------------------------------------------------------------
        private void btnCreateOrganisation_Click(object sender, EventArgs e) {
            string output = "CREATE ORGANISATION: ";
            try {
                BlockchainKey organisationKey = mElectorate.CreateOrganisation(Organisation.FundingOption.SystemFunded);
                Organisation organisation = mElectorate.Organisation(organisationKey, true);
                output += "'" + organisation.Key.PublicString + "', '" + organisation.Key.PrivateString + "', '" + organisation.Funding + "'";
            }
            catch (SqlException ex) { output += "Database exception: " + ex.Message + "."; }
            
            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }
        
        //--------------------------------------------------------------------------------
        private void btnListOrganisations_Click(object sender, EventArgs e) {
            // Organisations
            string output = "LIST ORGANISATIONS: ";
            try {
                Organisation[] organisations = mElectorate.Organisations();
                foreach (Organisation o in organisations) {
                    output = output + "\r\n'" + o.Key.PublicString + "', '" + o.Key.PrivateString + "', '" + o.Funding + "'";
                }
            }
            catch (SqlException ex) { output += "Database exception: " + ex.Message + "."; }

            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }
        
        //--------------------------------------------------------------------------------
        private async void btnOrganisationBalance_Click(object sender, EventArgs e) {
            // Organisation key
            BlockchainKey organisationKey = InputOrganisationKey();
            if ((organisationKey == null) || (organisationKey.Public == null))
                return;

            // Balance
            string output = "ORGANISATION BALANCE: ";
            try {
                decimal balance = await mElectorate.OrganisationBalance(organisationKey);
                output += balance;
            }
            catch (NotFoundException ex) { output += ex.Message + "."; }
            catch (UnauthorizedAccessException) { output += "Invalid organisation private key."; }

            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }
        
        //--------------------------------------------------------------------------------
        private void btnSwapOrganisationFunding_Click(object sender, EventArgs e) {
            // Organisation key
            BlockchainKey organisationKey = InputOrganisationKey();
            if ((organisationKey == null) || (organisationKey.Public == null))
                return;

            // Swap
            string output = "SWAP ORGANISATION FUNDING: ";
            try {
                Organisation organisation = mElectorate.Organisation(organisationKey, true);
                if (organisation.Funding == Organisation.FundingOption.SystemFunded)
                    organisation.Funding = Organisation.FundingOption.SelfFunded;
                else
                    organisation.Funding = Organisation.FundingOption.SystemFunded;
                organisation.SaveChanges();
                output += "'" + organisation.Funding + "'";
            }
            catch (NotFoundException ex) { output += ex.Message + "."; }
            catch (SqlException ex) { output += "Database exception: " + ex.Message + "."; }
            
            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }
        

        // BALLOTS ================================================================================
        //--------------------------------------------------------------------------------
        private async void btnCreateBallot_Click(object sender, EventArgs e) {
            // Organisation key
            BlockchainKey organisationKey = InputOrganisationKey();
            if ((organisationKey == null) || (organisationKey.Public == null))
                return;

            // Create
            string output = "CREATE BALLOT: ";
            try {
                BlockchainAddress address = await mElectorate.CreateBallot(organisationKey, "Ballot", "0.6", "Test Ballot", DateTime.UtcNow.AddDays(7.0));
                output += "'" + address.Address + "'";
            }
            catch (NotFoundException ex) { output += ex.Message + "."; }
            catch (UnauthorizedAccessException) { output += "Invalid organisation private key."; }
            catch (BlockchainException ex) { output += ex.Message + "."; }
            catch (SqlException ex) { output += "Database exception: " + ex.Message + "."; }
            
            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }
        
        //--------------------------------------------------------------------------------
        private async void btnListBallots_Click(object sender, EventArgs e) {
            // Organisation key
            BlockchainKey organisationKey = InputOrganisationKey();
            if ((organisationKey == null) || (organisationKey.Public == null))
                return;

            // Ballots
            string output = "LIST BALLOTS: ";
            try {
                Ballot[] ballots = mElectorate.Ballots(organisationKey);
                foreach (Ballot b in ballots) {
                    output += "\r\n'" + b.Address.Address + "', '" + b.Name + "', '" + (await b.Version()) + "', '" + (await b.OwnerAddress()).Address + "', '" + (await b.EndTime()) + "', '" + (await b.OptionsLocked()) + "'";

                    for (uint i = 0; i < await b.OptionsCount(); ++i) {
                        BallotOption option = await b.Option(i);
                        output += "\r\n  " + (i + 1) + ". " + option.Name + ": " + (await option.Votes());
                    }
                }
            }
            catch (NotFoundException ex) { output += ex.Message + "."; }
            catch (SqlException ex) { output += "Database exception: " + ex.Message + "."; }
            
            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }
        
        //--------------------------------------------------------------------------------
        private async void btnAddOption_Click(object sender, EventArgs e) {
            // Organisation key
            BlockchainKey organisationKey = InputOrganisationKey();
            if ((organisationKey == null) || (organisationKey.Public == null))
                return;

            // Ballot address
            if (string.IsNullOrEmpty(txtBallotAddress.Text))
                return;
            BlockchainAddress ballotAddress = new BlockchainAddress(txtBallotAddress.Text);

            // Option name
            string optionName = Interaction.InputBox("Option name", "Enter an option name", "");
            if (string.IsNullOrEmpty(optionName))
                return;

            // Add
            string output = "ADD OPTION: ";
            try {
                var result = await mElectorate.AddBallotOption(organisationKey, ballotAddress, optionName); // Alternative: await mElectorate.Ballot(organisationKey, ballotAddress).AddOption(optionName);
                output += result.hash;
            }
            catch (UnauthorizedAccessException) { output += "Unauthorised account or invalid private key."; }
            catch (BlockchainException) { output += "Options are locked."; }

            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }
        
        //--------------------------------------------------------------------------------
        private void btnRemoveOption_Click(object sender, EventArgs e) {
            // Organisation key
            BlockchainKey organisationKey = InputOrganisationKey();
            if ((organisationKey == null) || (organisationKey.Public == null))
                return;

            // Ballot address
            if (string.IsNullOrEmpty(txtBallotAddress.Text))
                return;
            BlockchainAddress ballotAddress = new BlockchainAddress(txtBallotAddress.Text);
        }
        
        //--------------------------------------------------------------------------------
        private async void btnLockOptions_Click(object sender, EventArgs e) {
            // Organisation key
            BlockchainKey organisationKey = InputOrganisationKey();
            if ((organisationKey == null) || (organisationKey.Public == null))
                return;

            // Ballot address
            if (string.IsNullOrEmpty(txtBallotAddress.Text))
                return;
            BlockchainAddress ballotAddress = new BlockchainAddress(txtBallotAddress.Text);

            // Lock
            string output = "LOCK OPTIONS: ";
            try {
                var result = await mElectorate.LockBallotOptions(organisationKey, ballotAddress);
                output += result.hash;
            }
            catch (UnauthorizedAccessException) { output += "Unauthorised account or invalid private key."; }

            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }
        
        //--------------------------------------------------------------------------------
        private void btnAllocateBallotVoter_Click(object sender, EventArgs e) {
            // Organisation key
            BlockchainKey organisationKey = InputOrganisationKey();
            if ((organisationKey == null) || (organisationKey.Public == null))
                return;

            // Ballot address
            if (string.IsNullOrEmpty(txtBallotAddress.Text))
                return;
            BlockchainAddress ballotAddress = new BlockchainAddress(txtBallotAddress.Text);

            // Voter ID
            int voterID = -1;
            while (voterID == -1) {
                string voterIDString = Interaction.InputBox("Voter ID", "Enter a voter ID", "");
                if (string.IsNullOrEmpty(voterIDString))
                    return;
                if (!int.TryParse(voterIDString, out voterID))
                    voterID = -1;
            }

            // Allocate
            string output = "ALLOCATE VOTER: ";
            try {
                mElectorate.AllocateBallotVoter(organisationKey, ballotAddress, voterID);
                output += "'" + ballotAddress.Address + "', " + voterID;
            }
            catch (NotFoundException ex) { output += ex.Message + "."; }
            catch (UnauthorizedAccessException) { output += "Invalid organisation private key."; }
            catch (SqlException ex) { output += "Database exception: " + ex.Message + "."; }

            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }
        
        //--------------------------------------------------------------------------------
        private void btnDeallocateBallotVoter_Click(object sender, EventArgs e) {
            // Organisation key
            BlockchainKey organisationKey = InputOrganisationKey();
            if ((organisationKey == null) || (organisationKey.Public == null))
                return;

            // Ballot address
            if (string.IsNullOrEmpty(txtBallotAddress.Text))
                return;
            BlockchainAddress ballotAddress = new BlockchainAddress(txtBallotAddress.Text);

            // Voter ID
            int voterID;
            if (!InputVoterID(out voterID))
                return;

            // Deallocate
            string output = "DEALLOCATE VOTER: ";
            try {
                if (mElectorate.DeallocateBallotVoter(organisationKey, ballotAddress, voterID))
                    output += "'" + ballotAddress.Address + "', " + voterID;
                else
                    output += "Voter not found.";
            }
            catch (NotFoundException ex) { output += ex.Message + "."; }
            catch (UnauthorizedAccessException) { output += "Invalid organisation private key."; }
            catch (SqlException ex) { output += "Database exception: " + ex.Message + "."; }

            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }
        
        //--------------------------------------------------------------------------------
        private void btnListBallotVoters_Click(object sender, EventArgs e) {
            // Organisation key
            BlockchainKey organisationKey = InputOrganisationKey();
            if ((organisationKey == null) || (organisationKey.Public == null))
                return;

            // Ballot address
            if (string.IsNullOrEmpty(txtBallotAddress.Text))
                return;
            BlockchainAddress ballotAddress = new BlockchainAddress(txtBallotAddress.Text);

            // Voters
            string output = "LIST ALLOCATED VOTERS: ";
            try {
                int[] voters = mElectorate.BallotAllocatedVoters(organisationKey, ballotAddress);
                foreach (int v in voters) {
                    Voter voter = mElectorate.Voter(v, true);
                    output += "\r\n" + voter.ID + ", '" + voter.Name + "'";
                }
            }
            catch (NotFoundException ex) { output += ex.Message + "."; }
            catch (SqlException ex) { output += "Database exception: " + ex.Message + "."; }

            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }
        
        //--------------------------------------------------------------------------------
        private async void btnLogLockOptions_Click(object sender, EventArgs e) {
            // Organisation key
            BlockchainKey organisationKey = InputOrganisationKey();
            if ((organisationKey == null) || (organisationKey.Public == null))
                return;

            // Ballot address
            if (string.IsNullOrEmpty(txtBallotAddress.Text))
                return;
            BlockchainAddress ballotAddress = new BlockchainAddress(txtBallotAddress.Text);

            // Logs
            string output = "LOG - LOCK OPTIONS: ";
            try {
                BlockchainLogEntry[] logEntries = await mElectorate.BallotLockOptionsLogs(organisationKey, ballotAddress);
                foreach (BlockchainLogEntry l in logEntries) {
                    output += "\r\n" + l.Data("block_number") + ", " + l.Data("transaction_hash");
                }
            }
            catch (NotFoundException ex) { output += ex.Message + "."; }
            catch (UnauthorizedAccessException) { output += "Invalid organisation private key."; }
            catch (BlockchainException ex) { output += ex.Message + "."; }
            
            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }
        
        //--------------------------------------------------------------------------------
        private async void btnLogAddVoter_Click(object sender, EventArgs e) {
            // Organisation key
            BlockchainKey organisationKey = InputOrganisationKey();
            if ((organisationKey == null) || (organisationKey.Public == null))
                return;

            // Ballot address
            if (string.IsNullOrEmpty(txtBallotAddress.Text))
                return;
            BlockchainAddress ballotAddress = new BlockchainAddress(txtBallotAddress.Text);

            // Logs
            string output = "LOG - ADD VOTER: ";
            try {
                BlockchainLogEntry[] logEntries = await mElectorate.BallotAddVoterLogs(organisationKey, ballotAddress);
                foreach (BlockchainLogEntry l in logEntries) {
                    output += "\r\n" + l.Data("block_number") + ", " + l.Data("transaction_hash") + ", " + l.Data("voter_count") + ", " + l.Data("voter_address");
                }
            }
            catch (NotFoundException ex) { output += ex.Message + "."; }
            catch (UnauthorizedAccessException) { output += "Invalid organisation private key."; }
            catch (BlockchainException ex) { output += ex.Message + "."; }
            
            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }
        
        //--------------------------------------------------------------------------------
        private async void btnLogVote_Click(object sender, EventArgs e) {
            // Organisation key
            BlockchainKey organisationKey = InputOrganisationKey();
            if ((organisationKey == null) || (organisationKey.Public == null))
                return;

            // Ballot address
            if (string.IsNullOrEmpty(txtBallotAddress.Text))
                return;
            BlockchainAddress ballotAddress = new BlockchainAddress(txtBallotAddress.Text);

            // Logs
            string output = "LOG - VOTE: ";
            try {
                BlockchainLogEntry[] logEntries = await mElectorate.BallotVoteLogs(organisationKey, ballotAddress);
                foreach (BlockchainLogEntry l in logEntries) {
                    output += "\r\n" + l.Data("block_number") + ", " + l.Data("transaction_hash") + ", " + l.Data("voter_address") + ", " + l.Data("option_index");
                }
            }
            catch (NotFoundException ex) { output += ex.Message + "."; }
            catch (UnauthorizedAccessException) { output += "Invalid organisation private key."; }
            catch (BlockchainException ex) { output += ex.Message + "."; }
            
            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }


        // VOTERS ================================================================================
        //--------------------------------------------------------------------------------
        private void btnCreateVoter_Click(object sender, EventArgs e) {
            // Organisation key
            BlockchainKey organisationKey = InputOrganisationKey();
            if ((organisationKey == null) || (organisationKey.Public == null))
                return;

            // Create
            string output = "CREATE VOTER: ";
            try {
                int voterID = mElectorate.CreateVoter(organisationKey, "Voter #" + new Random().Next());
                Voter voter = mElectorate.Voter(voterID, true);
                output += voter.ID + ", '" + voter.Name + "'";
            }
            catch (NotFoundException ex) { output += ex.Message + "."; }
            catch (UnauthorizedAccessException) { output += "Invalid organisation private key."; }
            catch (SqlException ex) { output += "Database exception: " + ex.Message + "."; }
            
            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }
        
        //--------------------------------------------------------------------------------
        private void btnListVoters_Click(object sender, EventArgs e) {
            // Organisation key
            BlockchainKey organisationKey = InputOrganisationKey();
            if ((organisationKey == null) || (organisationKey.Public == null))
                return;

            // Voters
            string output = "LIST VOTERS: ";
            try {
                Voter[] voters = mElectorate.Voters(organisationKey);
                foreach (Voter v in voters) {
                    output = output + "\r\n" + v.ID + ", '" + v.Name + "'";
                }
            }
            catch (NotFoundException ex) { output += ex.Message + "."; }
            catch (SqlException ex) { output += "Database exception: " + ex.Message + "."; }

            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }
        
        //--------------------------------------------------------------------------------
        // IMPORTANT:
        // The registration process supports a divide between the part of the
        // system which manages organisations, ballots, and voter IDs, from the part of
        // the system which manages voter blockchain addresses and the ability to login
        // and vote.
        // The purpose of such a separate is to make sure that the organisation/ballot
        // side of the system does not know which voter IDs are connected with which voter
        // blockchain addresses, making anonymity more difficult to break from one point
        // of attack.
        // For our MVP system however, there's not a great need to setup that divide, so
        // the code below just does it all on one side, with comments and functions named
        // Client_# and Server_# to represent what would happen on voter and ballot sides
        // respectively (calls between the two would be done across the network).
        private async void btnRegisterVoter_Click(object sender, EventArgs e) {
            // Ballot address
            if (string.IsNullOrEmpty(txtBallotAddress.Text))
                return;
            BlockchainAddress ballotAddress = new BlockchainAddress(txtBallotAddress.Text);

            // Voter ID
            int voterID;
            if (!InputVoterID(out voterID))
                return;

            // Register
            string output = "REGISTER VOTER: ";
            try {
                // Voter side checks to see if registration has already occurred (including incomplete registration), and makes the appropriate
                // call to the server to resume the process where it left off.
                VoterToken voterToken = mElectorate.VoterToken(voterID, ballotAddress);
                VoterAccount voterAccount = mElectorate.VoterAccount(voterID, ballotAddress);

                if ((voterAccount == null) && (voterToken == null)) {
                    output += "Newly registered - ";
                    await Server_1_ProvideBallotPublicRSAKey(voterID, ballotAddress); // Voter side requests ballot's public RSA key from the ballot side.
                }
                else if (voterAccount == null) {
                    output += "Resumed from sign voter token - ";
                    await Server_2_SignVoterToken(voterID, ballotAddress, voterToken.BlindedToken);
                }
                else if (!voterAccount.Registered) {
                    output += "Resumed from register ballot voter - ";
                    await Server_3_RegisterBallotVoter(voterID, voterAccount.Address, ballotAddress, voterToken.Token, voterToken.SignedToken);
                }
                else
                    output += "Already registered - ";

                // Result
                voterAccount = mElectorate.VoterAccount(voterID, ballotAddress);
                output += "'" + voterAccount.Address.Address + "'";
            }
            catch (NotFoundException ex) { output += ex.Message + "."; }
            catch (UnauthorizedAccessException) { output += "Invalid organisation private key."; }
            catch (InvalidSignatureException) { output += "Invalid voter token signature.";  }
            catch (NotAllocatedException ex) { output += ex.Message + "."; }
            catch (DuplicateException ex) { output += ex.Message + "."; }
            catch (BlockchainException ex) { output += ex.Message + "."; }
            catch (SqlException ex) { output += "Database exception: " + ex.Message + "."; }

            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }

        //--------------------------------------------------------------------------------
        // Ballot side returns ballot's public RSA key across the network, at request of
        // the voter side.
        //--------------------------------------------------------------------------------
        private async Task<bool> Server_1_ProvideBallotPublicRSAKey(int voterID, BlockchainAddress ballotAddress) {
            // Server
            Voter voter = mElectorate.Voter(voterID);
            Ballot ballot = mElectorate.Ballot(voter.OrganisationKey, ballotAddress, true);
            byte[] rsaPublicKeyModulus = ((RsaKeyParameters)ballot.RSAKey.Public).Modulus.ToByteArray();
            byte[] rsaPublicKeyExponent = ((RsaKeyParameters)ballot.RSAKey.Public).Exponent.ToByteArray();

            // Client
            await Client_2_CreateVoterToken(voterID, ballotAddress, rsaPublicKeyModulus, rsaPublicKeyExponent);
            return true;
        }

        //--------------------------------------------------------------------------------
        // Voter side receives and reconstructs the public RSA key (make sure to use
        // Org.BouncyCastle.Math.BigInteger), creates a token to associate with this
        // voter/ballot pair, then sends a request for it to be signed.
        //--------------------------------------------------------------------------------
        private async Task<bool> Client_2_CreateVoterToken(int voterID, BlockchainAddress ballotAddress, byte[] rsaPublicKeyModulus, byte[] rsaPublicKeyExponent) {
            // Client
            VoterToken token = mElectorate.CreateVoterToken(voterID, ballotAddress, rsaPublicKeyModulus, rsaPublicKeyExponent);

            // Server
            await Server_2_SignVoterToken(voterID, ballotAddress, token.BlindedToken);
            return true;
        }

        //--------------------------------------------------------------------------------
        // Ballot side receives and signs the blinded token (*if* the voter is allocated
        // to the ballot and not already registered), then sends the signed blinded token
        // back.
        //--------------------------------------------------------------------------------
        private async Task<bool> Server_2_SignVoterToken(int voterID, BlockchainAddress ballotAddress, byte[] blindedToken) {
            // Server
            byte[] signedBlindedToken = mElectorate.SignVoterToken(ballotAddress, voterID, blindedToken);

            // Client
            await Client_3_CreateVoterAccount(voterID, ballotAddress, signedBlindedToken);
            return true;
        }

        //--------------------------------------------------------------------------------
        // Voter side verifies the signed blinded token and creates a new blockchain
        // account to vote with, then sends a request to register this address as a voting
        // address with the ballot.
        //--------------------------------------------------------------------------------
        private async Task<bool> Client_3_CreateVoterAccount(int voterID, BlockchainAddress ballotAddress, byte[] signedBlindedToken) {
            // Client
            VoterAccount voterAccount = mElectorate.CreateVoterAccount(voterID, ballotAddress, signedBlindedToken);
            VoterToken voterToken = mElectorate.VoterToken(voterID, ballotAddress);

            // Server
            await Server_3_RegisterBallotVoter(voterID, voterAccount.Address, ballotAddress, voterToken.Token, voterToken.SignedToken);
            return true;
        }

        //--------------------------------------------------------------------------------
        // Ballot side attempts to register the voter address with the ballot's blockchain
        // contract, making it eligible to vote - if this succeeds the ballot side informs
        // the voter side that it is now able to vote in this ballot.
        //--------------------------------------------------------------------------------
        private async Task<bool> Server_3_RegisterBallotVoter(int voterID, BlockchainAddress voterAddress, BlockchainAddress ballotAddress, byte[] token, byte[] signedToken) {
            // Server
            BlockchainContract.InvokeResult result = await mElectorate.RegisterBallotVoter(ballotAddress, voterID, voterAddress, token, signedToken);

            // Client
            Client_4_MarkVoterAccountAsRegistered(voterID, ballotAddress);
            return true;
        }

        //--------------------------------------------------------------------------------
        // Voter side marks the voter account as registered.
        //--------------------------------------------------------------------------------
        private void Client_4_MarkVoterAccountAsRegistered(int voterID, BlockchainAddress ballotAddress) {
            // Client
            VoterAccount voterAccount = mElectorate.VoterAccount(voterID, ballotAddress);
            if (voterAccount == null)
                throw new NotFoundException("Voter not found");
            voterAccount.Registered = true;
            voterAccount.SaveChanges();
        }
        
        //--------------------------------------------------------------------------------
        private async void btnVoterBalance_Click(object sender, EventArgs e) {
            // Ballot address
            if (string.IsNullOrEmpty(txtBallotAddress.Text))
                return;
            BlockchainAddress ballotAddress = new BlockchainAddress(txtBallotAddress.Text);

            // Voter ID
            int voterID;
            if (!InputVoterID(out voterID))
                return;

            // Balance
            string output = "VOTER BALANCE: ";
            try {
                // Voter
                Voter voter = mElectorate.Voter(voterID);
                VoterAccount voterAccount = mElectorate.VoterAccount(voterID, ballotAddress);

                // Funds
                if (voterAccount != null) {
                    decimal balance = await mElectorate.Blockchain.Balance(voterAccount.Key);
                    output += balance;
                }
                else
                    output += "Voter isn't registered with this ballot.";
            }
            catch (NotFoundException ex) { output += ex.Message + "."; }

            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }

        //--------------------------------------------------------------------------------
        // If the system were split as explained above, this process would also have to be
        // split - the voter would have to make a request to the system that they receive
        // funding from the organisation (with their voter ID, and the ballot address and
        // voting address for one of their registrations).
        // Separate credentials might have to be provided to prove the trustworthiness of
        // the voter in this request, or the voter's session could be implicitly trusted.
        //--------------------------------------------------------------------------------
        private async void btnFundVoter_Click(object sender, EventArgs e) {
            // Organisation key
            BlockchainKey organisationKey = InputOrganisationKey();
            if ((organisationKey == null) || (organisationKey.Public == null))
                return;

            // Ballot address
            if (string.IsNullOrEmpty(txtBallotAddress.Text))
                return;
            BlockchainAddress ballotAddress = new BlockchainAddress(txtBallotAddress.Text);

            // Voter ID
            int voterID;
            if (!InputVoterID(out voterID))
                return;

            // Amount
            decimal amount;
            if (!InputAmount(out amount))
                return;

            // Fund
            string output = "FUND VOTER: ";
            try {
                VoterAccount voterAccount = mElectorate.VoterAccount(voterID, ballotAddress);
                if (voterAccount != null) {
                    string receipt = await mElectorate.Blockchain.SendFunds(organisationKey, voterAccount.Address, amount);
                    output += "'" + receipt + "'";
                }
                else
                    output += "Voter isn't registered with this ballot.";
            }
            catch (NotFoundException ex) { output += ex.Message + "."; }
            catch (BlockchainException ex) { output += ex.Message + "."; }
            catch (SqlException ex) { output += "Database exception: " + ex.Message + "."; }

            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }
        
        //--------------------------------------------------------------------------------
        private async void btnVote_Click(object sender, EventArgs e) {
            // Voter ID
            int voterID;
            if (!InputVoterID(out voterID))
                return;

            // Ballot address
            if (string.IsNullOrEmpty(txtBallotAddress.Text))
                return;
            BlockchainAddress ballotAddress = new BlockchainAddress(txtBallotAddress.Text);

            // Vote
            string output = "VOTE: ";
            try {
                // Message
                string message = "Please vote for one of the following options:";

                // Voter / ballot
                VoterAccount voterAccount = mElectorate.VoterAccount(voterID, ballotAddress);
                Ballot ballot = mElectorate.Ballot(voterAccount.Key, ballotAddress);

                // Options
                uint optionsCount = await ballot.OptionsCount();
                for (uint i = 0; i < optionsCount; ++i) {
                    message += "\r\n  " + (i + 1) + ". " + (await ballot.Option(i)).Name;
                }

                // Vote
                int vote;
                if (!InputVote(out vote, (int)optionsCount, message))
                    return;
                BlockchainContract.InvokeResult result = await mElectorate.Vote(ballotAddress, voterAccount.Key, (uint)(vote - 1)); // Option indexes start from 0
                output += "'" + result.hash + "'";
            }
            catch (NotFoundException ex) { output += ex.Message + "."; }
            catch (BlockchainException ex) { output += ex.Message + "."; }
            catch (SqlException ex) { output += "Database exception: " + ex.Message + "."; }

            // Log
            txtLog.Text = output + "\r\n" + txtLog.Text;
        }


        // UI ================================================================================
        //--------------------------------------------------------------------------------
        private BlockchainKey InputKey(string entityType, bool publicOnly = false) {
            // Public key
            string publicKey = Interaction.InputBox("Public key", "Enter an " + entityType + " public key", "");
            if (string.IsNullOrEmpty(publicKey))
                return null;

            // Private key
            string privateKey = null;
            if (!publicOnly) {
                privateKey = Interaction.InputBox("Private key", "Enter an " + entityType + " private key", "");
                if (string.IsNullOrEmpty(privateKey))
                    return null;
            }

            // Key
            BlockchainKey key = new BlockchainKey(publicKey, privateKey);
            return (!key.IsNull ? key : null);
        }

        //--------------------------------------------------------------------------------
        private BlockchainKey InputOrganisationKey(bool publicOnly = false) {
            if (!string.IsNullOrEmpty(txtOrganisationPublicKey.Text) && (publicOnly || !string.IsNullOrEmpty(txtOrganisationPrivateKey.Text))) {
                BlockchainKey key = new BlockchainKey(txtOrganisationPublicKey.Text, (!publicOnly ? txtOrganisationPrivateKey.Text : null));
                return (!key.IsNull ? key : null);
            }
            else
                return InputKey("organisation", publicOnly);
        }

        //--------------------------------------------------------------------------------
        private bool InputVoterID(out int voterID) {
            voterID = -1;
            while (voterID == -1) {
                string voterIDString = Interaction.InputBox("Voter ID", "Enter a voter ID", "");
                if (string.IsNullOrEmpty(voterIDString))
                    return false;
                if (!int.TryParse(voterIDString, out voterID))
                    voterID = -1;
            }
            return true;
        }
        
        //--------------------------------------------------------------------------------
        private bool InputAmount(out decimal amount) {
            amount = 0.0m;
            while (true) {
                string amountString = Interaction.InputBox("Amount", "Enter an amount", "");
                if (string.IsNullOrEmpty(amountString))
                    return false;
                if (decimal.TryParse(amountString, out amount))
                    break;
            }
            return true;
        }

        //--------------------------------------------------------------------------------
        private bool InputVote(out int vote, int maximum, string message) {
            vote = -1;
            while ((vote < 1) || (vote > maximum)) {
                string voteString = Interaction.InputBox(message, "Vote", "");
                if (string.IsNullOrEmpty(voteString))
                    return false;
                if (!int.TryParse(voteString, out vote))
                    vote = -1;
            }
            return true;
        }
    }

}
