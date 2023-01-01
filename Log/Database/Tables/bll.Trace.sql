CREATE TABLE [bll].[Trace]
(
	[TraceId] BIGINT IDENTITY(1,1) NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[EventCode] VARCHAR(200) NOT NULL,
	[Message] NVARCHAR(2000) NOT NULL,
	[Data] NVARCHAR(MAX) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_Trace_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[EventId] UNIQUEIDENTIFIER NULL,
	[Category] NVARCHAR(512) CONSTRAINT [DF_Trace_Category] DEFAULT ('') NOT NULL,
	[Level] NVARCHAR(512) CONSTRAINT [DF_Trace_Level] DEFAULT ('') NOT NULL,
	CONSTRAINT [PK_Trace] PRIMARY KEY NONCLUSTERED ([TraceId]), 
    CONSTRAINT [FK_Trace_To_EventId] FOREIGN KEY ([EventId]) REFERENCES [bll].[EventID]([EventId])
)
WITH (DATA_COMPRESSION = PAGE)

GO

CREATE CLUSTERED INDEX [IX_Trace_DomainId] ON [bll].[Trace] ([DomainId], [EventCode]);

GO

CREATE INDEX [IX_Trace_EventId] ON [bll].[Trace] ([EventId])
