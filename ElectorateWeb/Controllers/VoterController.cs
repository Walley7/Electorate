using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Electorate;
using Electorate.Blockchain;
using Electorate.Exceptions;
using ElectorateDataExt;
using ElectorateDataExt.DTO;
using ElectorateWeb.Filter;
using ElectorateWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Org.BouncyCastle.Crypto.Parameters;

namespace ElectorateWeb.Controllers
{
  [Route("api/[controller]")]
  [CustomException]
  public class VoterController : Controller
  {
    private readonly IConfiguration _configuration;
    ElectorateInstance _electorate;
    ElectorateDB _electorateDB;
    public VoterController(IConfiguration configuration)
    {
      _configuration = configuration;
      var connectionString = _configuration.GetValue<string>("ConnectionStrings:VotingDatabase");
      var key = _configuration.GetValue<string>("BlockChainSettings:BlockChainPrivateKey");
      _electorate = new ElectorateInstance(connectionString, ElectorateInstance.BlockchainType.ETHEREUM, "https://rinkeby.infura.io",
                                                 new BlockchainKey("", key));

      _electorateDB = new ElectorateDB(_electorate, connectionString);

    }


    // POST api/values
    [HttpPost]
    public void Post([FromBody]VoterVM value)
    {
      var key = new BlockchainKey(value.PublicKey, value.PrivateKey);
      var result = _electorate.CreateVoter(key, value.Name);
    }


    [HttpGet]
    public IEnumerable<VoterVM> Get()
    {
      List<VoterVM> voterVMs = new List<VoterVM>();
      var organisations = _electorateDB.GetOrganisations();
      foreach (OrganisationDTO organisation in organisations)
      {
        var key = new BlockchainKey(organisation.PublicKey, organisation.PrivateKey);
        Voter[] voters = _electorate.Voters(key);
        try
        {
          foreach (Voter b in voters)
          {
            var voterVM = new VoterVM();
            voterVM.PublicKey = organisation.PublicKey;
            voterVM.PrivateKey = organisation.PrivateKey;
            voterVM.OrganisationName = organisation.Name;
            voterVM.Name = b.Name;
            voterVM.Id = b.ID ?? 0;
            voterVMs.Add(voterVM);
          }
        }
        catch (Exception ex)
        { }
      }
      return voterVMs;
    }



    [HttpPost("AllocatedVoters")]
    public IEnumerable<VoterVM> AllocatedVoters([FromBody]BallotVM value)
    {
      List<VoterVM> voterVMs = new List<VoterVM>();
      var key = new BlockchainKey(value.PublicKey, value.PrivateKey);
      BlockchainAddress ballotAddress = new BlockchainAddress(value.Address);
      Voter[] voters = _electorate.Voters(key);
      int[] ballotAllocatedVoters = _electorate.BallotAllocatedVoters(key, ballotAddress);

      foreach (Voter voter in voters)
      {
        var voterVM = new VoterVM();
        voterVM.PublicKey = value.PublicKey;
        voterVM.PrivateKey = value.PrivateKey;
        voterVM.OrganisationName = value.OrganisationName;
        voterVM.Name = voter.Name;
        voterVM.Id = voter.ID ?? 0;
        voterVM.BallotAddresskey = value.Address;
        voterVM.IsAllocated = false;
        if (ballotAllocatedVoters != null)
        {
          voterVM.IsAllocated = ballotAllocatedVoters.Contains(voterVM.Id);
        }
        voterVMs.Add(voterVM);
      }
      return voterVMs;
    }


    [HttpPost("AllocatVoters")]
    public IEnumerable<VoterVM> AllocatVoters([FromBody]List<VoterVM> value)
    {
      List<VoterVM> voterVMs = new List<VoterVM>();
      if (value?.Count == 0)
      {
        return voterVMs;
      }
      var firstValue = value.First();
      var key = new BlockchainKey(firstValue.PublicKey, firstValue.PrivateKey);
      BlockchainAddress ballotAddress = new BlockchainAddress(firstValue.BallotAddresskey);
      int[] ballotAllocatedVoters = _electorate.BallotAllocatedVoters(key, ballotAddress);

      foreach (VoterVM voter in value)
      {
        if (voter.IsAllocated == false)
        {
          if (ballotAllocatedVoters.Contains(voter.Id))
          {
            _electorate.DeallocateBallotVoter(key, ballotAddress, voter.Id);
          }
        }
        else
        {
          if (ballotAllocatedVoters.Contains(voter.Id) == false)
          {
            _electorate.AllocateBallotVoter(key, ballotAddress, voter.Id);
          }
        }

        voterVMs.Add(voter);
      }
      return voterVMs;
    }




   [HttpPost("RegisterVoters")]
   public object RegisterVoters([FromBody]VoterVM value)
    {

      var result = string.Empty;
      var key = new BlockchainKey(value.PublicKey, value.PrivateKey);
      BlockchainAddress ballotAddress = new BlockchainAddress(value.BallotAddresskey);
      int[] ballotAllocatedVoters = _electorate.BallotAllocatedVoters(key, ballotAddress);

      if (ballotAllocatedVoters.Contains(value.Id))
      {

        VoterToken voterToken = _electorate.VoterToken(value.Id, ballotAddress);
        VoterAccount voterAccount = _electorate.VoterAccount(value.Id, ballotAddress);

        if ((voterAccount == null) && (voterToken == null))
        {
          result += "Newly registered - ";
          var resonse = Server_1_ProvideBallotPublicRSAKey(value.Id, ballotAddress).Result; // Voter side requests ballot's public RSA key from the ballot side.
        }
        else if (voterAccount == null)
        {
          result += "Resumed from sign voter token - ";
          var resonse = Server_2_SignVoterToken(value.Id, ballotAddress, voterToken.BlindedToken).Result;
        }
        else if (!voterAccount.Registered)
        {
          result += "Resumed from register ballot voter - ";
          var resonse = Server_3_RegisterBallotVoter(value.Id, voterAccount.Address, ballotAddress, voterToken.Token, voterToken.SignedToken).Result;
        }
        else
          result += "Already registered - ";
      }else
      {
        result = "Voter is not allocated.";
      }
      return new { Message = result };
    }


    #region private method

    //--------------------------------------------------------------------------------
    // Ballot side returns ballot's public RSA key across the network, at request of
    // the voter side.
    //--------------------------------------------------------------------------------
    private async Task<bool> Server_1_ProvideBallotPublicRSAKey(int voterID, BlockchainAddress ballotAddress)
    {
      // Server
      Voter voter = _electorate.Voter(voterID);
      Ballot ballot = _electorate.Ballot(voter.OrganisationKey, ballotAddress, true);
      byte[] rsaPublicKeyModulus = ((RsaKeyParameters)ballot.RSAKey.Public).Modulus.ToByteArray();
      byte[] rsaPublicKeyExponent = ((RsaKeyParameters)ballot.RSAKey.Public).Exponent.ToByteArray();

      // Client
      await Client_2_CreateVoterToken(voterID, ballotAddress, rsaPublicKeyModulus, rsaPublicKeyExponent);
      return true;
    }

    //--------------------------------------------------------------------------------
    // Voter side receives and reconstructs the public RSA key (make sure to use
    // Org.BouncyCastle.Math.BigInteger), creates a token to associate with this
    // voter/ballot pair, then sends a request for it to be signed.
    //--------------------------------------------------------------------------------
    private async Task<bool> Client_2_CreateVoterToken(int voterID, BlockchainAddress ballotAddress, byte[] rsaPublicKeyModulus, byte[] rsaPublicKeyExponent)
    {
      // Client
      VoterToken token = _electorate.CreateVoterToken(voterID, ballotAddress, rsaPublicKeyModulus, rsaPublicKeyExponent);

      // Server
      await Server_2_SignVoterToken(voterID, ballotAddress, token.BlindedToken);
      return true;
    }

    //--------------------------------------------------------------------------------
    // Ballot side receives and signs the blinded token (*if* the voter is allocated
    // to the ballot and not already registered), then sends the signed blinded token
    // back.
    //--------------------------------------------------------------------------------
    private async Task<bool> Server_2_SignVoterToken(int voterID, BlockchainAddress ballotAddress, byte[] blindedToken)
    {
      // Server
      byte[] signedBlindedToken = _electorate.SignVoterToken(ballotAddress, voterID, blindedToken);

      // Client
      await Client_3_CreateVoterAccount(voterID, ballotAddress, signedBlindedToken);
      return true;
    }

    //--------------------------------------------------------------------------------
    // Voter side verifies the signed blinded token and creates a new blockchain
    // account to vote with, then sends a request to register this address as a voting
    // address with the ballot.
    //--------------------------------------------------------------------------------
    private async Task<bool> Client_3_CreateVoterAccount(int voterID, BlockchainAddress ballotAddress, byte[] signedBlindedToken)
    {
      // Client
      VoterAccount voterAccount = _electorate.CreateVoterAccount(voterID, ballotAddress, signedBlindedToken);
      VoterToken voterToken = _electorate.VoterToken(voterID, ballotAddress);

      // Server
      await Server_3_RegisterBallotVoter(voterID, voterAccount.Address, ballotAddress, voterToken.Token, voterToken.SignedToken);
      return true;
    }

    //--------------------------------------------------------------------------------
    // Ballot side attempts to register the voter address with the ballot's blockchain
    // contract, making it eligible to vote - if this succeeds the ballot side informs
    // the voter side that it is now able to vote in this ballot.
    //--------------------------------------------------------------------------------
    private async Task<bool> Server_3_RegisterBallotVoter(int voterID, BlockchainAddress voterAddress, BlockchainAddress ballotAddress, byte[] token, byte[] signedToken)
    {
      // Server
      BlockchainContract.InvokeResult result = await _electorate.RegisterBallotVoter(ballotAddress, voterID, voterAddress, token, signedToken);

      // Client
      Client_4_MarkVoterAccountAsRegistered(voterID, ballotAddress);
      return true;
    }

    //--------------------------------------------------------------------------------
    // Voter side marks the voter account as registered.
    //--------------------------------------------------------------------------------
    private void Client_4_MarkVoterAccountAsRegistered(int voterID, BlockchainAddress ballotAddress)
    {
      // Client
      VoterAccount voterAccount = _electorate.VoterAccount(voterID, ballotAddress);
      if (voterAccount == null)
        throw new NotFoundException("Voter not found");
      voterAccount.Registered = true;
      voterAccount.SaveChanges();
    }





    #endregion



  }
}
