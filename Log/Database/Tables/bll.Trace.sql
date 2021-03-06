﻿CREATE TABLE [bll].[Trace]
(
	[TraceId] BIGINT IDENTITY(1,1) NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[EventCode] VARCHAR(200) NOT NULL,
	[Message] NVARCHAR(2000) NOT NULL,
	[Data] NVARCHAR(MAX) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_Trace_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_Trace] PRIMARY KEY CLUSTERED ([TraceId])
)
WITH (DATA_COMPRESSION = PAGE)

GO

CREATE INDEX [IX_Trace_DomainId] ON [bll].[Trace] ([DomainId], [EventCode]) INCLUDE ([CreateTimestamp])
