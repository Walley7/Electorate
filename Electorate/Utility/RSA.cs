using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;



namespace Electorate {

    public class RSA {
        // RSA ================================================================================
        //--------------------------------------------------------------------------------
        public static AsymmetricCipherKeyPair GenerateKeyPair() {
            RSACryptoServiceProvider rsaCSP = new RSACryptoServiceProvider(2048);
            RSAParameters rsaParameters = rsaCSP.ExportParameters(true);
            return DotNetUtilities.GetRsaKeyPair(rsaParameters);
        }
        
        //--------------------------------------------------------------------------------
        private static string _ToPemString(object input) {
            StringWriter stringWriter = new StringWriter();
            PemWriter pemWriter = new PemWriter(stringWriter);
            pemWriter.WriteObject(input);
            pemWriter.Writer.Flush();
            string pemString = stringWriter.ToString();
            stringWriter.Dispose();
            return pemString;
        }

        //--------------------------------------------------------------------------------
        public static string KeyPairToPEMString(AsymmetricCipherKeyPair keyPair) { return _ToPemString(keyPair); }
        
        //--------------------------------------------------------------------------------
        public static AsymmetricCipherKeyPair KeyPairFromPEMString(string pemString) {
            PemReader pemReader = new PemReader(new StringReader(pemString));
            return (AsymmetricCipherKeyPair)pemReader.ReadObject();
        }

        //--------------------------------------------------------------------------------
        public static string KeyToPEMString(AsymmetricKeyParameter key) { return _ToPemString(key); }
    }

}
