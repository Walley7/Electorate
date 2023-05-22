using System;
using System.Collections.Generic;
using System.Linq;
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

namespace ElectorateWeb.Controllers
{
  [Route("api/[controller]")]
  [CustomException]
  public class CandidateController : Controller
  {
    private readonly IConfiguration _configuration;
    ElectorateInstance _electorate;
    ElectorateDB _electorateDB;
    public CandidateController(IConfiguration configuration)
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
    public void Post([FromBody]CandidateVM value)
    {

      var key = new BlockchainKey(value.PublicKey, value.PrivateKey);
      var organisations = _electorateDB.CreateCandidate(key, value.Name, value.BallotAddresskey, string.Empty);
      var result = organisations.Result;


    }


    [HttpGet]
    public IEnumerable<CandidateVM> Get()
    {
      List<CandidateVM> CandidateVMs = new List<CandidateVM>();

      var organisations = _electorateDB.GetOrganisations();
      foreach (OrganisationDTO organisation in organisations)
      {
        var key = new BlockchainKey(organisation.PublicKey, organisation.PrivateKey);
        Ballot[] ballots = _electorate.Ballots(key);
        try
        {
          foreach (Ballot b in ballots)
          {
            var optionsResult = b.OptionsCount().Result;
            for (uint i = 0; i < optionsResult; ++i)
            {

              BallotOption option =  b.Option(i).Result;
              var candidate = new CandidateVM();
              candidate.PublicKey = organisation.PublicKey;
              candidate.PrivateKey = organisation.PublicKey;
              candidate.BallotAddresskey = b.Address.Address;
              candidate.Name = option.Name;
              candidate.OrganisationName = organisation.Name;
              candidate.BallotName = b.Name;
              CandidateVMs.Add(candidate);

            }
          }
        }
        catch (Exception ex)
        { }

       

      }

      return CandidateVMs;

    }


    [HttpPost("CandidatesOfBallots")]
    public IEnumerable<CandidateVM> CandidatesOfBallots([FromBody]VoterVM value)
    {
      List<CandidateVM> CandidateVMs = new List<CandidateVM>();
      BlockchainAddress ballotAddress = new BlockchainAddress(value.BallotAddresskey);
      VoterAccount voterAccount = _electorate.VoterAccount(value.Id, ballotAddress);
      if (voterAccount == null)
        return CandidateVMs;

      Ballot ballot = _electorate.Ballot(voterAccount.Key, ballotAddress);
      uint optionsCount =  ballot.OptionsCount().Result;
      for (uint i = 0; i < optionsCount; ++i)
      {
        var ballotOption = ballot.Option(i).Result;
        var candidate = new CandidateVM();
        candidate.PublicKey = value.PublicKey;
        candidate.PrivateKey = value.PrivateKey;
        candidate.BallotAddresskey = value.BallotAddresskey;
        candidate.Name = ballotOption.Name;
        candidate.OrganisationName = value.OrganisationName;
        candidate.BallotName = ballot.Name;
        candidate.Index = ballotOption.Index;
        candidate.UserId = value.Id;
        CandidateVMs.Add(candidate);
      }

      return CandidateVMs;
    }

    [HttpPost("CastVote")]
    public object CastVote([FromBody]CandidateVM value)
    {      
        BlockchainAddress ballotAddress = new BlockchainAddress(value.BallotAddresskey);
        VoterAccount voterAccount = _electorate.VoterAccount(value.UserId, ballotAddress);
        BlockchainContract.InvokeResult result = _electorate.Vote(ballotAddress, voterAccount.Key, (uint)(value.Index)).Result;
        return new { Message = "Vote is casted" };
    }



  }
}
