using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectorateWeb.Models
{
  public class VoterVM
  {
      public int Id { get; set; }
      public string PublicKey { get; set; }
      public string PrivateKey { get; set; }
      public string Name { get; set; }
      public string OrganisationName { get; set; }
      public string BallotAddresskey { get; set; }
      public bool IsAllocated { get; set; }
      

  }
}
