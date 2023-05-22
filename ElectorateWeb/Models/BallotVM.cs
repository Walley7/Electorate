using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectorateWeb.Models
{
  public class BallotVM
  {
    public string Address { get; set; }
    public string PublicKey { get; set; }
    public string PrivateKey { get; set; }
    public string Name { get; set; }
    public string OrganisationName { get; set; }
    public string Version { get; set; }
    public List<CandidateVM> Result { get; set; }
  }
}
