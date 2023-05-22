using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate.Exceptions {

    public class DuplicateException : Exception {
        //================================================================================
        public DuplicateException() { }
        public DuplicateException(string message) : base(message) { }
        public DuplicateException(string message, Exception inner) : base(message, inner) { }
    }

}
