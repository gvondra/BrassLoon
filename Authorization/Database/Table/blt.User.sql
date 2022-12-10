CREATE TABLE [blt].[User]
(
	[UserId] UNIQUEIDENTIFIER NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[ReferenceId] VARCHAR(1024) NOT NULL,
	[EmailAddressId] UNIQUEIDENTIFIER NOT NULL,
	[Name] VARCHAR(1024) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_User_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_User_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_User] PRIMARY KEY NONCLUSTERED ([UserId]), 
    CONSTRAINT [FK_User_To_EmailAddress] FOREIGN KEY ([EmailAddressId]) REFERENCES [blt].[EmailAddress]([EmailAddressId])
)

GO

CREATE UNIQUE INDEX [IX_User_ReferenceId] ON [blt].[User] ([ReferenceId])

GO

CREATE UNIQUE INDEX [IX_User_EmailAddressId] ON [blt].[User] ([EmailAddressId])

GO

CREATE CLUSTERED INDEX [IX_User_DomainId] ON [blt].[User] ([DomainId])
