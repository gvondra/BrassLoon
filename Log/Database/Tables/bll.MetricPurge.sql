CREATE TABLE [bll].[MetricPurge]
(
	[PurgeId] BIGINT IDENTITY(1,1) NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[Status] SMALLINT NOT NULL CONSTRAINT [DF_MerticPurge_Status] DEFAULT (0),
	[TargetId] BIGINT NOT NULL,
	[ExpirationTimestamp] DATETIME2(4) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_MerticPurge_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_MetricPurge_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_MerticPurge] PRIMARY KEY NONCLUSTERED ([PurgeId])
)

GO

CREATE CLUSTERED INDEX [IX_MerticPurge_DomainId] ON [bll].[MetricPurge] ([DomainId]);

GO

CREATE UNIQUE INDEX [IX_MetricPurge_TargetId] ON [bll].[MetricPurge] ([TargetId])
