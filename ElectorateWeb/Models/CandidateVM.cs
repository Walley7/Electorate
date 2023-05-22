using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectorateWeb.Models
{
  public class CandidateVM
  {
    public string CandidateKey { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public string BallotAddresskey { get; set; }    
    public string PublicKey { get; set; }
    public string PrivateKey { get; set; }
    public string OrganisationName { get; set; }
    public string BallotName { get; set; }
    public uint Index { get; set; }
    public bool Vote { get; set; }
    public int UserId { get; set; }
    public uint Result { get; set; }
  }
}
