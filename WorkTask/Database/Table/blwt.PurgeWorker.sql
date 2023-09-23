CREATE TABLE [blwt].[PurgeWorker]
(
	[PurgeWorkerId] UNIQUEIDENTIFIER NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[Status] SMALLINT NOT NULL CONSTRAINT [DF_PurgeWorker_Status] DEFAULT (0),
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_PurgeWorker_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_PurgeWorker_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_PurgeWorker] PRIMARY KEY NONCLUSTERED ([PurgeWorkerId])
)

GO

CREATE CLUSTERED INDEX [IX_PurgeWorker_DomainId] ON [blwt].[PurgeWorker] ([DomainId])
