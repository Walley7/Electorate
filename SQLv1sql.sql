
/****** Object:  Table [dbo].[Ballot]    Script Date: 29-10-2018 17:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Ballot](
	[Address] [varchar](512) NOT NULL,
	[OrganisationPublicKey] [varbinary](512) NOT NULL,
	[Name] [varchar](256) NOT NULL,
	[ABI] [text] NULL,
	[RSAKey] [text] NULL,
	[VoterFundAmount] [decimal](32, 16) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Address] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BallotOption]    Script Date: 29-10-2018 17:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BallotOption](
	[CandidateKey] [varchar](700) NOT NULL,
	[BallotKey] [varchar](700) NULL,
	[OrganisationPrivateKey] [varbinary](1000) NULL,
	[OrganisationPublicKey] [varbinary](512) NULL,
	[Name] [nvarchar](100) NULL,
	[Address] [nvarchar](1000) NULL,
 CONSTRAINT [PK_BallotOption] PRIMARY KEY CLUSTERED 
(
	[CandidateKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BallotVoterAllocation]    Script Date: 29-10-2018 17:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BallotVoterAllocation](
	[BallotAddress] [varchar](256) NOT NULL,
	[VoterID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BallotAddress] ASC,
	[VoterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[BallotVoterRegistration]    Script Date: 29-10-2018 17:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[BallotVoterRegistration](
	[BallotAddress] [varchar](256) NOT NULL,
	[VoterAddress] [varchar](256) NOT NULL,
	[SignedTokenHash] [varbinary](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BallotAddress] ASC,
	[VoterAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EthereumContractSource]    Script Date: 29-10-2018 17:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EthereumContractSource](
	[Contract] [nvarchar](256) NOT NULL,
	[Version] [nvarchar](32) NOT NULL,
	[ABI] [ntext] NOT NULL,
	[Bytecode] [ntext] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Contract] ASC,
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Organisation]    Script Date: 29-10-2018 17:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Organisation](
	[PublicKey] [varbinary](512) NOT NULL,
	[PrivateKey] [varbinary](512) NOT NULL,
	[Funding] [int] NOT NULL,
	[Name] [nvarchar](100) NULL,
	[RegistrationNo] [nvarchar](100) NULL,
	[Address] [nvarchar](1000) NULL,
PRIMARY KEY CLUSTERED 
(
	[PublicKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OrganisationData]    Script Date: 29-10-2018 17:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrganisationData](
	[PublicKey] [nvarchar](600) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[RegistrationNo] [nvarchar](100) NULL,
	[Address] [nvarchar](1000) NULL,
	[Funding] [nvarchar](50) NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Voter]    Script Date: 29-10-2018 17:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Voter](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[OrganisationPublicKey] [varbinary](512) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VoterAccount]    Script Date: 29-10-2018 17:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VoterAccount](
	[VoterID] [int] NOT NULL,
	[BallotAddress] [varchar](256) NOT NULL,
	[Address] [varchar](256) NOT NULL,
	[PublicKey] [varbinary](512) NOT NULL,
	[PrivateKey] [varbinary](512) NOT NULL,
	[Registered] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[VoterID] ASC,
	[BallotAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VoterToken]    Script Date: 29-10-2018 17:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VoterToken](
	[VoterID] [int] NOT NULL,
	[BallotAddress] [varchar](256) NOT NULL,
	[RSAKeyModulus] [varbinary](max) NOT NULL,
	[RSAKeyExponent] [varbinary](max) NOT NULL,
	[Token] [varbinary](max) NOT NULL,
	[BlindedToken] [varbinary](max) NOT NULL,
	[BlindingFactor] [varbinary](max) NOT NULL,
	[SignedToken] [varbinary](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[VoterID] ASC,
	[BallotAddress] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[VoterTokenSignature]    Script Date: 29-10-2018 17:34:12 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[VoterTokenSignature](
	[BallotAddress] [varchar](256) NOT NULL,
	[VoterID] [int] NOT NULL,
	[BlindedTokenHash] [varbinary](max) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BallotAddress] ASC,
	[VoterID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[Ballot]  WITH CHECK ADD FOREIGN KEY([OrganisationPublicKey])
REFERENCES [dbo].[Organisation] ([PublicKey])
GO
ALTER TABLE [dbo].[Voter]  WITH CHECK ADD FOREIGN KEY([OrganisationPublicKey])
REFERENCES [dbo].[Organisation] ([PublicKey])
GO
