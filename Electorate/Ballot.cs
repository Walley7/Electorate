using Electorate.Blockchain;
using Electorate.Exceptions;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate {

    public class Ballot : Entity {
        //================================================================================
        // TODO:
        // - Being able to access RSAKey.Private with only a public organisation key is a
        //   problem. Giving clearing the private field of RSAKey didn't work, best bet is
        //   to split access to it to RSAKeyPublic and RSAKeyPrivate properties, where
        //   RSAKeyPrivate returns null if !mOrganisationKey.IsPrivate.
        //================================================================================
        private BlockchainKey                       mKey;

        private DBField<BlockchainAddress>          mAddress = new DBBlockchainAddressField("Address");
        
        private DBField<string>                     mName = new DBField<string>("Name");
        private DBField<string>                     mABI = new DBField<string>("ABI");
        private DBField<AsymmetricCipherKeyPair>    mRSAKey = new DBRSAKeyField("RSAKey");

        private DBField<decimal?>                   mVoterFundAmount = new DBField<decimal?>("VoterFundAmount");

        private BlockchainContract                  mContract = null;


        //================================================================================
        //--------------------------------------------------------------------------------
        public Ballot(ElectorateInstance electorate, BlockchainKey key, BlockchainAddress address, bool prefetch = false) :
        base(electorate, "Ballot", "Address", address.Address) {
            // Key
            mKey = key;

            // Prefetch
            if (prefetch)
                DBOpenReader("*");

            // Fields
            DBLoad(ref mAddress);
            DBLoad(ref mABI);

            // Prefetch
            if (prefetch) {
                DBLoad(ref mName);
                DBLoad(ref mRSAKey);
                DBLoad(ref mVoterFundAmount);
                DBCloseReader();
            }

            // Contract
            mContract = electorate.Blockchain.Contract(Address, ABI);
        }
        
        
        // ENTITY ================================================================================
        //--------------------------------------------------------------------------------
        public bool Exists { get { return mAddress.Loaded; } }
        

        // KEY ================================================================================
        //--------------------------------------------------------------------------------
        public BlockchainKey Key { get { return mKey; } }


        // FIELDS ================================================================================
        //--------------------------------------------------------------------------------
        public BlockchainAddress Address { get { return DBLoad(ref mAddress).Value; } }
        public string Name { get { return DBLoad(ref mName).Value; } }
        public string ABI { get { return DBLoad(ref mABI).Value; } }
        public AsymmetricCipherKeyPair RSAKey { get { return DBLoad(ref mRSAKey).Value; } }
        
        //--------------------------------------------------------------------------------
        public decimal? VoterFundAmount {
            set { DBLoad(ref mVoterFundAmount).Value = value; }
            get { return DBLoad(ref mVoterFundAmount).Value; }
        }


        // CONTRACT ================================================================================
        //--------------------------------------------------------------------------------
        public BlockchainContract Contract { get { return mContract; } }

        //--------------------------------------------------------------------------------
        public async Task<string> Version(BlockchainKey accessKey) { return await Contract.CallFunction<string>(accessKey, "Version"); }
        public async Task<string> Version() { return await Version(mKey); }
        
        //--------------------------------------------------------------------------------
        public async Task<BlockchainAddress> OwnerAddress(BlockchainKey accessKey) { return new BlockchainAddress(await Contract.CallFunction<string>(accessKey, "Owner")); }
        public async Task<BlockchainAddress> OwnerAddress() { return await OwnerAddress(mKey); }
        
        //--------------------------------------------------------------------------------
        public async Task<DateTime> EndTime(BlockchainKey accessKey) { return Utility.FromSecondsSinceEpoch(await Contract.CallFunction<uint>(accessKey, "EndTime")); }
        public async Task<DateTime> EndTime() { return await EndTime(mKey); }
        

        // OPTIONS ================================================================================
        //--------------------------------------------------------------------------------
        public async Task<BlockchainContract.InvokeResult> LockOptions(BlockchainKey key) { return await Contract.InvokeFunction(key, "LockOptions"); }
        public async Task<BlockchainContract.InvokeResult> LockOptions() { return await LockOptions(mKey); }
        public async Task<bool> OptionsLocked(BlockchainKey key) { return await Contract.CallFunction<bool>(key, "OptionsLocked"); }
        public async Task<bool> OptionsLocked() { return await OptionsLocked(mKey); }
        
        //--------------------------------------------------------------------------------
        public async Task<BlockchainContract.InvokeResult> AddOption(BlockchainKey accessKey, string name) { return await Contract.InvokeFunction(accessKey, "AddOption", name); }
        public async Task<BlockchainContract.InvokeResult> AddOption(string name) { return await AddOption(mKey, name); }
        public async Task<BlockchainContract.InvokeResult> RemoveOption(BlockchainKey accessKey, uint index) { return await Contract.InvokeFunction(accessKey, "RemoveOption", index); }
        public async Task<BlockchainContract.InvokeResult> RemoveOption(uint index) { return await RemoveOption(mKey, index); }
        
        //--------------------------------------------------------------------------------
        public async Task<BallotOption> Option(BlockchainKey accessKey, uint index) {
            BallotOption option = await BallotOption.GetBallotOption(this, accessKey, index);
            if (!option.Exists)
                throw new NotFoundException();
            return option;
        }

        //--------------------------------------------------------------------------------
        public async Task<BallotOption> Option(uint index) { return await Option(mKey, index); }
        public async Task<uint> OptionsCount(BlockchainKey key) { return await Contract.CallFunction<uint>(key, "OptionsCount"); }
        public async Task<uint> OptionsCount() { return await OptionsCount(mKey); }
        

        // VOTERS ================================================================================
        //--------------------------------------------------------------------------------
        public async Task<BlockchainContract.InvokeResult> AddRegisteredVoter(BlockchainKey key, BlockchainAddress address) {
            return await Contract.InvokeFunction(key, "AddVoter", address.Address);
        }

        //--------------------------------------------------------------------------------
        public async Task<BlockchainContract.InvokeResult> AddRegisteredVoter(BlockchainAddress address) { return await AddRegisteredVoter(mKey, address); }
        public async Task<uint> RegisteredVoterCount(BlockchainKey key) { return await Contract.CallFunction<uint>(key, "VoterCount"); }
        public async Task<uint> RegisteredVoterCount() { return await RegisteredVoterCount(mKey); }

        //--------------------------------------------------------------------------------
        public async Task<BlockchainContract.InvokeResult> Vote(BlockchainKey key, uint optionIndex) {
            return await Contract.InvokeFunction(key, "Vote", optionIndex);
        }
        
        //--------------------------------------------------------------------------------
        public async Task<BlockchainContract.InvokeResult> Vote(uint optionIndex) { return await Vote(mKey, optionIndex); }


        // SAVING ================================================================================
        //--------------------------------------------------------------------------------
        public override int SaveChanges() { return SaveChanges(mVoterFundAmount); }
    }

}
