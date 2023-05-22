using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Electorate;
using Electorate.Blockchain;
using ElectorateDataExt;
using ElectorateDataExt.DTO;
using ElectorateWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ElectorateWeb.Controllers
{
  [Route("api/[controller]")]
  public class OrganisationController : Controller
  {
    private readonly IConfiguration _configuration;
    ElectorateInstance _electorate;
    ElectorateDB _electorateDB;

    public OrganisationController(IConfiguration configuration)
    {
      _configuration = configuration;
      var connectionString = _configuration.GetValue<string>("ConnectionStrings:VotingDatabase");
      var key = _configuration.GetValue<string>("BlockChainSettings:BlockChainPrivateKey");

      _electorate = new ElectorateInstance(connectionString, ElectorateInstance.BlockchainType.ETHEREUM, "https://rinkeby.infura.io",
                                                 new BlockchainKey("", key));
      _electorateDB = new ElectorateDB(_electorate, connectionString);
    }

    // GET api/values
    [HttpGet]
    public IEnumerable<OrganisationVM> Get()
    {
      var organisations = _electorateDB.GetOrganisations();
      var vmData = organisations.Select(t => new OrganisationVM()
      {
        PrivateKey = t.PrivateKey,
        PublicKey = t.PublicKey,
        Address = t.Address,
        Funding = t.Funding,
        Name = t.Name,
        RegistrationNo = t.RegistrationNo

      }).ToList();

      return vmData;

    }


    [HttpGet("OrganisationsWithBallot")]
    public IEnumerable<OrganisationVM> OrganisationsWithBallot()
    {
      List<OrganisationVM> organisationVMs = new List<OrganisationVM>();
      var organisations = _electorateDB.GetOrganisations();
      foreach (OrganisationDTO organisation in organisations)
      {
        var key = new BlockchainKey(organisation.PublicKey, organisation.PrivateKey);
        Ballot[] ballots = _electorate.Ballots(key);
        if (ballots?.Length > 0)
        {
          OrganisationVM organisationVM = new OrganisationVM();
          organisationVM.PrivateKey = organisation.PrivateKey;
          organisationVM.PublicKey = organisation.PublicKey;
          organisationVM.Name = organisation.Name;
          organisationVM.RegistrationNo = organisation.RegistrationNo;
          organisationVMs.Add(organisationVM);
        }
      }

      return organisationVMs;

    }

    // POST api/values
    [HttpPost]
    public void Post([FromBody]OrganisationVM value)
    {
      var organisations = _electorateDB.CreateOrganisation(value.Funding, value.Name, value.RegistrationNo, value.Address);
    }

    

  }
}
