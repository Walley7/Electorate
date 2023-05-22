using Electorate.Blockchain;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;



namespace Electorate {

    public class Organisation : Entity {
        //================================================================================
        public enum FundingOption {
            None = 0,
            SystemFunded = 1,
            SelfFunded = 2
        }


        //================================================================================
        private DBField<BlockchainKey>          mKey;

        private DBField<FundingOption?>         mFunding = new DBField<FundingOption?>("Funding");

        /*private DBField<string> mName = new DBField<string>("Name");

        private DBField<string> mRegistrationNo = new DBField<string>("RegistrationNo");

        private DBField<string> mAddress = new DBField<string>("Address");*/


        //================================================================================
        //--------------------------------------------------------------------------------
        public Organisation(ElectorateInstance electorate, BlockchainKey organisationKey, bool prefetch = false) :
        base(electorate, "Organisation", "PublicKey", organisationKey.Public) {
            // Prefetch
            if (prefetch)
                DBOpenReader("*");

            // Key
            if (organisationKey.IsPrivate)
                mKey = new DBBlockchainKeyField("PublicKey", "PrivateKey");
            else
                mKey = new DBBlockchainKeyField("PublicKey");
            DBLoad(ref mKey);

            // Private key mismatch (clear the private key and load it to the field)
            if (organisationKey.IsPrivate && mKey.Loaded && !organisationKey.Equals(mKey.Value)) {
                mKey.Value.Private = null;
                mKey.Load(mKey.Value);
            }

            // Prefetch
            if (prefetch) {
                DBLoad(ref mFunding);
                /*DBLoad(ref mName);
                DBLoad(ref mRegistrationNo);
                DBLoad(ref mAddress);*/
                DBCloseReader();
            }
        }


        // ENTITY ================================================================================
        //--------------------------------------------------------------------------------
        public bool Exists { get { return mKey.Loaded; } }
        

        // KEY ================================================================================
        //--------------------------------------------------------------------------------
        public BlockchainKey Key { get { return mKey.Value; } }
        

        // FIELDS ================================================================================
        //--------------------------------------------------------------------------------
        public FundingOption? Funding {
            set { DBLoad(ref mFunding).Value = value; }
            get { return DBLoad(ref mFunding).Value; }
        }

        //--------------------------------------------------------------------------------
        public bool IsSystemFunded { get { return (Funding == FundingOption.SystemFunded); } }


        /*public string  Name { get { return mName.Value; } }

        public string RegistrationNo { get { return mRegistrationNo.Value; } }

        public string Address { get { return mAddress.Value; } }*/


        // SAVING ================================================================================
        //--------------------------------------------------------------------------------
        public override int SaveChanges() { return SaveChanges(mFunding); }
    }

}
