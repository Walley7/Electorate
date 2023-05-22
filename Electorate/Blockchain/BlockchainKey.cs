using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace Electorate.Blockchain {

    public class BlockchainKey {
        //================================================================================
        // Never allow keys to be changed once this class is instantiated - many functions
        // are built around the assumption it's final.
        //================================================================================
        private byte[]                          mPublic = null;
        private byte[]                          mPrivate = null;


        //================================================================================
        //--------------------------------------------------------------------------------
        public BlockchainKey(byte[] publicKey, byte[] privateKey = null) {
            mPublic = publicKey;
            mPrivate = privateKey;
        }
        
        //--------------------------------------------------------------------------------
        public BlockchainKey(string publicKey, string privateKey = null) {
            try { mPublic = Enumerable.Range(0, publicKey.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(publicKey.Substring(x, 2), 16)).ToArray(); }
            catch (Exception) { }
            try { mPrivate = (privateKey != null) ? Enumerable.Range(0, privateKey.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(privateKey.Substring(x, 2), 16)).ToArray() : null; }
            catch (Exception) { }
        }

        //--------------------------------------------------------------------------------
        public byte[] Public {
            set { mPublic = value; }
            get { return mPublic; }
        }
        
        //--------------------------------------------------------------------------------
        public string PublicString { get { return (mPublic != null) ? BitConverter.ToString(mPublic).Replace("-", "") : ""; } }

        //--------------------------------------------------------------------------------
        public byte[] Private {
            set { mPublic = value; }
            get { return mPrivate; }
        }

        //--------------------------------------------------------------------------------
        public string PrivateString { get { return (mPrivate != null) ? BitConverter.ToString(mPrivate).Replace("-", "") : ""; } }

        //--------------------------------------------------------------------------------
        public bool IsPrivate { get { return (Private != null); } }
        public bool IsNull { get { return (Public == null) && (Private == null); } }


        // COMPARISON ================================================================================
        //--------------------------------------------------------------------------------
        public override bool Equals(Object other) {
            if ((other == null) || (GetType() != other.GetType()))
                return false;
            BlockchainKey key = (BlockchainKey)other;
            return (((mPublic == key.Public) || mPublic.SequenceEqual(key.Public)) && ((mPrivate == key.Private) || mPrivate.SequenceEqual(key.Private)));
        }

        //--------------------------------------------------------------------------------
        public override int GetHashCode() { return base.GetHashCode(); }
    }

}
