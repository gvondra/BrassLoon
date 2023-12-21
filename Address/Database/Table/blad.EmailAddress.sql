CREATE TABLE [blad].[EmailAddress]
(
	[EmailAddressId] UNIQUEIDENTIFIER NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[KeyId] UNIQUEIDENTIFIER NOT NULL,
	[Hash] BINARY(64) NOT NULL,
	[InitializationVector] BINARY(16) NOT NULL,
	[Address] VARBINARY(8000) NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_EmailAddress_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_EmailAddress] PRIMARY KEY NONCLUSTERED ([EmailAddressId])
)

GO

CREATE CLUSTERED INDEX [IX_EmailAddress_DomainId] ON [blad].[EmailAddress] ([DomainId])
GO

CREATE NONCLUSTERED INDEX [IX_EmailAddress_Hash] ON [blad].[EmailAddress] ([DomainId], [Hash])
