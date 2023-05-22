using Nethereum.ABI.FunctionEncoding.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate.Blockchain.Ethereum {

    public class OnAddVoterInterpreter : EthereumEventInterpreter {
        //================================================================================
        //--------------------------------------------------------------------------------
        [Parameter("address", "voterAddress", 1, true)]
        public string VoterAddress { get; set; }
            
        //--------------------------------------------------------------------------------
        [Parameter("uint", "voterCount", 2, true)]
        public uint VoterCount { get; set; }
        
        //--------------------------------------------------------------------------------
        public override void ReadEventData(ref BlockchainLogEntry target, object ethereumEvent) {
            target.SetData("voter_address", ((OnAddVoterInterpreter)ethereumEvent).VoterAddress);
            target.SetData("voter_count", ((OnAddVoterInterpreter)ethereumEvent).VoterCount.ToString());
        }
    }

}
