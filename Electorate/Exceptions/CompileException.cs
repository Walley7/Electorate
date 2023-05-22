using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate.Exceptions {

    class CompileException : Exception {
        //================================================================================
        public CompileException() { }
        public CompileException(string message) : base(message) { }
        public CompileException(string message, Exception inner) : base(message, inner) { }
    }

}
