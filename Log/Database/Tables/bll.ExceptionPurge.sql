CREATE TABLE [bll].[ExceptionPurge]
(
	[PurgeId] BIGINT IDENTITY(1,1) NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[Status] SMALLINT NOT NULL CONSTRAINT [DF_ExceptionPurge_Status] DEFAULT (0),
	[TargetId] BIGINT NOT NULL,
	[ExpirationTimestamp] DATETIME2(4) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_ExceptionPurge_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_ExceptionPurge_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_ExceptionPurge] PRIMARY KEY NONCLUSTERED ([PurgeId])
)
WITH (DATA_COMPRESSION = PAGE)

GO

CREATE CLUSTERED INDEX [IX_ExceptionPurge_DomainId] ON [bll].[ExceptionPurge] ([DomainId]);

GO

CREATE UNIQUE INDEX [IX_ExceptionPurge_TargetId] ON [bll].[ExceptionPurge] ([TargetId])
