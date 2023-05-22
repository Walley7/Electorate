
export class Organisation {
  publicKey: string;
  privateKey: string;
  funding: FundingOption;
  name: string;
  registrationNo: string;
  address: string;
}

export enum FundingOption {
  None = 0,
  SystemFunded = 1,
  SelfFunded = 2
}

export class Ballot {
  publicKey: string;
  privateKey: string;
  address: string;
  name: string;
  version: string;
  organisationName: string;
  result: Candidate[];
}

export class Candidate {

  candidateKey: string;
  name: string;
  address: string;
  ballotAddresskey: string;
  publicKey: string;
  privateKey: string;
  organisationName: string;
  ballotName: string;
  vote: Boolean;
  Index: number;
  userId: number;
  result: number;

}

export class Voter {
  Id: number;
  publicKey: string;
  privateKey: string;
  name: string;
  organisationName: string;
  ballotAddresskey: string;
  isAllocated: boolean;
}
