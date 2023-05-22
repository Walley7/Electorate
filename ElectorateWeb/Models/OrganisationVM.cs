using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Electorate.Organisation;

namespace ElectorateWeb.Models
{
  public class OrganisationVM
  {
    public string PublicKey { get; set; }
    public string PrivateKey { get; set; }
    public FundingOption Funding { get; set; }

    public string Name { get; set; }
    public string RegistrationNo { get; set; }
    public string Address { get; set; }

  }

}
