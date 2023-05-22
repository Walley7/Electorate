using Electorate;
using Electorate.Blockchain;
using ElectorateDataExt.DTO;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace ElectorateDataExt
{

    public class ElectorateDB
    {
        ElectorateInstance _electorate;
        string _databaseConnection;
        SqlConnection _sqlConnection;
        public ElectorateDB(ElectorateInstance electorateInstance, string databaseConnection)
        {
            this._electorate = electorateInstance;
            this._databaseConnection = databaseConnection;
            this._sqlConnection = new SqlConnection(this._databaseConnection);
            this._sqlConnection.Open();
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
                                                   "values (@CandidateKey, @BallotKey, @OrganisationPrivateKey, @OrganisationPublicKey, @Name, @Address)", _sqlConnection);
                command.Parameters.AddWithValue("@CandidateKey", OptoionKey);
                command.Parameters.AddWithValue("@BallotKey", ballotAddresskey);
                command.Parameters.AddWithValue("@OrganisationPrivateKey", organisationKey.Private);
                command.Parameters.AddWithValue("@OrganisationPublicKey", organisationKey.Public);
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

        public BlockchainKey CreateOrganisation(Organisation.FundingOption funding, string name, string registrationNo, string address)
        {

            BlockchainKey organisationKey = this._electorate.CreateOrganisation(funding);

            // Database
            SqlCommand command = new SqlCommand("insert into OrganisationData (PublicKey, Funding, Name, RegistrationNo, Address ) values (@PublicKey, @Funding, @Name, @RegistrationNo, @Address)", _sqlConnection);
            command.Parameters.AddWithValue("@PublicKey", organisationKey.PublicString);          
            command.Parameters.AddWithValue("@Funding", funding);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@RegistrationNo", registrationNo);
            command.Parameters.AddWithValue("@Address", address);
            command.ExecuteNonQuery();
            command.Dispose();

            // Return
            return organisationKey;
        }


        public List<OrganisationDTO> GetOrganisations()
        {
            // Organisation
            var OrganisationsList = new List<OrganisationDTO>();
            var organisations = _electorate.Organisations(true);
            if(organisations != null)
            {
                foreach (Organisation organisation in organisations)
                {
                    OrganisationDTO organisationDTO = new OrganisationDTO()
                    {
                        PrivateKey = organisation.Key.PrivateString,
                        PublicKey = organisation.Key.PublicString,
                        Funding = organisation.Funding.Value,
                    };

                    SqlCommand command = new SqlCommand("Select Name,RegistrationNo, Address from OrganisationData Where PublicKey = @PublicKey", _sqlConnection);
                    command.Parameters.AddWithValue("@PublicKey", organisation.Key.PublicString);
                    var reader = command.ExecuteReader();
                    reader.Read();
                    if(reader.HasRows)
                    {
                        organisationDTO.Name = reader["Name"]?.ToString();
                        organisationDTO.RegistrationNo = reader["RegistrationNo"]?.ToString();
                        organisationDTO.Address = reader["Address"]?.ToString();
                    }
                    reader.Close();

                    OrganisationsList.Add(organisationDTO);
                }

            }

            return OrganisationsList;

        }


    }
}
