CREATE TABLE [bla].[EmailAddress]
(
	[EmailAddressGuid] UNIQUEIDENTIFIER CONSTRAINT [DF_EmailAddress_Id] DEFAULT(NEWID()) NOT NULL,
	[Address] NVARCHAR(2000) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_EmailAddress_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_EmailAddress] PRIMARY KEY CLUSTERED ([EmailAddressGuid])
)
WITH (DATA_COMPRESSION = PAGE)

GO

CREATE UNIQUE INDEX [IX_EmailAddress_Address] ON [bla].[EmailAddress] ([Address])
