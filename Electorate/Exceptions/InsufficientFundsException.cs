using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate.Exceptions {

    class InsufficientFundsException : Exception {
        //================================================================================
        public InsufficientFundsException() { }
        public InsufficientFundsException(string message) : base(message) { }
        public InsufficientFundsException(string message, Exception inner) : base(message, inner) { }
    }

}
