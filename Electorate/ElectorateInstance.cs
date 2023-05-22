using Electorate.Blockchain;
using Electorate.Blockchain.Ethereum;
using Electorate.Exceptions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using SolcNet;
using SolcNet.DataDescription.Input;
using SolcNet.DataDescription.Output;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;



namespace Electorate {

    public class ElectorateInstance {
        //================================================================================
        // TODO:
        // - Move organisation refunding to own function, call from lock options, add
        //   option and register ballot voter.
        // - Add auditing.
        // - Fund organisation account on creation?
        // - Make system funding thresholds customisable.
        // - Figure out whether to build in further voter funding customisation.
        // - Make gas limits for ethereum contracts not hard coded.
        //================================================================================
        public enum BlockchainType {
            ETHEREUM
        }

        //--------------------------------------------------------------------------------
        public const int                        BALANCE_WAIT_SLEEP_PERIOD = 50; // Milliseconds to sleep thread between balance waits
        public const decimal                    BALLOT_VOTER_INITIAL_FUNDS = 0.005M;


        //================================================================================
        private SqlConnection                   mDatabaseConnection;

        private BlockchainManager               mBlockchain;

        private BlockchainKey                   mSystemKey;


        //================================================================================
        /// <summary>
        /// Establishes a connection to the database and initialises the necessary
        /// structures for connecting to a blockchain.
        /// </summary>
        /// <param name="databaseConnectionString"> SQL connection string to the database.</param>
        /// <param name="blockchainType">Only one option at the moment: ElectorateInstance.BlockchainType.ETHEREUM.</param>
        /// <param name="blockchainConnectionString">Connection string for the blockchain network to connect to - usually an IP address.</param>
        /// <param name="systemKey">
        /// Private key of the system's blockchain account on the blockchain network - this account is used to fund organisations if they're not
        /// self funding.
        /// </param>
        //--------------------------------------------------------------------------------
        public ElectorateInstance(string databaseConnectionString, BlockchainType blockchainType, string blockchainConnectionString, BlockchainKey systemKey) {
            // Database
            mDatabaseConnection = new SqlConnection(databaseConnectionString);
            mDatabaseConnection.Open();

            // Blockchain
            switch (blockchainType) {
                case BlockchainType.ETHEREUM:   mBlockchain = new EthereumManager(this, blockchainConnectionString); break;
            }

            // System key
            mSystemKey = systemKey;
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Closes the database and blockchain connects and disposes the associated
        /// structures. Should generally be called as the final closing call of an
        /// ElectorateInstance.
        /// </summary>
        //--------------------------------------------------------------------------------
        public void Dispose() {
            mDatabaseConnection.Close();
            mDatabaseConnection = null;
            mBlockchain.Dispose();
            mBlockchain = null;
        }


        // DATABASE ================================================================================
        //--------------------------------------------------------------------------------
        /// <summary>The database connection object.</summary>
        //--------------------------------------------------------------------------------
        public SqlConnection DatabaseConnection { get { return mDatabaseConnection; } }


        // BLOCKCHAIN ================================================================================
        //--------------------------------------------------------------------------------
        /// <summary>The database connection object.</summary>
        //--------------------------------------------------------------------------------
        public BlockchainManager Blockchain { get { return mBlockchain; } }


        // SYSTEM ================================================================================
        //--------------------------------------------------------------------------------
        /// <summary>The private key of the system's blockchain account.</summary>
        //--------------------------------------------------------------------------------
        public BlockchainKey SystemKey { get { return mSystemKey; } }

        //--------------------------------------------------------------------------------
        /// <summary>The balance of the system's blockchain account.</summary>
        //--------------------------------------------------------------------------------
        public async Task<decimal> SystemBalance() { return await Blockchain.Balance(mSystemKey); }

        //--------------------------------------------------------------------------------
        /// <summary>The balance threshold below which a system funded organisation will
        /// be automatically funded by the system.</summary>
        //--------------------------------------------------------------------------------
        public decimal SystemFundingThreshold { get { return 0.5m; } }

        //--------------------------------------------------------------------------------
        /// <summary>The amount a system funded organisation will be funded with when it
        /// falls below SystemFundingThreshold.</summary>
        //--------------------------------------------------------------------------------
        public decimal SystemFundingAmount { get { return 0.5m; } }


        // ORGANISATIONS ================================================================================
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Generates an account on the blockchain for the organisation, and then stores
        /// the private key in a record on the database.
        /// account.
        /// </summary>
        /// <param name="funding">
        /// SystemFunded = system automatically funds the organisation's blockchain
        /// account when needed., SelfFunded = organisation has to fund their blockchain
        /// account manually.
        /// </param>
        /// <returns>The private key of the new organisation.</returns>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public BlockchainKey CreateOrganisation(Organisation.FundingOption funding) {
            // Blockchain
            BlockchainKey key = Blockchain.CreateAccount();

            // Database
            SqlCommand command = new SqlCommand("insert into Organisation (PublicKey, PrivateKey, Funding) values (@PublicKey, @PrivateKey, @Funding)", DatabaseConnection);
            command.Parameters.AddWithValue("@PublicKey", key.Public);
            command.Parameters.AddWithValue("@PrivateKey", key.Private);
            command.Parameters.AddWithValue("@Funding", funding);
            command.ExecuteNonQuery();
            command.Dispose();

            // Return
            return key;
        }

        //--------------------------------------------------------------------------------
        /// <summary>Returns an instance of an Organisation class, which provides access
        /// to the organisation's data in the database and on the blockchain.</summary>
        /// <param name="key">The organisation's private key.</param>
        /// <param name="prefetch">Whether to immediately retrieve all of the database fields.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public Organisation Organisation(BlockchainKey key, bool prefetch = false) {
            // Checks
            if ((key == null) || (key.Public == null))
                throw new ArgumentNullException();

            // Organisation
            Organisation organisation = new Organisation(this, key, prefetch);
            if (!organisation.Exists)
                throw new NotFoundException("Organisation not found");
            return organisation;
        }

        //--------------------------------------------------------------------------------
        /// <summary>A list of organisations in the system.</summary>
        /// <param name="prefetch">Whether to immediately retrieve all of the database fields.</param>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public Organisation[] Organisations(bool prefetch = false) {
            // Read
            SqlCommand command = new SqlCommand("select PublicKey, PrivateKey from Organisation", DatabaseConnection);
            SqlDataReader reader = command.ExecuteReader();

            // Public keys
            List<byte[]> publicKeys = new List<byte[]>();
            List<byte[]> privateKeys = new List<byte[]>();
            while (reader.Read()) {
                if ((reader["PublicKey"] != DBNull.Value) && (reader["PrivateKey"] != DBNull.Value)) {
                    publicKeys.Add((byte[])reader["PublicKey"]);
                    privateKeys.Add((byte[])reader["PrivateKey"]);
                }
            }

            // Close
            reader.Close();
            command.Dispose();

            // Organisations
            Organisation[] organisations = new Organisation[publicKeys.Count]; 
            for (int i = 0; i < organisations.Length; ++i) {
                BlockchainKey key = new BlockchainKey(publicKeys[i], privateKeys[i]);
                organisations[i] = Organisation(key, prefetch);
            }

            return organisations;
        }
        
        //--------------------------------------------------------------------------------
        /// <summary>A count of the number of organisations in the system.</summary>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public int OrganisationCount() {
            SqlCommand command = new SqlCommand("select Count(*) from Organisation", DatabaseConnection);
            int count = (int)command.ExecuteScalar();
            command.Dispose();
            return count;
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Retrieves the private key of the organisation key associated with the provided
        /// public key.
        /// This function should only ever be used internally within the system, as it's
        /// essentially a password retrieval.
        /// </summary>
        /// <param name="key">The organisation's public key.</param>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public BlockchainKey OrganisationPrivateKey(BlockchainKey key) {
            // Checks
            if ((key == null) || (key.Public == null))
                throw new ArgumentNullException();

            // Retrieve
            SqlCommand command = new SqlCommand("select PrivateKey from Organisation where PublicKey = @PublicKey", DatabaseConnection);
            command.Parameters.AddWithValue("@PublicKey", key.Public);
            byte[] privateKey = (byte[])command.ExecuteScalar();
            command.Dispose();

            // Key
            return new BlockchainKey(key.Public, privateKey);
        }
        
        //--------------------------------------------------------------------------------
        /// <summary>The organisation's balance on the blockchain.</summary>
        /// <param name="key">The organisation's private key.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        //--------------------------------------------------------------------------------
        public async Task<decimal> OrganisationBalance(BlockchainKey key) {
            // Organisation
            Organisation organisation = Organisation(key);
            if (!organisation.Key.IsPrivate)
                throw new UnauthorizedAccessException();

            // Balance
            return await Blockchain.Balance(organisation.Key);
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// If the organisation is eligible (system funded and below the threshold), this
        /// will send funds from the system's blockchain account to the organisation's
        /// blockchain account.
        /// </summary>
        /// <param name="key">The organisation's public key.</param>
        //--------------------------------------------------------------------------------
        public async Task<string> FundOrganisationIfEligible(BlockchainKey key) {
            // Organisation
            Organisation organisation = Organisation(key);

            // Balance
            if (organisation.IsSystemFunded) {
                // Fund
                string receipt = null;
                decimal balance = await Blockchain.Balance(organisation.Key);
                if (balance < SystemFundingThreshold)
                    receipt = await Blockchain.SendFunds(SystemKey, organisation.Key, SystemFundingAmount);

                // Wait
                while (await Blockchain.Balance(organisation.Key) < SystemFundingThreshold) {
                    Thread.Sleep(BALANCE_WAIT_SLEEP_PERIOD);
                }

                return receipt;
            }

            return null;
        }


        // BALLOTS ================================================================================
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Deploys a new ballot to the blockchain, owned by the supplied organisation.
        /// For Ethereum, the ballot's source code is retrieved from the ethereum contract
        /// sources.
        /// </summary>
        /// <param name="organisationKey">The organisation's private key.</param>
        /// <param name="contract">The name of the contract in the sources.</param>
        /// <param name="version">The version of the contract in the sources.</param>
        /// <param name="name">The name of the ballot.</param>
        /// <param name="endTime">The date and time at which the ballot ends.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public async Task<BlockchainAddress> CreateBallot(BlockchainKey organisationKey, string contract, string version, string name, DateTime endTime) {
            // Organisation
            Organisation organisation = Organisation(organisationKey);
            if (!organisation.Key.IsPrivate)
                throw new UnauthorizedAccessException();

            // Organisation funding
            await FundOrganisationIfEligible(organisation.Key);

            // Deploy
            BlockchainManager.DeployContractResult deployResult;
            deployResult = await Blockchain.DeployContract(organisation.Key, contract, version, name, Utility.ToSecondsSinceEpoch(endTime));

            // RSA key
            AsymmetricCipherKeyPair rsaKey = RSA.GenerateKeyPair();

            // Database
            SqlCommand command = new SqlCommand("insert into Ballot (Address, OrganisationPublicKey, Name, ABI, RSAKey, VoterFundAmount) " +
                                                "values (@Address, @OrganisationPublicKey, @Name, @ABI, @RSAKey, @VoterFundAmount)", DatabaseConnection);
            command.Parameters.AddWithValue("@Address", deployResult.address.Address);
            command.Parameters.AddWithValue("@OrganisationPublicKey", organisation.Key.Public);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@ABI", deployResult.abi);
            command.Parameters.AddWithValue("@RSAKey", RSA.KeyPairToPEMString(rsaKey));
            command.Parameters.AddWithValue("@VoterFundAmount", BALLOT_VOTER_INITIAL_FUNDS);
            command.ExecuteNonQuery();
            command.Dispose();
            return deployResult.address;
        }

        //--------------------------------------------------------------------------------
        /// <summary>Returns an instance of a Ballot class, which provides access to the
        /// ballot's data in the database and on the blockchain.</summary>
        /// <param name="key">The public or private key of the blockchain account which
        /// will interact with the ballot on the blockchain for this instance of the
        /// Ballot class.</param>
        /// <param name="address">The ballot's address on the blockchain.</param>
        /// <param name="prefetch">Whether to immediately retrieve all of the database fields.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public Ballot Ballot(BlockchainKey key, BlockchainAddress address, bool prefetch = false) {
            // Checks
            if (address == null)
                throw new ArgumentNullException();

            // Ballot
            Ballot ballot = new Ballot(this, key, address, prefetch);
            if (!ballot.Exists)
                throw new NotFoundException("Ballot not found");
            return ballot;
        }
        
        //--------------------------------------------------------------------------------
        /// <summary>A list of ballot instances, one for each ballot owned by the
        /// specified organisation.</summary>
        /// <param name="organisationKey">The owning organisation's private key.</param>
        /// <param name="prefetch">Whether to immediately retrieve all of the database fields.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public Ballot[] Ballots(BlockchainKey organisationKey, bool prefetch = false) {
            // Organisation
            Organisation organisation = Organisation(organisationKey);

            // Read
            SqlCommand command = new SqlCommand("select Address from Ballot where OrganisationPublicKey = @OrganisationPublicKey", DatabaseConnection);
            command.Parameters.AddWithValue("@OrganisationPublicKey", organisation.Key.Public);
            SqlDataReader reader = command.ExecuteReader();
            
            // Addresses
            List<BlockchainAddress> addresses = new List<BlockchainAddress>();
            while (reader.Read()) {
                if (reader["Address"] != DBNull.Value)
                    addresses.Add(new BlockchainAddress((string)reader["Address"]));
            }

            // Close
            reader.Close();
            command.Dispose();

            // Ballots
            Ballot[] ballots = new Ballot[addresses.Count]; 
            for (int i = 0; i < ballots.Length; ++i) {
                ballots[i] = Ballot(organisation.Key, addresses[i], prefetch);
            }

            return ballots;
        }

        //--------------------------------------------------------------------------------
        /// <summary>A count of the number of ballots owned by the specified organisation.
        /// </summary>
        /// <param name="organisationKey">The owning organisation's public key.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public int BallotCount(BlockchainKey organisationKey) {
            // Organisation
            Organisation organisation = Organisation(organisationKey);

            // Count
            SqlCommand command = new SqlCommand("select Count(*) from Ballot where OrganisationPublicKey = @OrganisationPublicKey", DatabaseConnection);
            command.Parameters.AddWithValue("@OrganisationPublicKey", organisation.Key.Public);
            int count = (int)command.ExecuteScalar();
            command.Dispose();
            return count;
        }
        
        //--------------------------------------------------------------------------------
        /// <summary>Locks the ballot's options - after this point any attempts to add or
        /// remove options for this ballot will fail.</summary>
        /// <param name="organisationKey">The owning organisation's private key.</param>
        /// <param name="address">The ballot's address on the blockchain.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        //--------------------------------------------------------------------------------
        public async Task<BlockchainContract.InvokeResult> LockBallotOptions(BlockchainKey organisationKey, BlockchainAddress address) {
            // Organisation funding
            await FundOrganisationIfEligible(organisationKey);
            
            // Organisation
            Organisation organisation = Organisation(organisationKey);
            if (!organisation.Key.IsPrivate)
                throw new UnauthorizedAccessException();

            // Lock options
            Ballot ballot = Ballot(organisation.Key, address);
            return await ballot.LockOptions();
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Any key with a private key will suffice - it just has to be enough to access
        /// an account with, which a public key alone is not.
        /// </summary>
        /// <param name="key">The public or private key of the blockchain account which
        /// will interact with the ballot on the blockchain for this instance of the
        /// Ballot class.</param>
        /// <param name="address">The ballot's address on the blockchain.</param>
        /// <exception cref="NotFoundException"></exception>
        //--------------------------------------------------------------------------------
        public async Task<bool> BallotOptionsLocked(BlockchainKey key, BlockchainAddress address) {
            Ballot ballot = Ballot(key, address);
            return await ballot.OptionsLocked();
        }

        //--------------------------------------------------------------------------------
        /// <summary>Adds an option to the ballot. If the ballot is locked or the end time
        /// has passed, this will fail.</summary>
        /// <param name="organisationKey">The owning organisation's private key.</param>
        /// <param name="address">The ballot's address on the blockchain.</param>
        /// <param name="name">The name of the option.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="BlockchainException"></exception>
        //--------------------------------------------------------------------------------
        public async Task<BlockchainContract.InvokeResult> AddBallotOption(BlockchainKey organisationKey, BlockchainAddress address, string name) {
            // Organisation
            Organisation organisation = Organisation(organisationKey);
            if (!organisation.Key.IsPrivate)
                throw new UnauthorizedAccessException();

            // Organisation funding
            await FundOrganisationIfEligible(organisation.Key);
            
            // Add option
            Ballot ballot = Ballot(organisation.Key, address);
            return await ballot.AddOption(name);
        }
        
        //--------------------------------------------------------------------------------
        /// <summary>Removes an option from the ballot. If the ballot is locked or the
        /// end time has passed, this will fail.</summary>
        /// <param name="organisationKey">The owning organisation's private key.</param>
        /// <param name="address">The ballot's address on the blockchain.</param>
        /// <param name="index">The index of the option.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="BlockchainException"></exception>
        //--------------------------------------------------------------------------------
        public async Task<BlockchainContract.InvokeResult> RemoveBallotOption(BlockchainKey organisationKey, BlockchainAddress address, uint index) {
            // Organisation
            Organisation organisation = Organisation(organisationKey);
            if (!organisation.Key.IsPrivate)
                throw new UnauthorizedAccessException();

            // Organisation funding
            await FundOrganisationIfEligible(organisation.Key);

            // Remove option
            Ballot ballot = Ballot(organisation.Key, address);
            return await ballot.RemoveOption(index);
        }
        
        //--------------------------------------------------------------------------------
        /// <summary>Retrieves a structure which provides access to information and voting
        /// data for the option.</summary>
        /// <param name="key">The public or private key of the blockchain account which
        /// will retrieve the ballot option.</param>
        /// <param name="address">The ballot's address on the blockchain.</param>
        /// <param name="index">The index of the option.</param>
        /// <exception cref="NotFoundException"></exception>
        //--------------------------------------------------------------------------------
        public async Task<BallotOption> BallotOption(BlockchainKey key, BlockchainAddress address, uint index) {
            Ballot ballot = Ballot(key, address);
            return await ballot.Option(index);
        }

        //--------------------------------------------------------------------------------
        /// <summary>A count of the number of options the ballot has.</summary>
        /// <param name="key">The public or private key of the blockchain account which
        /// will retrieve the ballot option.</param>
        /// <param name="address">The ballot's address on the blockchain.</param>
        /// <param name="index">The index of the option.</param>
        /// <exception cref="NotFoundException"></exception>
        //--------------------------------------------------------------------------------
        public async Task<uint> BallotOptionsCount(BlockchainKey key, BlockchainAddress address) {
            Ballot ballot = Ballot(key, address);
            return await ballot.OptionsCount();
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// A list of log entries for the ballot for the specified event.
        /// Log fields are stored as string/string key value pairs. These include:
        /// "block_hash"
        /// "block_number"
        /// "transaction_hash"
        /// </summary>
        /// <param name="key">The public or private key of the blockchain account which
        /// will retrieve the ballot logs.</param>
        /// <param name="address">The ballot's address on the blockchain.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="BlockchainException"></exception>
        //--------------------------------------------------------------------------------
        public async Task<BlockchainLogEntry[]> BallotEventLogs<T>(BlockchainKey key, BlockchainAddress address, string eventName) where T : EthereumEventInterpreter, new() {
            Ballot ballot = Ballot(key, address);
            return await ballot.Contract.ReadEventLogs<T>(key, eventName);
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// A list of log entries for when the ballot has been locked.
        /// Log fields are stored as string/string key value pairs. These include:
        /// "block_hash"
        /// "block_number"
        /// "transaction_hash"
        /// </summary>
        /// <param name="key">The public or private key of the blockchain account which
        /// will retrieve the ballot logs.</param>
        /// <param name="address">The ballot's address on the blockchain.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="BlockchainException"></exception>
        //--------------------------------------------------------------------------------
        public async Task<BlockchainLogEntry[]> BallotLockOptionsLogs(BlockchainKey key, BlockchainAddress address) {
            return await BallotEventLogs<OnLockOptionsInterpreter>(key, address, "OnLockOptions");
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// A list of log entries for when voters have been added to the ballot.
        /// Log fields are stored as string/string key value pairs. These include:
        /// "block_hash"
        /// "block_number"
        /// "transaction_hash"
        /// </summary>
        /// <param name="key">The public or private key of the blockchain account which
        /// will retrieve the ballot logs.</param>
        /// <param name="address">The ballot's address on the blockchain.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="BlockchainException"></exception>
        //--------------------------------------------------------------------------------
        public async Task<BlockchainLogEntry[]> BallotAddVoterLogs(BlockchainKey key, BlockchainAddress address) {
            return await BallotEventLogs<OnAddVoterInterpreter>(key, address, "OnAddVoter");
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// A list of log entries for when voters have voters in the ballot.
        /// Log fields are stored as string/string key value pairs. These include:
        /// "block_hash"
        /// "block_number"
        /// "transaction_hash"
        /// </summary>
        /// <param name="key">The public or private key of the blockchain account which
        /// will retrieve the ballot logs.</param>
        /// <param name="address">The ballot's address on the blockchain.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="BlockchainException"></exception>
        //--------------------------------------------------------------------------------
        public async Task<BlockchainLogEntry[]> BallotVoteLogs(BlockchainKey key, BlockchainAddress address) {
            return await BallotEventLogs<OnVoteInterpreter>(key, address, "OnVote");
        }


        // VOTERS ================================================================================
        //--------------------------------------------------------------------------------
        /// <summary>
        /// Creates a voter record in the database for the specified organisation. This
        /// has no impact on the blockchain - that comes later during voter registration.
        /// </summary>
        /// <param name="organisationKey">The owning organisation's private key.</param>
        /// <param name="name">The name of the voter in the database.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public int CreateVoter(BlockchainKey organisationKey, string name) {
            // Checks
            if ((organisationKey == null) || (organisationKey.Public == null))
                throw new ArgumentNullException();

            // Organisation
            Organisation organisation = Organisation(organisationKey);
            if (!organisation.Key.IsPrivate)
                throw new UnauthorizedAccessException();

            // Database
            SqlCommand command = new SqlCommand("insert into Voter (OrganisationPublicKey, Name) output Inserted.ID " +
                                                "values (@OrganisationPublicKey, @Name)", DatabaseConnection);
            command.Parameters.AddWithValue("@OrganisationPublicKey", organisation.Key.Public);
            command.Parameters.AddWithValue("@Name", name);
            int id = (int)command.ExecuteScalar();
            command.Dispose();

            // Return
            return id;
        }
        
        //--------------------------------------------------------------------------------
        /// <summary>Returns an instance of a Voter class, which provides access to the
        /// voter's data in the database.</summary>
        /// <param name="id">ID of the voter (primary key in the database).</param>
        /// <param name="prefetch">Whether to immediately retrieve all of the database fields.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public Voter Voter(int id, bool prefetch = false) {
            Voter voter = new Voter(this, id, prefetch);
            if (!voter.Exists)
                throw new NotFoundException("Voter not found");
            return voter;
        }
        
        //--------------------------------------------------------------------------------
        /// <summary>A list of voters instances, one for each voter owned by the specified
        /// organisation.</summary>
        /// <param name="organisationKey">The owning organisation's public key.</param>
        /// <param name="prefetch">Whether to immediately retrieve all of the database fields.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public Voter[] Voters(BlockchainKey organisationKey, bool prefetch = false) {
            // Checks
            if ((organisationKey == null) || (organisationKey.Public == null))
                throw new ArgumentNullException();

            // Organisation
            Organisation organisation = Organisation(organisationKey);

            // Read
            SqlCommand command = new SqlCommand("select ID from Voter where OrganisationPublicKey = @OrganisationPublicKey", DatabaseConnection);
            command.Parameters.AddWithValue("@OrganisationPublicKey", organisation.Key.Public);
            SqlDataReader reader = command.ExecuteReader();
            
            // IDs
            List<int> ids = new List<int>();
            while (reader.Read()) {
                if (reader["ID"] != DBNull.Value)
                    ids.Add((int)reader["ID"]);
            }

            // Close
            reader.Close();
            command.Dispose();

            // Voters
            Voter[] voters = new Voter[ids.Count]; 
            for (int i = 0; i < voters.Length; ++i) {
                voters[i] = Voter(ids[i], prefetch);
            }

            return voters;
        }

        //--------------------------------------------------------------------------------
        /// <summary>A count of the number of voters owned by the specified organisation.
        /// </summary>
        /// <param name="organisationKey">The owning organisation's public key.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public int VoterCount(BlockchainKey organisationKey) {
            // Checks
            if ((organisationKey == null) || (organisationKey.Public == null))
                throw new ArgumentNullException();

            // Organisation
            Organisation organisation = Organisation(organisationKey);

            // Count
            SqlCommand command = new SqlCommand("select Count(*) from Voter where OrganisationPublicKey = @OrganisationPublicKey", DatabaseConnection);
            command.Parameters.AddWithValue("@OrganisationPublicKey", organisation.Key.Public);
            int count = (int)command.ExecuteScalar();
            command.Dispose();
            return count;
        }
        

        // VOTER ALLOCATIONS ================================================================================
        //--------------------------------------------------------------------------------
        /// <summary>Allocates a voter to a ballot (ergo, grants them eligibility to
        /// register to voter in it) and adds an entry to the BallotVoterAllocation table
        /// in the database. Allocations are purely database side - nothing is committed
        /// to the blockchain.</summary>
        /// <param name="organisationKey">The owning organisation's private key.</param>
        /// <param name="ballotAddress">The ballot's address on the blockchain.</param>
        /// <param name="voterID">The ID of the voter.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public void AllocateBallotVoter(BlockchainKey organisationKey, BlockchainAddress ballotAddress, int voterID) {
            // Organisation
            Organisation organisation = Organisation(organisationKey);
            if (!organisation.Key.IsPrivate)
                throw new UnauthorizedAccessException();

            // Ballot
            Ballot ballot = Ballot(organisation.Key, ballotAddress);

            // Voter
            Voter voter = Voter(voterID);

            // Database
            SqlCommand command = new SqlCommand("insert into BallotVoterAllocation (BallotAddress, VoterID) values (@BallotAddress, @VoterID)", DatabaseConnection);
            command.Parameters.AddWithValue("@BallotAddress", ballotAddress.Address);
            command.Parameters.AddWithValue("@VoterID", voter.ID);
            command.ExecuteNonQuery();
            command.Dispose();
        }
        
        //--------------------------------------------------------------------------------
        /// <summary>Removes a voter's allocation from a ballot. Note that if the user has
        /// already registered for the ballot, this change has no impact on that - voter
        /// allocations only control future registrations.</summary>
        /// <param name="organisationKey">The owning organisation's private key.</param>
        /// <param name="ballotAddress">The ballot's address on the blockchain.</param>
        /// <param name="voterID">The ID of the voter.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public bool DeallocateBallotVoter(BlockchainKey organisationKey, BlockchainAddress ballotAddress, int voterID) {
            // Organisation
            Organisation organisation = Organisation(organisationKey);
            if (!organisation.Key.IsPrivate)
                throw new UnauthorizedAccessException();

            // Ballot
            Ballot ballot = Ballot(organisation.Key, ballotAddress);

            // Voter
            Voter voter = Voter(voterID);

            // Database
            SqlCommand command = new SqlCommand("delete from BallotVoterAllocation where BallotAddress = @BallotAddress and @VoterID = VoterID", DatabaseConnection);
            command.Parameters.AddWithValue("@BallotAddress", ballotAddress.Address);
            command.Parameters.AddWithValue("@VoterID", voter.ID);
            int deleteCount = command.ExecuteNonQuery();
            command.Dispose();
            return (deleteCount > 0);
        }

        //--------------------------------------------------------------------------------
        /// <summary>Returns whether a voter is allocated to a ballot.</summary>
        /// <param name="key">The public or private key of the blockchain account which
        /// will be used to access the ballot.</param>
        /// <param name="ballotAddress">The ballot's address on the blockchain.</param>
        /// <param name="voterID">The ID of the voter.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public bool BallotHasVoterAllocation(BlockchainKey key, BlockchainAddress ballotAddress, int voterID) {
            // Ballot / voter
            Ballot ballot = Ballot(key, ballotAddress);
            Voter voter = Voter(voterID);

            // Database
            SqlCommand command = new SqlCommand("select Count(*) from BallotVoterAllocation where BallotAddress = @BallotAddress and @VoterID = VoterID", DatabaseConnection);
            command.Parameters.AddWithValue("@BallotAddress", ballotAddress.Address);
            command.Parameters.AddWithValue("@VoterID", voter.ID);
            int count = (int)command.ExecuteScalar();
            command.Dispose();
            return (count != 0);
        }

        //--------------------------------------------------------------------------------
        /// <summary>A list of voters allocated to the specified ballot.</summary>
        /// <param name="organisationKey">The owning organisation's public key.</param>
        /// <param name="ballotAddress">The ballot's address on the blockchain.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public int[] BallotAllocatedVoters(BlockchainKey organisationKey, BlockchainAddress ballotAddress) {
            // Ballot
            Ballot ballot = Ballot(organisationKey, ballotAddress);

            // Database
            SqlCommand command = new SqlCommand("select VoterID from BallotVoterAllocation where BallotAddress = @BallotAddress", DatabaseConnection);
            command.Parameters.AddWithValue("@BallotAddress", ballotAddress.Address);
            SqlDataReader reader = command.ExecuteReader();
            

            // Voters
            List<int> voters = new List<int>();
            while (reader.Read()) {
                voters.Add((int)reader["VoterID"]);
            }

            reader.Close();
            command.Dispose();

            return voters.ToArray();
        }
        
        //--------------------------------------------------------------------------------
        /// <summary>A count of the number of voter allocations for an organisation.
        /// </summary>
        /// <param name="organisationKey">The owning organisation's public key.</param>
        /// <param name="ballotAddress">The ballot's address on the blockchain.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public int BallotAllocatedVoterCount(BlockchainKey organisationKey, BlockchainAddress ballotAddress) {
            // Ballot
            Ballot ballot = Ballot(organisationKey, ballotAddress);

            // Database
            SqlCommand command = new SqlCommand("select Count(*) from BallotVoterAllocation where BallotAddress = @BallotAddress", DatabaseConnection);
            command.Parameters.AddWithValue("@BallotAddress", ballotAddress.Address);
            int count = (int)command.ExecuteScalar();
            command.Dispose();
            return count;
        }


        // VOTER REGISTRATIONS ================================================================================        
        //--------------------------------------------------------------------------------
        /// <summary>
        /// <para>Creates a voter token for a voter/ballot pair, which is used to initiate
        /// and later verify a voter registration.
        /// </para>
        /// <para>Voter tokens are used to allow a separation between the ballot a voter is
        /// registering for, and knowledge of the blockchain address the voter will use to
        /// vote. The token, once signed, serves as authentication that the sender of the
        /// token is the same voter who was registered to vote with it.</para>
        /// <para>This also requires that the voter side of the application be aware of the
        /// ballot's public RSA key. This can be accessed via Ballot.RSAKey.Public, and
        /// could then be sent over the network to the front facing system which voters
        /// register through.</para>
        /// </summary>
        /// <param name="voterID">The ID of the voter.</param>
        /// <param name="ballotAddress">The ballot's address on the blockchain.</param>
        /// <param name="ballotRSAPublicKey">The public RSA key of the ballot</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public VoterToken CreateVoterToken(int voterID, BlockchainAddress ballotAddress, AsymmetricKeyParameter ballotRSAPublicKey) {
            // Checks
            if (ballotRSAPublicKey.IsPrivate)
                throw new ArgumentException();

            // Already exists?
            VoterToken token = VoterToken(voterID, ballotAddress);
            if (token != null)
                return token;
            
            // Signature
            BlindSignature signature = new BlindSignature();
            signature.Blind(ballotRSAPublicKey);

            // RSA key
            RsaKeyParameters rsaKey = (RsaKeyParameters)ballotRSAPublicKey;

            // Create
            SqlCommand command = new SqlCommand("insert into VoterToken (VoterID, BallotAddress, RSAKeyModulus, RSAKeyExponent, Token, BlindedToken, BlindingFactor) " +
                                                "values (@VoterID, @BallotAddress, @RSAKeyModulus, @RSAKeyExponent, @Token, @BlindedToken, @BlindingFactor)", DatabaseConnection);
            command.Parameters.AddWithValue("@VoterID", voterID);
            command.Parameters.AddWithValue("@BallotAddress", ballotAddress.Address);
            command.Parameters.AddWithValue("@RSAKeyModulus", rsaKey.Modulus.ToByteArray());
            command.Parameters.AddWithValue("@RSAKeyExponent", rsaKey.Exponent.ToByteArray());
            command.Parameters.AddWithValue("@Token", signature.Token);
            command.Parameters.AddWithValue("@BlindedToken", signature.BlindedToken);
            command.Parameters.AddWithValue("@BlindingFactor", signature.BlindingFactor.ToByteArray());
            command.ExecuteNonQuery();
            command.Dispose();

            // Token
            return VoterToken(voterID, ballotAddress);
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Convenience method for passing in RSA key components.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public VoterToken CreateVoterToken(int voterID, BlockchainAddress ballotAddress, byte[] rsaPublicKeyModulus, byte[] rsaPublicKeyExponent) {
            RsaKeyParameters ballotPublicRSAKey = new RsaKeyParameters(false, new BigInteger(rsaPublicKeyModulus), new BigInteger(rsaPublicKeyExponent));
            return CreateVoterToken(voterID, ballotAddress, ballotPublicRSAKey);
        }        

        //--------------------------------------------------------------------------------
        /// <summary>
        /// <para>Returns a previously created voter token for a voter/ballot pair, if
        /// it exists</para>
        /// <para>Unlike other access functions, token functions return null instead of throwing
        /// an exception if the token isn't found. This is to make checking if the token
        /// already exists cleaner.</para>
        /// </summary>
        /// <returns>The voter token if it exists, or null otherwise.</returns>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public VoterToken VoterToken(int voterID, BlockchainAddress ballotAddress, bool prefetch = true) {
            VoterToken token = new VoterToken(this, voterID, ballotAddress, prefetch);
            return (token.Exists ? token : null);
        }
        
        //--------------------------------------------------------------------------------
        /// <summary>
        /// <para>Signs a blinded voter token and returns the signed version. Used to
        /// establish two-way trust for the voting process without having to know which
        /// voter address belongs to which voter. The signed token is also committed to
        /// the database as a VoterTokenSignature record.</para>
        /// <para>If a separation exists between the system which voters directly interact
        /// with, and the system which manages ballots, this should be called from the
        /// system which manages ballots, and then communicated between the two systems in
        /// some manner (e.g. across the network).</para>
        /// </summary>
        /// <param name="organisationKey">The owning organisation's private key.</param>
        /// <param name="ballotAddress">The ballot's address on the blockchain.</param>
        /// <param name="voterID">The ID of the voter.</param>
        /// <param name="blindedToken">The blinded token.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="NotAllocatedException"></exception>
        /// <exception cref="DuplicateException"></exception>
        /// <exception cref="SqlException"></exception>
        // <para>NO LONGER TRUE: The signed blinded token generated here is only generated
        // once, and not stored by the ballot side of the system - as such it's important
        // that it gets back to the voter side of the system. One approach to this would
        // be to maintain a table of outgoing signed blinded tokens, which are cleared
        // once the voter side has confirmed receiving them, and re-sent periodically if
        // it hasn't.</para>
        //--------------------------------------------------------------------------------
        public byte[] SignVoterToken(BlockchainKey organisationKey, BlockchainAddress ballotAddress, int voterID, byte[] blindedToken) {
            // Organisation
            Organisation organisation = Organisation(organisationKey);
            if (!organisation.Key.IsPrivate)
                throw new UnauthorizedAccessException();

            // Ballot
            Ballot ballot = Ballot(organisation.Key, ballotAddress);
            
            // Checks
            if (!BallotHasVoterAllocation(organisation.Key, ballotAddress, voterID))
                throw new NotAllocatedException("Voter is not allocated to this ballot");
            //if (VoterTokenSignature(ballotAddress, voterID) != null)
            //    throw new DuplicateException("A token has already been signed for this ballot/voter combination");

            // Sign
            byte[] signedBlindedToken = BlindSignature.Sign(blindedToken, ballot.RSAKey.Private);

            // Database
            if (VoterTokenSignature(ballotAddress, voterID) == null) {
                SqlCommand command = new SqlCommand("insert into VoterTokenSignature (BallotAddress, VoterID, BlindedTokenHash) " +
                                                "values (@BallotAddress, @VoterID, @BlindedTokenHash)", DatabaseConnection);
                command.Parameters.AddWithValue("@BallotAddress", ballotAddress.Address);
                command.Parameters.AddWithValue("@VoterID", voterID);
                command.Parameters.AddWithValue("@BlindedTokenHash", Utility.Sha256Hash(blindedToken));
                command.ExecuteNonQuery();
                command.Dispose();
            }

            // Return
            return signedBlindedToken;
        }

        //--------------------------------------------------------------------------------
        /// <summary>Convenience method for looking up the voter's organisation.</summary>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="NotAllocatedException"></exception>
        /// <exception cref="DuplicateException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public byte[] SignVoterToken(BlockchainAddress ballotAddress, int voterID, byte[] blindedToken) {
            Voter voter = Voter(voterID);
            BlockchainKey organisationKey = OrganisationPrivateKey(voter.OrganisationKey); // Lookup the organisation that the voter belongs to
            return SignVoterToken(organisationKey, ballotAddress, voterID, blindedToken);
        }

        //--------------------------------------------------------------------------------
        /// <summary>Retrieves a structure containing the ballot side hash of the blinded
        /// token received from a voter and signed for later verification.</summary>
        /// <param name="ballotAddress">The ballot's address on the blockchain.</param>
        /// <param name="voterID">The ID of the voter.</param>
        /// <param name="prefetch">Whether to immediately retrieve all of the database fields.</param>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public VoterTokenSignature VoterTokenSignature(BlockchainAddress ballotAddress, int voterID, bool prefetch = true) {
            VoterTokenSignature tokenSignature = new VoterTokenSignature(this, ballotAddress, voterID, prefetch);
            return (tokenSignature.Exists ? tokenSignature : null);
        }
        
        //--------------------------------------------------------------------------------
        /// <summary>Creates a blockchain account for this voter/ballot pair, to used by
        /// this voter to voter in the ballot.</summary>
        /// <param name="voterID">The ID of the voter.</param>
        /// <param name="ballotAddress">The ballot's address on the blockchain.</param>
        /// <param name="signedBlindedToken">The signed blinded token for the voter/ballot pair.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="DuplicateException"></exception>
        /// <exception cref="InvalidSignatureException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public VoterAccount CreateVoterAccount(int voterID, BlockchainAddress ballotAddress, byte[] signedBlindedToken) {
            // Checks
            if (VoterAccount(voterID, ballotAddress) != null)
                throw new DuplicateException("Voter account already exists for this voter/ballot pair");

            // Token
            VoterToken token = VoterToken(voterID, ballotAddress);
            if (token == null)
                throw new NotFoundException("Voter token not found");

            // Signature
            BlindSignature signature = new BlindSignature(token.Token, null);
            signature.SetSignature(signedBlindedToken, token.RSAPublicKey, new BigInteger(token.BlindingFactor));
            if (!signature.VerifySignature(token.RSAPublicKey))
                throw new InvalidSignatureException("Signed blinded token is not a match");

            // Update token
            token.SignedToken = signature.SignedToken;
            token.SaveChanges();

            // Voter account
            BlockchainAddress voterAddress;
            BlockchainKey voterKey = Blockchain.CreateAccount(out voterAddress);

            // Database
            SqlCommand command = new SqlCommand("insert into VoterAccount (VoterID, BallotAddress, Address, PublicKey, PrivateKey) " +
                                                "values (@VoterID, @BallotAddress, @Address, @PublicKey, @PrivateKey)", DatabaseConnection);
            command.Parameters.AddWithValue("@VoterID", voterID);
            command.Parameters.AddWithValue("@BallotAddress", ballotAddress.Address);
            command.Parameters.AddWithValue("@Address", voterAddress.Address);
            command.Parameters.AddWithValue("@PublicKey", voterKey.Public);
            command.Parameters.AddWithValue("@PrivateKey", voterKey.Private);
            command.ExecuteNonQuery();
            command.Dispose();

            // Return
            return VoterAccount(voterID, ballotAddress);
        }
        
        //--------------------------------------------------------------------------------
        /// <summary>Returns a VoterAccount instance which provides acccess to data for
        /// the registration between the voter/ballot pair and the voting blockchain
        /// account itself.</summary>
        /// <param name="voterID">The ID of the voter.</param>
        /// <param name="ballotAddress">The ballot's address on the blockchain.</param>
        /// <param name="prefetch">Whether to immediately retrieve all of the database fields.</param>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public VoterAccount VoterAccount(int voterID, BlockchainAddress ballotAddress, bool prefetch = true) {
            VoterAccount account = new VoterAccount(this, voterID, ballotAddress, prefetch);
            return (account.Exists ? account : null);
        }

        //--------------------------------------------------------------------------------
        /// <summary>Finalises registration of a voter with a ballot, using the generated
        /// voter blockchain account and the signed voter token.</summary>
        /// <param name="organisationKey">The owning organisation's private key.</param>
        /// <param name="ballotAddress">The ballot's address on the blockchain.</param>
        /// <param name="voterAddress">The blockchain address of the voter account.</param>
        /// <param name="token">The unsigned voter token for this voter/ballot pair.</param>
        /// <param name="signedToken">The signed voter token for this voter/ballot pair.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="InvalidSignatureException"></exception>
        //--------------------------------------------------------------------------------
        public async Task<BlockchainContract.InvokeResult> RegisterBallotVoter(BlockchainKey organisationKey, BlockchainAddress ballotAddress, BlockchainAddress voterAddress, byte[] token, byte[] signedToken) {
            // Organisation
            Organisation organisation = Organisation(organisationKey);
            if (!organisation.Key.IsPrivate)
                throw new UnauthorizedAccessException();
            
            // Ballot
            Ballot ballot = Ballot(organisation.Key, ballotAddress);

            // Verify tokens
            BlindSignature signature = new BlindSignature(token, signedToken);
            if (!signature.VerifySignature(ballot.RSAKey.Public))
                throw new InvalidSignatureException("Provided tokens are not a match");
            
            // Organisation funding
            await FundOrganisationIfEligible(organisation.Key);

            // Register
            BlockchainContract.InvokeResult result = await ballot.AddRegisteredVoter(voterAddress);

            // Database
            SqlCommand command = new SqlCommand("insert into BallotVoterRegistration (BallotAddress, VoterAddress, SignedTokenHash) " +
                                                "values (@BallotAddress, @VoterAddress, @SignedTokenHash)", DatabaseConnection);
            command.Parameters.AddWithValue("@BallotAddress", ballotAddress.Address);
            command.Parameters.AddWithValue("@VoterAddress", voterAddress.Address);
            command.Parameters.AddWithValue("@SignedTokenHash", Utility.Sha256Hash(signedToken));
            command.ExecuteNonQuery();
            command.Dispose();

            // Funds
            string fundsReceipt = await Blockchain.SendFunds(organisation.Key, voterAddress, (decimal)ballot.VoterFundAmount);

            // Return
            return result;
        }

        //--------------------------------------------------------------------------------
        /// <summary>
        /// Convenience method for looking up the voter's organisation.
        /// </summary>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="UnauthorizedAccessException"></exception>
        /// <exception cref="NotAllocatedException"></exception>
        /// <exception cref="DuplicateException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public async Task<BlockchainContract.InvokeResult> RegisterBallotVoter(BlockchainAddress ballotAddress, int voterID, BlockchainAddress voterAddress, byte[] token, byte[] signedToken) {
            Voter voter = Voter(voterID);
            BlockchainKey organisationKey = OrganisationPrivateKey(voter.OrganisationKey); // Lookup the organisation that the voter belongs to
            return await RegisterBallotVoter(organisationKey, ballotAddress, voterAddress, token, signedToken);
        }
        
        //--------------------------------------------------------------------------------
        /// <summary>Returns whether a voter is registered to a ballot.</summary>
        /// <param name="organisationKey">The owning organisation's private key.</param>
        /// <param name="ballotAddress">The ballot's address on the blockchain.</param>
        /// <param name="voterAddress">The blockchain address of the voter account.</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public bool BallotHasVoterRegistration(BlockchainKey organisationKey, BlockchainAddress ballotAddress, BlockchainAddress voterAddress) {
            // Ballot
            Ballot ballot = Ballot(organisationKey, ballotAddress);

            // Database
            SqlCommand command = new SqlCommand("select Count(*) from BallotVoterRegistration where BallotAddress = @BallotAddress and @VoterAddress = VoterAddress", DatabaseConnection);
            command.Parameters.AddWithValue("@BallotAddress", ballotAddress.Address);
            command.Parameters.AddWithValue("@VoterAddress", voterAddress.Address);
            int count = (int)command.ExecuteScalar();
            command.Dispose();
            return (count != 0);
        }
        
        //--------------------------------------------------------------------------------
        /// <summary>Casts a vote to the specified ballot, on behalf of the blockchain
        /// account associated with the provided private key.</summary>
        /// <param name="ballotAddress">The ballot's address on the blockchain.</param>
        /// <param name="voterAccountKey">The private key of the voter account for this voter/ballot pair.</param>
        /// <param name="optionIndex">The index of the option to vote for</param>
        /// <exception cref="NotFoundException"></exception>
        /// <exception cref="BlockchainException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public async Task<BlockchainContract.InvokeResult> Vote(BlockchainAddress ballotAddress, BlockchainKey voterAccountKey, uint optionIndex) {
            // Ballot
            Ballot ballot = Ballot(voterAccountKey, ballotAddress);

            // Vote
            return await ballot.Vote(optionIndex);
        }

        
        // ETHEREUM - CONTRACT SOURCES ================================================================================
        //--------------------------------------------------------------------------------
        // Until we've implemented another blockchain, it's hard to envision how this
        // would best be abstracted, so for now it's exposed as Ethereum specific methods.
        //--------------------------------------------------------------------------------
        /// <summary>Compiles the specified solidity files into Ethereum contacts and
        /// stores them in the database for deployment as ballots.</summary>
        /// <exception cref="CompileException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public void AddEthereumContractSource(string contractsPath, string contractFileName, string contractClassName, string version, out string[] warnings) {
            // Solc
            SolcLib solcLib = SolcLib.Create(contractsPath);
            //Console.Out.WriteLine("Version: " + solcLib.VersionDescription);

            // Compile
            OutputDescription compileOutput = solcLib.Compile(new[] { contractFileName }, new[] { OutputType.Abi, OutputType.EvmBytecode });

            // Contract
            //Console.Out.WriteLine(compileOutput.Contracts[contractFileName][contractClassName].AbiJsonString);
            //Console.Out.WriteLine("0x" + compileOutput.Contracts[contractFileName][contractClassName].Evm.Bytecode.Object);
            //foreach (KeyValuePair<string, Dictionary<string, Contract>> f in compileOutput.Contracts) {
            //    foreach (KeyValuePair<string, Contract> c in f.Value) { }
            //}

            // Errors / warnings
            List<string> warningList = new List<string>();
            foreach (Error e in compileOutput.Errors) {
                if (e.Severity == Severity.Error)
                    throw new CompileException(e.FormattedMessage);
                else
                    warningList.Add(e.FormattedMessage);
            }
            warnings = warningList.ToArray();

            // Database
            try {
                SqlCommand command = new SqlCommand("insert into EthereumContractSource (Contract, Version, ABI, Bytecode) output Inserted.Contract values (@Contract, @Version, @ABI, @Bytecode)", DatabaseConnection);
                command.Parameters.AddWithValue("@Contract", contractClassName);
                command.Parameters.AddWithValue("@Version", version);
                command.Parameters.AddWithValue("@ABI", compileOutput.Contracts[contractFileName][contractClassName].AbiJsonString);
                command.Parameters.AddWithValue("@Bytecode", "0x" + compileOutput.Contracts[contractFileName][contractClassName].Evm.Bytecode.Object);
                string insertedContract = (string)command.ExecuteScalar();
                Console.Out.WriteLine("inserted: " + insertedContract);
                command.Dispose();
            }
            catch (SqlException ex) {
                throw ex;
            }
        }

        //--------------------------------------------------------------------------------
        /// <summary>Compiles the specified solidity files into Ethereum contacts and
        /// stores them in the database for deployment as ballots.</summary>
        /// <exception cref="CompileException"></exception>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public void AddEthereumContractSource(string contractsPath, string contractFileName, string contractClassName, string version) {
            string[] warnings;
            AddEthereumContractSource(contractsPath, contractFileName, contractClassName, version, out warnings);
        }

        //--------------------------------------------------------------------------------
        /// <summary>Retrieves a previously compiled ethereum contract source.</summary>
        /// <exception cref="SqlException"></exception>
        //--------------------------------------------------------------------------------
        public EthereumContractSource EthereumContractSource(string contract, string version, bool prefetch = false) {
            EthereumContractSource source = new EthereumContractSource(this, contract, version, prefetch);
            return (source.Exists ? source : null);
        }




        //================================================================================
        // KETAN'S CODE:
        // Have commented these out, as well as the extra fields in the Organisation
        // class. The intended design is that extra data be added in your own custom
        // tables outside of the sql tables Electorate directly uses, and then you join
        // to them by the primary key, so it would be better if you switched to doing it
        // that way. Sorry if it's a pain - I get terrified of code entropy, it doesn't
        // take much for a code base to become a nasty mess, so I try to keep it orderly :)!
        //
        // Also let me know what you were going for with the candidates functions, I sense
        // there might be some confusion about what's what in the library - I'm happy to
        // clarify!
        //--------------------------------------------------------------------------------

        /*public List<Organisation> GetOrganisations( bool prefetch = false)
        {
           
            // Read
            SqlCommand command = new SqlCommand("select *  from Organisation  Order by Name ", DatabaseConnection);            
            SqlDataReader reader = command.ExecuteReader();

            // IDs
            List<BlockchainKey> ids = new List<BlockchainKey>();
            while (reader.Read())
            {
                BlockchainKey key;
                if ((reader["PublicKey"] != DBNull.Value) && (reader["PrivateKey"] != DBNull.Value))
                {
                    key = new BlockchainKey((byte[])reader["PublicKey"], (byte[])reader["PrivateKey"]);
                }else
                {
                    key = new BlockchainKey((byte[])reader["PublicKey"]);
                }

                ids.Add(key);

            }

            // Close
            reader.Close();
            command.Dispose();

            // Voters
            List<Organisation> Organisations = new List<Organisation>();
            foreach(BlockchainKey key in ids)
            {
                Organisation organisation = this.Organisation(key, true);
                Organisations.Add(organisation);
            }
           

            return Organisations;
        }*/

        /*
        public List<BallotOption> GetCandidates(bool prefetch = false)
        {

            // Read
            SqlCommand command = new SqlCommand("select *  from BallotOption  Order by Name ", DatabaseConnection);
            SqlDataReader reader = command.ExecuteReader();

            // IDs
            List<BlockchainKey> ids = new List<BlockchainKey>();
            while (reader.Read())
            {
                BlockchainKey key;
                if ((reader["PublicKey"] != DBNull.Value) && (reader["PrivateKey"] != DBNull.Value))
                {
                    key = new BlockchainKey((byte[])reader["PublicKey"], (byte[])reader["PrivateKey"]);
                }
                else
                {
                    key = new BlockchainKey((byte[])reader["PublicKey"]);
                }

                ids.Add(key);

            }

            // Close
            reader.Close();
            command.Dispose();

            // Voters
            List<Organisation> Organisations = new List<Organisation>();
            foreach (BlockchainKey key in ids)
            {
                Organisation organisation = this.Organisation(key, true);
                Organisations.Add(organisation);
            }


            return Organisations;
        }*/

            /*
        public async Task<string> CreateCandidate(BlockchainKey organisationKey, string name, string ballotAddresskey, string address)
        {
            try
            {
                // Organisation
                Organisation organisation = Organisation(organisationKey);
                if (!organisation.Key.IsPrivate)
                    throw new UnauthorizedAccessException();

                BlockchainAddress ballotAddress = new BlockchainAddress(ballotAddresskey);

                // Organisation funding
                await FundOrganisationIfEligible(organisation.Key);

                // Deploy
                var result = await AddBallotOption(organisationKey, ballotAddress, name);
                var OptoionKey = result.hash;

                // // Database
                // SqlCommand command = new SqlCommand("insert into BallotOption (CandidateKey, BallotKey, OrganisationPrivateKey, OrganisationPublicKey, Name, Address) " +
                //                                    "values (@CandidateKey, @BallotKey, @OrganisationPrivateKey, @OrganisationPublicKey, @Name, @Address)", DatabaseConnection);
                //command.Parameters.AddWithValue("@CandidateKey", OptoionKey);
                //command.Parameters.AddWithValue("@BallotKey", ballotAddresskey);
                //command.Parameters.AddWithValue("@OrganisationPrivateKey", organisationKey.PrivateString);
                //command.Parameters.AddWithValue("@OrganisationPublicKey", organisationKey.PublicString);
                //command.Parameters.AddWithValue("@Name", name);
                //command.Parameters.AddWithValue("@Address", address);                
                //command.ExecuteNonQuery();
                //command.Dispose();
                return OptoionKey;

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        */
    }

}
