using Electorate.Blockchain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Electorate {

    public class BallotOption {
        //================================================================================
        private Ballot                              mBallot;
        private BlockchainKey                       mAccessKey;
        private uint                                mIndex;

        private string                              mName = null;


        //================================================================================
        //--------------------------------------------------------------------------------
        private BallotOption(Ballot ballot, BlockchainKey accessKey, uint index) {
            mBallot = ballot;
            mAccessKey = accessKey;
            mIndex = index;
        }

        //--------------------------------------------------------------------------------
        private async Task<BallotOption> Initialise() {
            mName = await Ballot.Contract.CallFunction<string>(AccessKey, "OptionName", Index);
            return this;
        }
        
        
        // ENTITY ================================================================================
        //--------------------------------------------------------------------------------
        public bool Exists { get { return (mName != null); } }
        
        
        // BALLOT ================================================================================
        //--------------------------------------------------------------------------------
        public Ballot Ballot { get { return mBallot; } }
        
        
        // ACCESS KEY ================================================================================
        //--------------------------------------------------------------------------------
        public BlockchainKey AccessKey { get { return mAccessKey; } }
        
        
        // OPTION ================================================================================
        //--------------------------------------------------------------------------------
        public uint Index { get { return mIndex; } }
        public string Name { get { return mName; } }
        public async Task<uint> Votes() { return await Ballot.Contract.CallFunction<uint>(AccessKey, "OptionVotes", Index); }


        // GET ================================================================================
        //--------------------------------------------------------------------------------
        public static async Task<BallotOption> GetBallotOption(Ballot ballot, BlockchainKey accessKey, uint index) {
            BallotOption option = new BallotOption(ballot, accessKey, index);
            await option.Initialise();
            return option;
        }
    }

}
