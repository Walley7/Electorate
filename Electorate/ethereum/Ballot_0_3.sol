pragma solidity ^0.4.18;


contract Ballot {
    //================================================================================
    string constant							VERSION = "0.3"; 


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
    function OptionName(uint index) public view returns (string) { return mOptions[index].name; }
    function OptionVotes(uint index) public view returns (uint) { return mOptions[index].votes; }
    function OptionsCount() public view returns (uint) { return mOptions.length; }

    //--------------------------------------------------------------------------------
    function LockOptions() public OwnerOnly {
        require(now <= mEndTime, 'End time has passed');
        require(mOptions.length >= 2, 'Two or more options required before locking');
        mOptionsLocked = true;
    }

    //--------------------------------------------------------------------------------
    function OptionsLocked() public view returns (bool) { return mOptionsLocked; }


    // VOTERS ================================================================================
    //--------------------------------------------------------------------------------
    function AddVoter(address voterAddress) public OwnerOnly {
        require(now <= mEndTime, 'End time has passed');
        mVoters[voterAddress].registered = true;
        mVoterCount += 1;
    }
    
    //--------------------------------------------------------------------------------
    function VoterCount() public view returns (uint) { return mVoterCount; }
    
    //--------------------------------------------------------------------------------
    function Vote(uint optionIndex) public {
        require(now <= mEndTime, 'End time has passed');
        require(mOptionsLocked == true, 'Options are not locked');
        
        Voter voter = mVoters[msg.sender];

        require(voter.registered, 'Voter is not registered');

        if (voter.voted == true)
            mOptions[voter.votedFor].votes -= 1; // Remove the existing vote of this voter
        voter.voted = true;
        voter.votedFor = optionIndex;
        mOptions[optionIndex].votes += 1;
    }


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
