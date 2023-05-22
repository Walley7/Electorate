using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;



namespace Electorate {

    public class Utility {
        // EXCEPTIONS ================================================================================
        //--------------------------------------------------------------------------------
        public static Exception InnermostException(Exception e) {
            while (e.InnerException != null) {
                e = e.InnerException;
            }
            return e;
        }

        
        // DATE TIMES ================================================================================
        //--------------------------------------------------------------------------------
        public static long ToSecondsSinceEpoch(DateTime dateTime) {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan difference = dateTime.ToUniversalTime() - origin;
            return (long)Math.Floor(difference.TotalSeconds);
        }

        //--------------------------------------------------------------------------------
        public static DateTime FromSecondsSinceEpoch(long seconds) {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(seconds);
        }


        // ENCRYPTION ================================================================================
        //--------------------------------------------------------------------------------
        public static byte[] Sha256Hash(byte[] input) {
            return new SHA256Managed().ComputeHash(input);
        }
    }

}
