using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate.Exceptions {

    public class BlockchainException : Exception {
        //================================================================================
        public BlockchainException() { }
        public BlockchainException(string message) : base(message) { }
        public BlockchainException(string message, Exception inner) : base(message, inner) { }
    }

}
