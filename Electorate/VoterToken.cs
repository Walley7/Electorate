using Electorate.Blockchain;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate {

    public class VoterToken : Entity {
        //================================================================================
        private DBField<int?>                       mVoterID = new DBField<int?>("VoterID");
        private DBField<BlockchainAddress>          mBallotAddress = new DBBlockchainAddressField("BallotAddress");
        
        private DBField<byte[]>                     mRSAKeyModulus = new DBField<byte[]>("RSAKeyModulus");
        private DBField<byte[]>                     mRSAKeyExponent = new DBField<byte[]>("RSAKeyExponent");

        private DBField<byte[]>                     mToken = new DBField<byte[]>("Token");
        private DBField<byte[]>                     mBlindedToken = new DBField<byte[]>("BlindedToken");
        private DBField<byte[]>                     mBlindingFactor = new DBField<byte[]>("BlindingFactor");
        private DBField<byte[]>                     mSignedToken = new DBField<byte[]>("SignedToken");

        
        //================================================================================
        //--------------------------------------------------------------------------------
        public VoterToken(ElectorateInstance electorate, int voterID, BlockchainAddress ballotAddress, bool prefetch = true) :
        base(electorate, "VoterToken", "VoterID", voterID, "BallotAddress", ballotAddress.Address) {
            // Fields
            DBOpenReader("*");
            DBLoad(ref mVoterID);
            DBLoad(ref mBallotAddress);

            // Prefetch
            if (prefetch) {
                DBLoad(ref mRSAKeyModulus);
                DBLoad(ref mRSAKeyExponent);
                DBLoad(ref mToken);
                DBLoad(ref mBlindedToken);
                DBLoad(ref mBlindingFactor);
                DBLoad(ref mSignedToken);
            }

            // Close
            DBCloseReader();
        }


        // ENTITY ================================================================================
        //--------------------------------------------------------------------------------
        public bool Exists { get { return mVoterID.Loaded; } }


        // VOTER ================================================================================
        //--------------------------------------------------------------------------------
        public int? VoterID { get { return mVoterID.Value; } }


        // BALLOT ================================================================================
        //--------------------------------------------------------------------------------
        public BlockchainAddress BallotAddress { get { return mBallotAddress.Value; } }
        

        // RSA KEY ================================================================================
        //--------------------------------------------------------------------------------
        public RsaKeyParameters RSAPublicKey {
            get {
                DBLoad(ref mRSAKeyModulus);
                DBLoad(ref mRSAKeyExponent);
                return new RsaKeyParameters(false, new BigInteger(mRSAKeyModulus.Value), new BigInteger(mRSAKeyExponent.Value));
            }
        }


        // TOKENS ================================================================================
        //--------------------------------------------------------------------------------
        public byte[] Token { get { return DBLoad(ref mToken).Value; } }
        public byte[] BlindedToken { get { return DBLoad(ref mBlindedToken).Value; } }
        public byte[] BlindingFactor { get { return DBLoad(ref mBlindingFactor).Value; } }
        
        //--------------------------------------------------------------------------------
        public byte[] SignedToken {
            set { DBLoad(ref mSignedToken).Value = value; }
            get { return DBLoad(ref mSignedToken).Value; }
        }


        // SAVING ================================================================================
        //--------------------------------------------------------------------------------
        public override int SaveChanges() { return SaveChanges(mSignedToken); }
    }

}
