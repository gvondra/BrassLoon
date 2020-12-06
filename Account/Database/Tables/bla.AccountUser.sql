CREATE TABLE [bla].[AccountUser]
(
	[AccountGuid] UNIQUEIDENTIFIER NOT NULL,
	[UserGuid] UNIQUEIDENTIFIER NOT NULL,
	[IsActive] BIT CONSTRAINT [DF_AccountUser_IsActive] DEFAULT(1) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_AccountUser_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_AccountUser_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_AccountUser] PRIMARY KEY CLUSTERED ([AccountGuid], [UserGuid]), 
    CONSTRAINT [FK_AccountUser_To_Account] FOREIGN KEY ([AccountGuid]) REFERENCES [bla].[Account]([AccountGuid]), 
    CONSTRAINT [FK_AccountUser_To_User] FOREIGN KEY ([UserGuid]) REFERENCES [bla].[User]([UserGuid]) 
)

GO

CREATE INDEX [IX_AccountUser_AccountGuid] ON [bla].[AccountUser] ([AccountGuid])

GO

CREATE INDEX [IX_AccountUser_UserGuid] ON [bla].[AccountUser] ([UserGuid])
