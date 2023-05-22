using Electorate.Exceptions;
using Nethereum.Hex.HexTypes;
using Nethereum.JsonRpc.Client;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Signer;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Org.BouncyCastle.Crypto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate.Blockchain.Ethereum {

    public class EthereumManager : BlockchainManager {
        //================================================================================
        public const long                       GAS_LIMIT_DEPLOY = 4700000;


        //================================================================================


        //================================================================================
        //--------------------------------------------------------------------------------
        public EthereumManager(ElectorateInstance electorate, string connectionString) : base(electorate, connectionString) {

        }

        //--------------------------------------------------------------------------------
        public override void Dispose() { }


        // ACCOUNTS ================================================================================
        //--------------------------------------------------------------------------------
        public override BlockchainKey CreateAccount(out BlockchainAddress address) {
            EthECKey voterECKey = Nethereum.Signer.EthECKey.GenerateKey();
            address = new BlockchainAddress(voterECKey.GetPublicAddress());
            return new BlockchainKey(voterECKey.GetPubKey(), voterECKey.GetPrivateKeyAsBytes());
        }

        //--------------------------------------------------------------------------------
        public override async Task<decimal> Balance(BlockchainKey key) {
            Account account = new Account(key.Private);
            Web3 web3 = new Web3(account, ConnectionString);
            HexBigInteger balance = await web3.Eth.GetBalance.SendRequestAsync(account.Address);
            return Web3.Convert.FromWei(balance);
        }


        // FUNDS ================================================================================
        //--------------------------------------------------------------------------------
        public override async Task<string> SendFunds(BlockchainKey fromKey, BlockchainAddress toAddress, decimal amount) {
            try {
                Account account = new Account(fromKey.Private);
                decimal balance = await Balance(fromKey);
                Web3 web3 = new Web3(account, ConnectionString);
                return await web3.TransactionManager.SendTransactionAsync(account.Address, toAddress.Address, new HexBigInteger(Web3.Convert.ToWei(amount)));
            }
            catch (Exception ex) { throw new BlockchainException(ex.Message, ex); }
        }

        //--------------------------------------------------------------------------------
        public override async Task<string> SendFunds(BlockchainKey fromKey, BlockchainKey toKey, decimal amount) {
            Account toAccount = new Account(toKey.Private);
            return await SendFunds(fromKey, new BlockchainAddress(toAccount.Address), amount);
        }


        // CONTRACTS ================================================================================
        //--------------------------------------------------------------------------------
        /// <exception cref="InsufficientFundsException"></exception>
        /// <exception cref="BlockchainException"></exception>
        public override async Task<DeployContractResult> DeployContract(BlockchainKey accountKey, string contract, string version, params object[] arguments) {
            // Contract source
            EthereumContractSource contractSource = Electorate.EthereumContractSource(contract, version, true);
            if (contractSource == null)
                throw new NotFoundException("Contract source '" + contract + "':'" + version + "' not found.");

            // Connect
            Account account = new Account(accountKey.Private);
            Web3 web3 = new Web3(account, ConnectionString);

            // Deploy
            TransactionReceipt receipt = null;
            try {
                receipt = await web3.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(contractSource.ABI, contractSource.ByteCode, account.Address,
                                                                                          new HexBigInteger(GAS_LIMIT_DEPLOY), null, arguments);
            }
            catch (RpcResponseException ex) {
                if (ex.Message.Contains("enough funds"))
                    throw new InsufficientFundsException(ex.Message);
                else
                    throw new BlockchainException(ex.Message);
            }

            // Result
            return new DeployContractResult(new BlockchainAddress(receipt.ContractAddress), contractSource.ABI);
        }
        
        //--------------------------------------------------------------------------------
        public override BlockchainContract Contract(BlockchainAddress address, string abi) {
            return new EthereumContract(this, address, abi);
        }
    }

}
