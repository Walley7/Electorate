using System;
using System.Collections.Generic;
using System.Text;



namespace Electorate.Blockchain {

    public class BlockchainAddress {
        //================================================================================
        private string                          mAddress;


        //================================================================================
        //--------------------------------------------------------------------------------
        public BlockchainAddress(string address) {
            mAddress = address;
        }

        //--------------------------------------------------------------------------------
        public string Address { get { return mAddress; } }
    }

}
