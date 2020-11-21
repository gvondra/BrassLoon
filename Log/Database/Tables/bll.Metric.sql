﻿CREATE TABLE [bll].[Metric]
(
	[MetricId] BIGINT IDENTITY(1,1) NOT NULL,
	[DomainId] VARCHAR(32) NOT NULL,
	[EventCode] VARCHAR(200) NOT NULL,
	[Magnitude] FLOAT NULL, -- in seconds
	[Data] NVARCHAR(MAX) NOT NULL,
	[CreateTimestamp] DATETIME2(4) DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_Metric] PRIMARY KEY CLUSTERED ([MetricId])
)
