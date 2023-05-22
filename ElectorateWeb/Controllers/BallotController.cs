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
  public class BallotController : Controller
  {
    private readonly IConfiguration _configuration;
    private readonly string _ballotVersion;
    ElectorateInstance _electorate;
    ElectorateDB _electorateDB;
    public BallotController(IConfiguration configuration)
    {
      _configuration = configuration;
      var connectionString = _configuration.GetValue<string>("ConnectionStrings:VotingDatabase");
      var key = _configuration.GetValue<string>("BlockChainSettings:BlockChainPrivateKey");
      _ballotVersion = _configuration.GetValue<string>("BlockChainSettings:BallotVersion");
      _electorate = new ElectorateInstance(connectionString, ElectorateInstance.BlockchainType.ETHEREUM, "https://rinkeby.infura.io",
                                                 new BlockchainKey("", key));
      _electorateDB = new ElectorateDB(_electorate, connectionString);

    }

    
    // POST api/values
    [HttpPost]
    public  void Post([FromBody]BallotVM value)
    {
     
        var key = new BlockchainKey(value.PublicKey, value.PrivateKey);
        var organisations =  _electorate.CreateBallot(key, "Ballot", _ballotVersion, value.Name, DateTime.UtcNow.AddDays(7.0));
        var result = organisations.Result;
    }



    [HttpGet()]
    public IEnumerable<BallotVM> Get()
    {
      List<BallotVM> ballotVMs = new List<BallotVM>();
      var organisations = _electorateDB.GetOrganisations();
      foreach (OrganisationDTO organisation in organisations)
      {
        var key = new BlockchainKey(organisation.PublicKey, organisation.PrivateKey);
        Ballot[] ballots = _electorate.Ballots(key);
        foreach (Ballot ballot in ballots)
        {
          BallotVM ballotVM  = new BallotVM();
          ballotVM.PrivateKey = organisation.PrivateKey;
          ballotVM.PublicKey = organisation.PublicKey;
          ballotVM.OrganisationName = organisation.Name;
          ballotVM.Name = ballot.Name;
          ballotVM.Address = ballot.Address.Address;         
          ballotVMs.Add(ballotVM);
        }
      }

      return ballotVMs;

    }



    [HttpGet("Result")]
    public IEnumerable<BallotVM> Result()
    {
      List<BallotVM> ballotVMs = new List<BallotVM>();
      var organisations = _electorateDB.GetOrganisations();
     
      foreach (OrganisationDTO organisation in organisations)
      {
        var key = new BlockchainKey(organisation.PublicKey, organisation.PrivateKey);
        Ballot[] ballots = _electorate.Ballots(key);
        foreach (Ballot ballot in ballots)
        {
          BallotVM ballotVM = new BallotVM();
          ballotVM.PrivateKey = organisation.PrivateKey;
          ballotVM.PublicKey = organisation.PublicKey;
          ballotVM.OrganisationName = organisation.Name;
          ballotVM.Name = ballot.Name;
          ballotVM.Address = ballot.Address.Address;
          ballotVM.Result = new List<CandidateVM>();

          var result = ballot.OptionsCount().Result;
          for (uint i = 0; i < result ; ++i)
          {
            BallotOption option = ballot.Option(i).Result;
            var votes = option.Votes().Result;
            var candidate = new CandidateVM();
            candidate.Name = option.Name;
            candidate.Result = votes;
            ballotVM.Result.Add(candidate);
          }

          ballotVMs.Add(ballotVM);
        }
      }

      return ballotVMs;

    }



    [HttpPost("BallotsOfOrganisation")]
    public IEnumerable<BallotVM> BallotsOfOrganisation([FromBody]OrganisationVM value)
    {
      List<BallotVM> ballotVMs = new List<BallotVM>();
      var key = new BlockchainKey(value.PublicKey, value.PrivateKey);     
      Ballot[] ballots = _electorate.Ballots(key);
      foreach(Ballot ballot in ballots)
      {
        BallotVM ballotVM = new BallotVM();
        ballotVM.Name = ballot.Name;
        ballotVM.Address = ballot.Address.Address;
        ballotVM.PrivateKey = value.PrivateKey;
        ballotVM.PublicKey = value.PublicKey;        
        ballotVMs.Add(ballotVM);
      }

      return ballotVMs;
    }



    [HttpPost("LockBallot")]
    public void LockBallot([FromBody]BallotVM value)
    {
      List<BallotVM> ballotVMs = new List<BallotVM>();
      var key = new BlockchainKey(value.PublicKey, value.PrivateKey);
      BlockchainAddress ballotAddress = new BlockchainAddress(value.Address);
      var result =  _electorate.LockBallotOptions(key, ballotAddress).Result;

    }


    [HttpPost("BallotsOfVoter")]
    public IEnumerable<BallotVM> BallotsOfVoter([FromBody]VoterVM value)
    {
      List<BallotVM> ballotVMs = new List<BallotVM>();
      var organisations = _electorateDB.GetOrganisations();
      foreach (OrganisationDTO organisation in organisations)
      {
        var key = new BlockchainKey(organisation.PublicKey, organisation.PrivateKey);
        Ballot[] ballots = _electorate.Ballots(key);
        foreach (Ballot ballot in ballots)
        {
          BallotVM ballotVM = new BallotVM();
          ballotVM.PrivateKey = organisation.PrivateKey;
          ballotVM.PublicKey = organisation.PublicKey;
          ballotVM.OrganisationName = organisation.Name;
          ballotVM.Name = ballot.Name;
          ballotVM.Address = ballot.Address.Address;

          VoterAccount voterAccount = _electorate.VoterAccount(value.Id, ballot.Address);

          //int[] ballotAllocatedVoters = _electorate.BallotAllocatedVoters(organisation.Key, ballot.Address);
          if(voterAccount != null)
          {
            ballotVMs.Add(ballotVM);
          }
        }
      }

      return ballotVMs;

    }



  }
}
