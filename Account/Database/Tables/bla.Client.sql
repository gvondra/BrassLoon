CREATE TABLE [bla].[Client]
(
	[ClientId] UNIQUEIDENTIFIER CONSTRAINT [DF_Client_Id] DEFAULT (NEWID()) NOT NULL,
	[AccountGuid] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(1024) CONSTRAINT [DF_Client_Name] DEFAULT ('') NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_Client_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_Client_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_Client] PRIMARY KEY NONCLUSTERED ([ClientId]), 
    CONSTRAINT [FK_Client_To_Account] FOREIGN KEY ([AccountGuid]) REFERENCES [bla].[Account]([AccountGuid])
)

GO

CREATE CLUSTERED INDEX [IX_Client_AccountGuid] ON [bla].[Client] ([AccountGuid])
