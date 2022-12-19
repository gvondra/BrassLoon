CREATE TABLE [blt].[UserRole]
(
	[UserId] UNIQUEIDENTIFIER NOT NULL,
	[RoleId] UNIQUEIDENTIFIER NOT NULL,
	[IsActive] BIT CONSTRAINT [DF_UserRole_IsActive] DEFAULT 1 NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_UserRole_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_UserRole_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_UserRole] PRIMARY KEY NONCLUSTERED ([UserId],[RoleId]), 
    CONSTRAINT [FK_UserRole_To_User] FOREIGN KEY ([UserId]) REFERENCES [blt].[User]([UserId]), 
    CONSTRAINT [FK_UserRole_To_Role] FOREIGN KEY ([RoleId]) REFERENCES [blt].[Role]([RoleId])
)

GO

CREATE INDEX [IX_UserRole_UserId] ON [blt].[UserRole] ([UserId])

GO

CREATE INDEX [IX_UserRole_RoleId] ON [blt].[UserRole] ([RoleId])
