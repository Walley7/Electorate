using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate.Exceptions {

    public class NotAllocatedException : Exception {
        //================================================================================
        public NotAllocatedException() { }
        public NotAllocatedException(string message) : base(message) { }
        public NotAllocatedException(string message, Exception inner) : base(message, inner) { }
    }

}
