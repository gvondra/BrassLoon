CREATE TABLE [blt].[ClientCredential]
(
	[ClientCredentialId] UNIQUEIDENTIFIER NOT NULL,
	[ClientId] UNIQUEIDENTIFIER NOT NULL,
	[Secret] BINARY(64) NOT NULL,
	[IsActive] BIT CONSTRAINT [DF_ClientCredential_IsActive] DEFAULT (1) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_ClientCredential_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_ClientCredential_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_ClientCredential] PRIMARY KEY CLUSTERED ([ClientCredentialId]), 
    CONSTRAINT [FK_ClientCredential_To_Client] FOREIGN KEY ([ClientId]) REFERENCES [blt].[Client]([ClientId])
)

GO

CREATE INDEX [IX_ClientCredential_ClientId] ON [blt].[ClientCredential] ([ClientId])
