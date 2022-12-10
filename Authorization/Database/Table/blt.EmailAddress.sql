CREATE TABLE [blt].[EmailAddress]
(
	[EmailAddressId] UNIQUEIDENTIFIER NOT NULL,
	[Address] NVARCHAR(2048) NOT NULL,
	[AddressHash] BINARY(32) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_EmailAddress_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_EmailAddress] PRIMARY KEY CLUSTERED ([EmailAddressId])
)
WITH (DATA_COMPRESSION = PAGE)

GO

CREATE UNIQUE INDEX [IX_EmailAddress_AddressHash] ON [blt].[EmailAddress] ([AddressHash])
