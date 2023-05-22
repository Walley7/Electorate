using Electorate.Exceptions;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Contracts;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.RPC.Eth.Filters;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;



namespace Electorate.Blockchain.Ethereum {

    public class EthereumContract : BlockchainContract {
        //================================================================================
        public const long                       GAS_LIMIT_INVOKE = 100000;


        //================================================================================
        private EthereumManager                 mManager;

        private BlockchainAddress               mAddress;
        private string                          mABI;


        //================================================================================
        //--------------------------------------------------------------------------------
        public EthereumContract(EthereumManager manager, BlockchainAddress address, string abi) {
            mManager = manager;
            mAddress = address;
            mABI = abi;
        }


        // FUNCTIONS ================================================================================
        //--------------------------------------------------------------------------------
        private Function Function(BlockchainKey key, string name, out string keyAddress) {
            // Checks
            if (key.IsNull)
                throw new ArgumentNullException();
            if (!key.IsPrivate)
                throw new UnauthorizedAccessException();

            // Connect
            Account account = new Account(key.Private);
            keyAddress = account.Address;
            Web3 web3 = new Web3(account, mManager.ConnectionString);

            // Contract
            Contract contract = web3.Eth.GetContract(mABI, mAddress.Address);

            // Function
            return contract.GetFunction(name);
        }

        //--------------------------------------------------------------------------------
        public override async Task<T> CallFunction<T>(BlockchainKey key, string functionName, params object[] arguments) {
            // Function
            string keyAddress;
            Function function = Function(key, functionName, out keyAddress);

            // Call
            try { return await function.CallAsync<T>(arguments); }
            catch (Exception ex) {
                if (ex.Message.Equals("Invalid account used signing"))
                    throw new UnauthorizedAccessException(ex.Message, ex);
                throw new BlockchainException(ex.Message, ex);
            }
        }

        //--------------------------------------------------------------------------------
        public override async Task<InvokeResult> InvokeFunction(BlockchainKey key, string functionName, params object[] arguments) {
            // Function
            string keyAddress;
            Function function = Function(key, functionName, out keyAddress);

            // Invoke
            try {
                TransactionReceipt receipt = await function.SendTransactionAndWaitForReceiptAsync(keyAddress, new HexBigInteger(GAS_LIMIT_INVOKE), null, null, arguments);
                return new InvokeResult(receipt.TransactionHash);
            }
            catch (Exception ex) {
                if (ex.Message.Equals("Invalid account used signing"))
                    throw new UnauthorizedAccessException(ex.Message, ex);
                throw new BlockchainException(ex.Message, ex);
            }
        }

        
        // EVENTS ================================================================================
        //--------------------------------------------------------------------------------
        private Event Event(BlockchainKey key, string name, out string keyAddress) {
            // Checks
            if (key.IsNull)
                throw new ArgumentNullException();
            if (!key.IsPrivate)
                throw new UnauthorizedAccessException();

            // Connect
            Account account = new Account(key.Private);
            keyAddress = account.Address;
            Web3 web3 = new Web3(account, mManager.ConnectionString);

            // Contract
            Contract contract = web3.Eth.GetContract(mABI, mAddress.Address);

            // Function
            return contract.GetEvent(name);
        }

        //--------------------------------------------------------------------------------
        public override async Task<BlockchainLogEntry[]> ReadEventLogs<T>(BlockchainKey key, string eventName) {
            // Event
            string keyAddress;
            Event evnt = Event(key, eventName, out keyAddress);

            // Logs
            List<EventLog<T>> logs;
            try {
                NewFilterInput filterInput = evnt.CreateFilterInput(BlockParameter.CreateEarliest(), BlockParameter.CreateLatest());
                logs = await evnt.GetAllChanges<T>(filterInput);
            }
            catch (Exception ex) { throw new BlockchainException(ex.Message, ex); }

            // Interpreter
            T interpreter = new T();

            // Read
            List<BlockchainLogEntry> logEntries = new List<BlockchainLogEntry>();
            foreach (EventLog<T> e in logs) {
                BlockchainLogEntry logEntry = new BlockchainLogEntry(eventName);
                logEntry.SetData("block_hash", e.Log.BlockHash);
                logEntry.SetData("block_number", e.Log.BlockNumber.Value.ToString());
                logEntry.SetData("transaction_hash", e.Log.TransactionHash);
                interpreter.ReadEventData(ref logEntry, e.Event);
                logEntries.Add(logEntry);
            }

            return logEntries.ToArray();
        }
    }

}
