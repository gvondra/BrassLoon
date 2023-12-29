CREATE TABLE [blad].[Address]
(
	[AddressId] UNIQUEIDENTIFIER NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[KeyId] UNIQUEIDENTIFIER NOT NULL,
	[Hash] BINARY(64) NOT NULL,
	[InitializationVector] BINARY(16) NOT NULL,
	[Attention] VARBINARY(8000) NULL,
	[Addressee] VARBINARY(8000) NULL,
	[Delivery] VARBINARY(8000) NULL,
	[Secondary] VARBINARY(8000) NULL,
	[City] VARBINARY(8000) NULL,
	[Territory] VARBINARY(4000) NULL,
	[PostalCode] VARBINARY(4000) NULL,
	[Country] VARBINARY(4000) NULL,
	[County] VARBINARY(8000) NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_Address_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_Address] PRIMARY KEY NONCLUSTERED ([AddressId])
)

GO

CREATE CLUSTERED INDEX [IX_Address_DomainId] ON [blad].[Address] ([DomainId])

GO

CREATE INDEX [IX_Address_Hash] ON [blad].[Address] ([DomainId], [Hash])
