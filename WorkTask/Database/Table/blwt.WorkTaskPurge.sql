CREATE TABLE [blwt].[WorkTaskPurge]
(
	[PurgeId] UNIQUEIDENTIFIER NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[Status] SMALLINT CONSTRAINT [DF_WorkTaskPurge_Status] DEFAULT (0) NOT NULL,
	[TargetId] UNIQUEIDENTIFIER NOT NULL,
	[ExpirationTimestamp] DATETIME2(4) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_WorkTaskPurge_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_WorkTaskPurge_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_ExceptionPurge] PRIMARY KEY NONCLUSTERED ([PurgeId])
)

GO

CREATE CLUSTERED INDEX [IX_WorkTaskPurge_DomainId] ON [blwt].[WorkTaskPurge] ([DomainId]);

GO

CREATE UNIQUE INDEX [IX_WorkTaskPurge_TargetId] ON [blwt].[WorkTaskPurge] ([TargetId])
