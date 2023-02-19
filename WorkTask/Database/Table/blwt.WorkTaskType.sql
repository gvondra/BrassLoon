﻿CREATE TABLE [blwt].[WorkTaskType]
(
	[WorkTaskTypeId] UNIQUEIDENTIFIER NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[Title] NVARCHAR(512) NOT NULL,
	[Description] NVARCHAR(MAX) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_WorkTaskType_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_WorkTaskType_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_WorkTaskType] PRIMARY KEY NONCLUSTERED ([WorkTaskTypeId])
)

GO

CREATE CLUSTERED INDEX [IX_WorkTaskType_DomainId] ON [blwt].[WorkTaskType] ([DomainId])