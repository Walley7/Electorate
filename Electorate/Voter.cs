using Electorate.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate {

    public class Voter : Entity {
        //================================================================================
        private DBField<int?>                   mID = new DBField<int?>("ID");

        private DBField<BlockchainKey>          mOrganisationKey = new DBBlockchainKeyField("OrganisationPublicKey");

        private DBField<string>                 mName = new DBField<string>("Name");


        //================================================================================
        //--------------------------------------------------------------------------------
        public Voter(ElectorateInstance electorate, int id, bool prefetch = false) :
        base(electorate, "Voter", "ID", id) {
            // Prefetch
            if (prefetch)
                DBOpenReader("*");

            // ID / organisation
            DBLoad(ref mID);
            DBLoad(ref mOrganisationKey);
            
            // Prefetch
            if (prefetch) {
                DBLoad(ref mName);
                DBCloseReader();
            }
        }


        // ENTITY ================================================================================
        //--------------------------------------------------------------------------------
        public bool Exists { get { return mID.Loaded; } }
        

        // ID ================================================================================
        //--------------------------------------------------------------------------------
        public int? ID { get { return mID.Value; } }
        

        // ORGANISATION ================================================================================
        //--------------------------------------------------------------------------------
        public BlockchainKey OrganisationKey { get { return mOrganisationKey.Value; } }
        

        // FIELDS ================================================================================
        //--------------------------------------------------------------------------------
        public string Name { get { return DBLoad(ref mName).Value; } }
    }

}
