using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate.Exceptions {

    public class InvalidSignatureException : Exception {
        //================================================================================
        public InvalidSignatureException() { }
        public InvalidSignatureException(string message) : base(message) { }
        public InvalidSignatureException(string message, Exception inner) : base(message, inner) { }
    }

}
