CREATE TABLE [bla].[UserInvitation]
(
	[UserInvitationId] UNIQUEIDENTIFIER CONSTRAINT [DF_UserInvitation_Id] DEFAULT(NEWID()) NOT NULL,
	[AccountGuid] UNIQUEIDENTIFIER NOT NULL,
	[EmailAddressGuid] UNIQUEIDENTIFIER NOT NULL,
	[Status] SMALLINT NOT NULL CONSTRAINT [DF_UserInvitation_Status] DEFAULT (0),
	[ExpirationTimestamp] DATETIME2(4) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_UserInvitation_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_UserInvitation_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_UserInvitation] PRIMARY KEY CLUSTERED ([UserInvitationId])
)
