using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate {

    public class EthereumContractSource : Entity {
        //================================================================================
        private DBField<string>                 mContract = new DBField<string>("Contract");
        private DBField<string>                 mVersion = new DBField<string>("Version");

        private DBField<string>                 mABI = new DBField<string>("ABI");
        private DBField<string>                 mByteCode = new DBField<string>("ByteCode");


        //================================================================================
        //--------------------------------------------------------------------------------
        public EthereumContractSource(ElectorateInstance electorate, string contract, string version, bool prefetch = false) :
        base(electorate, "EthereumContractSource", "Contract", contract, "Version", version) {
            // Prefetch
            if (prefetch)
                DBOpenReader("*");

            // Contract
            DBLoad(ref mContract);
            DBLoad(ref mVersion);
            
            // Prefetch
            if (prefetch) {
                DBLoad(ref mABI);
                DBLoad(ref mByteCode);
                DBCloseReader();
            }
        }


        // ENTITY ================================================================================
        //--------------------------------------------------------------------------------
        public bool Exists { get { return ((Contract != null) && (Version != null)); } }
        

        // CONTRACT ================================================================================
        //--------------------------------------------------------------------------------
        public string Contract { get { return mContract.Value; } }
        public string Version { get { return mVersion.Value; } }

        //--------------------------------------------------------------------------------
        public string ABI {
            set { DBLoad(ref mABI).Value = value; }
            get { return DBLoad(ref mABI).Value; }
        }

        //--------------------------------------------------------------------------------
        public string ByteCode {
            set { DBLoad(ref mByteCode).Value = value; }
            get { return DBLoad(ref mByteCode).Value; }
        }
        

        // SAVING ================================================================================
        //--------------------------------------------------------------------------------
        public override int SaveChanges() { return SaveChanges(mABI, mByteCode); }
    }

}
