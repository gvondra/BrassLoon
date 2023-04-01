CREATE TABLE [blwt].[WorkTaskStatus]
(
	[WorkTaskStatusId] UNIQUEIDENTIFIER NOT NULL,
	[WorkTaskTypeId] UNIQUEIDENTIFIER NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[Code] NVARCHAR(128) NOT NULL,
	[Name] NVARCHAR(128) NOT NULL,
	[Description] NVARCHAR(MAX) NOT NULL,
	[IsDefaultStatus] BIT CONSTRAINT [DF_WorkTaskStatus_IsDefaultStatus] DEFAULT 0 NOT NULL,
	[IsClosedStatus] BIT CONSTRAINT [DF_WorkTaskStatus_IsClosedStatus] DEFAULT 0 NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_WorkTaskStatus_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_WorkTaskStatus_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_WorkTaskStatus] PRIMARY KEY NONCLUSTERED ([WorkTaskStatusId]), 
    CONSTRAINT [FK_WorkTaskStatus_To_WorkTaskType] FOREIGN KEY ([WorkTaskTypeId]) REFERENCES [blwt].[WorkTaskType]([WorkTaskTypeId])
)

GO

CREATE CLUSTERED INDEX [IX_WorkTaskStatus_DomainId] ON [blwt].[WorkTaskStatus] ([DomainId])

GO

CREATE UNIQUE INDEX [IX_WorkTaskStatus_WorkTaskTypeId_ Code] ON [blwt].[WorkTaskStatus] ([WorkTaskTypeId], [Code])
