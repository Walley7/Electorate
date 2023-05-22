using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate.Blockchain.Ethereum {

    public abstract class EthereumEventInterpreter {
        //================================================================================
        //--------------------------------------------------------------------------------
        public abstract void ReadEventData(ref BlockchainLogEntry target, object ethereumEvent);
    }

}
