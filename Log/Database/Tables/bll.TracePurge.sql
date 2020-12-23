CREATE TABLE [bll].[TracePurge]
(
	[PurgeId] BIGINT IDENTITY(1,1) NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[Status] SMALLINT NOT NULL CONSTRAINT [DF_TracePurge_Status] DEFAULT (0),
	[TargetId] BIGINT NOT NULL,
	[ExpirationTimestamp] DATETIME2(4) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_TracePurge_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_TracePurge_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_TracePurge] PRIMARY KEY CLUSTERED ([PurgeId])
)
WITH (DATA_COMPRESSION = PAGE)

GO

CREATE INDEX [IX_TracePurge_DomainId] ON [bll].[TracePurge] ([DomainId]) INCLUDE ([Status]);

GO

CREATE UNIQUE INDEX [IX_TracePurge_TargetId] ON [bll].[TracePurge] ([TargetId])
