CREATE TABLE [bla].[User]
(
	[UserGuid] UNIQUEIDENTIFIER CONSTRAINT [DF_User_Id] DEFAULT(NEWID()) NOT NULL,
	[ReferenceId] VARCHAR(512) NOT NULL,
	[Name] NVARCHAR(512) NOT NULL,
	[EmailAddressGuid] UNIQUEIDENTIFIER NOT NULL,
	[Roles] SMALLINT NOT NULL CONSTRAINT [DF_User_Roles] DEFAULT (0),
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_User_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_User_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserGuid]), 
    CONSTRAINT [FK_User_To_EmailAddress] FOREIGN KEY ([EmailAddressGuid]) REFERENCES [bla].[EmailAddress]([EmailAddressGuid])
)

GO

CREATE UNIQUE INDEX [IX_User_ReferenceId] ON [bla].[User] ([ReferenceId])

GO

CREATE INDEX [IX_User_EmailAddressGuid] ON [bla].[User] ([EmailAddressGuid])
