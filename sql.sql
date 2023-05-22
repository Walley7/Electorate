-- Operations
select * from EthereumContractSource
select * from Voter
select * from Ballot
select * from Organisation
select * from BallotVoterAllocation
select * from VoterTokenSignature
select * from BallotVoterRegistration
select * from VoterToken
select * from VoterAccount

delete from EthereumContractSource
delete from VoterTokenSignature
delete from BallotVoterRegistration
delete from VoterToken
delete from VoterAccount
delete from BallotVoterAllocation
delete from Voter
delete from Ballot
delete from Organisation

drop table EthereumContractSource
drop table VoterTokenSignature
drop table BallotVoterRegistration
drop table VoterToken
drop table VoterAccount
drop table BallotVoterAllocation
drop table Voter
drop table Ballot
drop table Organisation


-- System / organisation / ballot side
create table EthereumContractSource(
    Contract nvarchar(256),
    Version nvarchar(32),
    ABI ntext not null,
    Bytecode ntext not null,
    primary key (Contract, Version)
);

create table Organisation(
    PublicKey varbinary(512) primary key not null,
    PrivateKey varbinary(512) not null,
	Funding int not null,
        [Name] [nvarchar](100) NULL,
	[RegistrationNo] [nvarchar](100) NULL,
	[Address] [nvarchar](1000) NULL
);

create table Ballot(
    Address varchar(512) primary key,
    OrganisationPublicKey varbinary(512) foreign key references Organisation(PublicKey) not null,
    Name varchar(256) not null unique,
    ABI text,
    RSAKey text,
    VoterFundAmount decimal(32, 16) not null
);

create table Voter(
    ID int identity(1, 1) primary key,
    OrganisationPublicKey varbinary(512) foreign key references Organisation(PublicKey) not null,
    Name nvarchar(256) not null
);

create table BallotVoterAllocation(
    BallotAddress varchar(256),
    VoterID int,
    primary key (BallotAddress, VoterID)
);

create table VoterTokenSignature(
    BallotAddress varchar(256),
    VoterID int,
	BlindedTokenHash varbinary(max) not null,
    primary key (BallotAddress, VoterID)
);

create table BallotVoterRegistration(
    BallotAddress varchar(256),
    VoterAddress varchar(256),
    SignedTokenHash varbinary(max) not null,
    primary key (BallotAddress, VoterAddress)
)


-- Voter side
create table VoterToken(
    VoterID int,
    BallotAddress varchar(256),
	RSAKeyModulus varbinary(max) not null,
	RSAKeyExponent varbinary(max) not null,
    Token varbinary(max) not null,
    BlindedToken varbinary(max) not null,
	BlindingFactor varbinary(max) not null,
    SignedToken varbinary(max),
    primary key (VoterID, BallotAddress)
);

create table VoterAccount(
    VoterID int,
    BallotAddress varchar(256),
    Address varchar(256) not null,
    PublicKey varbinary(512) not null,
    PrivateKey varbinary(512) not null,
    Registered bit,
    primary key (VoterID, BallotAddress)
);
