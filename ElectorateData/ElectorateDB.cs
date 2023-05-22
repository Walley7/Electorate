using Electorate;
using Electorate.Blockchain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ElectorateData
{

   public class ElectorateDB
    {
        ElectorateInstance _electorate;
        public ElectorateDB(ElectorateInstance electorateInstance)
        {
            this._electorate = electorateInstance;
        }


        public async Task<string> CreateCandidate(BlockchainKey organisationKey, string name, string ballotAddresskey, string address)
        {
            try
            {
                // Organisation
                Organisation organisation = _electorate.Organisation(organisationKey);
                if (!organisation.Key.IsPrivate)
                    throw new UnauthorizedAccessException();

                BlockchainAddress ballotAddress = new BlockchainAddress(ballotAddresskey);

                // Organisation funding
                await _electorate.FundOrganisationIfEligible(organisation.Key);

                // Deploy
                var result = await _electorate.AddBallotOption(organisationKey, ballotAddress, name);
                var OptoionKey = result.hash;

                // // Database
                SqlCommand command = new SqlCommand("insert into BallotOption (CandidateKey, BallotKey, OrganisationPrivateKey, OrganisationPublicKey, Name, Address) " +
                                                   "values (@CandidateKey, @BallotKey, @OrganisationPrivateKey, @OrganisationPublicKey, @Name, @Address)", DatabaseConnection);
                command.Parameters.AddWithValue("@CandidateKey", OptoionKey);
                command.Parameters.AddWithValue("@BallotKey", ballotAddresskey);
                command.Parameters.AddWithValue("@OrganisationPrivateKey", organisationKey.PrivateString);
                command.Parameters.AddWithValue("@OrganisationPublicKey", organisationKey.PublicString);
                command.Parameters.AddWithValue("@Name", name);
                command.Parameters.AddWithValue("@Address", address);
                command.ExecuteNonQuery();
                command.Dispose();
                return OptoionKey;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



    }
}
