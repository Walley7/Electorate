using Electorate.Blockchain.Ethereum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate.Blockchain {

    public abstract class BlockchainContract {
        //================================================================================
        //--------------------------------------------------------------------------------
        public BlockchainContract() { }


        // FUNCTIONS ================================================================================
        //--------------------------------------------------------------------------------
        public abstract Task<T> CallFunction<T>(BlockchainKey key, string function, params object[] arguments);
        public abstract Task<InvokeResult> InvokeFunction(BlockchainKey key, string function, params object[] arguments);

        //--------------------------------------------------------------------------------
        /// <typeparam name="T">Specifies the type of the Ethereum event class which will
        /// receive the event log's data</typeparam>
        public abstract Task<BlockchainLogEntry[]> ReadEventLogs<T>(BlockchainKey key, string eventName) where T : EthereumEventInterpreter, new();


        //================================================================================
        //********************************************************************************
        public struct InvokeResult {
            public string hash;

            public InvokeResult(string hash) {
                this.hash = hash;
            }
        }
    }

}
