pragma solidity ^0.4.18;


contract Ballot {
    //================================================================================
    string constant							VERSION = "0.6"; 


    //================================================================================
    address									mOwner;

    string									mName;

    uint                                    mEndTime;

    VotingOption[] public					mOptions;
    bool                                    mOptionsLocked;

    mapping(address => Voter) public		mVoters;
    uint                                    mVoterCount; // Required, as voters is a mapping, not a collection


    //================================================================================
    //--------------------------------------------------------------------------------
    function Ballot(string name, uint endTime) public {
        mOwner = msg.sender;
        mName = name;
        mOptionsLocked = false;
        mVoterCount = 0;
        mEndTime = endTime;
    }
    

    // VERSION ================================================================================
    //--------------------------------------------------------------------------------
    function Version() public pure returns (string) { return VERSION; }


    // OWNER ================================================================================
    //--------------------------------------------------------------------------------
    function Owner() public view returns (address) { return mOwner; }
    

    // NAME ================================================================================
    //--------------------------------------------------------------------------------
    function Name() public view returns (string) { return mName; }


    // END TIME ================================================================================
    //--------------------------------------------------------------------------------
    function EndTime() public view returns (uint) { return mEndTime; }


    // OPTIONS ================================================================================
    //--------------------------------------------------------------------------------
    function AddOption(string optionName) public OwnerOnly {
        require(now <= mEndTime, 'End time has passed');
        require(mOptionsLocked == false, 'Options are locked');
        mOptions.push(VotingOption({name: optionName, votes: 0}));
    }

    //--------------------------------------------------------------------------------
    function RemoveOption(uint index) public OwnerOnly {
        require(now <= mEndTime, 'End time has passed');
        require(mOptionsLocked == false, 'Options are locked');

        // Delete
        delete mOptions[index];

        // Compact
        for (uint i = index; i < mOptions.length; i++) {
            mOptions[i] = mOptions[i + 1];
        }
        delete mOptions[mOptions.length - 1];
        mOptions.length--;
    }
    
    //--------------------------------------------------------------------------------
    function OptionName(uint index) public view returns (string) { return mOptions[index].name; }
    function OptionVotes(uint index) public view returns (uint) { return mOptions[index].votes; }
    function OptionsCount() public view returns (uint) { return mOptions.length; }

    //--------------------------------------------------------------------------------
    function LockOptions() public OwnerOnly {
        // Checks
        require(now <= mEndTime, 'End time has passed');
        require(mOptions.length >= 2, 'Two or more options required before locking');

        // Lock
        if (mOptionsLocked == false) {
            mOptionsLocked = true;
            emit OnLockOptions();
        }
    }

    //--------------------------------------------------------------------------------
    function OptionsLocked() public view returns (bool) { return mOptionsLocked; }


    // VOTERS ================================================================================
    //--------------------------------------------------------------------------------
    function AddVoter(address voterAddress) public OwnerOnly {
        // Checks
        require(now <= mEndTime, 'End time has passed');

        // Add
        if (mVoters[voterAddress].registered != true) {
            mVoters[voterAddress].registered = true;
            mVoterCount += 1;
            emit OnAddVoter(voterAddress, mVoterCount);
        }
    }
    
    //--------------------------------------------------------------------------------
    function VoterCount() public view returns (uint) { return mVoterCount; }
    
    //--------------------------------------------------------------------------------
    function Vote(uint optionIndex) public {
        // Checks
        require(now <= mEndTime, 'End time has passed');
        require(mOptionsLocked == true, 'Options are not locked');
        
        // Voter
        Voter voter = mVoters[msg.sender];

        // Registration check
        require(voter.registered, 'Voter is not registered');

        // Previous vote
        if (voter.voted == true)
            mOptions[voter.votedFor].votes -= 1; // Remove the existing vote of this voter

        // Vote
        voter.voted = true;
        voter.votedFor = optionIndex;
        mOptions[optionIndex].votes += 1;

        // Event
        emit OnVote(msg.sender, optionIndex);
    }


    // EVENTS ================================================================================
    //--------------------------------------------------------------------------------
    event OnLockOptions();
    event OnAddVoter(address indexed voterAddress, uint indexed voterCount);
    event OnVote(address indexed voterAddress, uint indexed optionIndex);


    //================================================================================
    //********************************************************************************
    // Modifer to allow only the owning account to call a function
    modifier OwnerOnly {
        require(msg.sender == mOwner, 'Owner only');
        _; // Continue execution of method
    }

    //********************************************************************************
    struct VotingOption {
        string name;
        uint votes; // Number of votes given to this option
    }

    //********************************************************************************
    // Registered field is required because in a mapping all addresses are mapped -
    // a non-registered voter will still have a mapping, so there needs to be a field
    // within the struct to indicate it.
    struct Voter {
        bool registered;
        bool voted;
        uint votedFor;
    }
}
