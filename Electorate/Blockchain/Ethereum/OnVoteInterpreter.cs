using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate.Blockchain.Ethereum {

    public class OnVoteInterpreter : EthereumEventInterpreter {
        //================================================================================
        //--------------------------------------------------------------------------------
        [Parameter("address", "voterAddress", 1, true)]
        public string VoterAddress { get; set; }
            
        //--------------------------------------------------------------------------------
        [Parameter("uint", "optionIndex", 2, true)]
        public uint OptionIndex { get; set; }
        
        //--------------------------------------------------------------------------------
        public override void ReadEventData(ref BlockchainLogEntry target, object ethereumEvent) {
            target.SetData("voter_address", ((OnVoteInterpreter)ethereumEvent).VoterAddress);
            target.SetData("option_index", ((OnVoteInterpreter)ethereumEvent).OptionIndex.ToString());
        }
    }

}
