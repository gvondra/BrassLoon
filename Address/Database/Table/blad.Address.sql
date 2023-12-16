CREATE TABLE [blad].[Address]
(
	[AddressId] UNIQUEIDENTIFIER NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[KeyId] UNIQUEIDENTIFIER NOT NULL,
	[Hash] BINARY(64) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_Address_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_Address] PRIMARY KEY NONCLUSTERED ([AddressId])
)

GO

CREATE CLUSTERED INDEX [IX_Address_DomainId] ON [blad].[Address] ([DomainId])

GO

CREATE INDEX [IX_Address_Hash] ON [blad].[Address] ([DomainId], [Hash])
