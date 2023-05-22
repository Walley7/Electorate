using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate.Blockchain {

    public abstract class BlockchainManager {
        //================================================================================
        private ElectorateInstance              mElectorate;

        private string                          mConnectionString;


        //================================================================================
        //--------------------------------------------------------------------------------
        public BlockchainManager(ElectorateInstance electorate, string connectionString) {
            mElectorate = electorate;
            mConnectionString = connectionString;
        }

        //--------------------------------------------------------------------------------
        public virtual void Dispose() { }


        // ELECTORATE ================================================================================
        //--------------------------------------------------------------------------------
        public ElectorateInstance Electorate { get { return mElectorate; } }


        // CONNECTION ================================================================================
        //--------------------------------------------------------------------------------
        public string ConnectionString { get { return mConnectionString; } }


        // ACCOUNTS ================================================================================
        //--------------------------------------------------------------------------------
        public abstract BlockchainKey CreateAccount(out BlockchainAddress address);
        public BlockchainKey CreateAccount() { return CreateAccount(out BlockchainAddress address); }
        public abstract Task<decimal> Balance(BlockchainKey key);


        // FUNDS ================================================================================
        //--------------------------------------------------------------------------------
        public abstract Task<string> SendFunds(BlockchainKey fromKey, BlockchainAddress toAddress, decimal amount);
        public abstract Task<string> SendFunds(BlockchainKey fromKey, BlockchainKey toKey, decimal amount);


        // CONTRACTS ================================================================================
        //--------------------------------------------------------------------------------
        public abstract Task<DeployContractResult> DeployContract(BlockchainKey accountKey, string contract, string version, params object[] arguments);
        public abstract BlockchainContract Contract(BlockchainAddress address, string abi);


        //================================================================================
        //********************************************************************************
        public struct DeployContractResult {
            public BlockchainAddress address;
            public string abi;

            public DeployContractResult(BlockchainAddress address, string abi) {
                this.address = address;
                this.abi = abi;
            }
        }
    }

}
