using Electorate.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate {

    public class VoterAccount : Entity {
        //================================================================================
        private DBField<int?>                       mVoterID = new DBField<int?>("VoterID");
        private DBField<BlockchainAddress>          mBallotAddress = new DBBlockchainAddressField("BallotAddress");
        
        private DBField<BlockchainAddress>          mAddress = new DBBlockchainAddressField("Address");
        private DBField<BlockchainKey>              mKey = new DBBlockchainKeyField("PublicKey", "PrivateKey");

        private DBField<bool?>                      mRegistered = new DBField<bool?>("Registered");


        //================================================================================
        //--------------------------------------------------------------------------------
        public VoterAccount(ElectorateInstance electorate, int voterID, BlockchainAddress ballotAddress, bool prefetch = true) :
        base(electorate, "VoterAccount", "VoterID", voterID, "BallotAddress", ballotAddress.Address) {
            // Fields
            DBOpenReader("*");
            DBLoad(ref mVoterID);
            DBLoad(ref mBallotAddress);

            // Prefetch
            if (prefetch) {
                DBLoad(ref mAddress);
                DBLoad(ref mKey);
                DBLoad(ref mRegistered);
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


        // ACCOUNT ================================================================================
        //--------------------------------------------------------------------------------
        public BlockchainAddress Address { get { return DBLoad(ref mAddress).Value; } }
        public BlockchainKey Key { get { return mKey.Value; } }

        //--------------------------------------------------------------------------------
        public bool Registered {
            set { DBLoad(ref mRegistered).Value = value; }
            get { return DBLoad(ref mRegistered).Value ?? false; }
        }


        // SAVING ================================================================================
        //--------------------------------------------------------------------------------
        public override int SaveChanges() { return SaveChanges(mRegistered); }
    }

}
