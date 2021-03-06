﻿CREATE TABLE [bll].[Metric]
(
	[MetricId] BIGINT IDENTITY(1,1) NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[EventCode] VARCHAR(200) NOT NULL,
	[Magnitude] FLOAT NULL, -- in seconds
	[Data] NVARCHAR(MAX) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_Metric_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_Metric] PRIMARY KEY CLUSTERED ([MetricId])
)
WITH (DATA_COMPRESSION = PAGE)

GO

CREATE INDEX [IX_Metric_DomainId] ON [bll].[Metric] ([DomainId], [EventCode]) INCLUDE ([CreateTimestamp])
