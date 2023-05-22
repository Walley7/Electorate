using Electorate.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate {

    public class VoterTokenSignature : Entity {
        //================================================================================
        private DBField<BlockchainAddress>          mBallotAddress = new DBBlockchainAddressField("BallotAddress");
        private DBField<int?>                       mVoterID = new DBField<int?>("VoterID");

        private DBField<byte[]>                     mBlindedTokenHash = new DBField<byte[]>("BlindedTokenHash");


        //================================================================================
        //--------------------------------------------------------------------------------
        public VoterTokenSignature(ElectorateInstance electorate, BlockchainAddress ballotAddress, int voterID, bool prefetch = true) :
        base(electorate, "VoterTokenSignature", "BallotAddress", ballotAddress.Address, "VoterID", voterID) {
            // Fields
            DBOpenReader("*");
            DBLoad(ref mBallotAddress);
            DBLoad(ref mVoterID);

            // Prefetch
            if (prefetch)
                DBLoad(ref mBlindedTokenHash);

            // Close
            DBCloseReader();
        }


        // ENTITY ================================================================================
        //--------------------------------------------------------------------------------
        public bool Exists { get { return mBallotAddress.Loaded; } }


        // BALLOT ================================================================================
        //--------------------------------------------------------------------------------
        public BlockchainAddress BallotAddress { get { return mBallotAddress.Value; } }


        // VOTER ================================================================================
        //--------------------------------------------------------------------------------
        public int? VoterID { get { return mVoterID.Value; } }


        // BLINDED TOKEN HASH ================================================================================
        //--------------------------------------------------------------------------------
        public byte[] BlindedTokenHash { get { return DBLoad(ref mBlindedTokenHash).Value; } }
    }

}
