using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate.Blockchain {

    public class BlockchainLogEntry {
        //================================================================================
        private string                          mEventName;

        private Dictionary<string, string>      mData = new Dictionary<string, string>();


        //================================================================================
        //--------------------------------------------------------------------------------
        public BlockchainLogEntry(string eventName) {
            mEventName = eventName;
        }

        
        // EVENT ================================================================================
        //--------------------------------------------------------------------------------
        public string EventName { get { return mEventName; } }

        
        // DATA ================================================================================
        //--------------------------------------------------------------------------------
        public void SetData(string name, string value) { mData[name] = value; }
        public string Data(string name) { return mData[name]; }
    }

}
