CREATE TABLE [blad].[Phone]
(
	[PhoneId] UNIQUEIDENTIFIER NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[KeyId] UNIQUEIDENTIFIER NOT NULL,
	[Hash] BINARY(64) NOT NULL,
	[InitializationVector] BINARY(16) NOT NULL,
	Number VARBINARY(4000) NULL,
	CountryCode VARCHAR(8) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_Phone_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_Phone] PRIMARY KEY NONCLUSTERED ([PhoneId])
)

GO

CREATE CLUSTERED INDEX [IX_Phone_DomainId] ON [blad].[Phone] ([DomainId])
GO

CREATE NONCLUSTERED INDEX [IX_Phone_Hash] ON [blad].[Phone] ([DomainId], [Hash])
