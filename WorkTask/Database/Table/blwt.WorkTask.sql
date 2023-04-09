CREATE TABLE [blwt].[WorkTask]
(
	[WorkTaskId] UNIQUEIDENTIFIER NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[WorkTaskTypeId] UNIQUEIDENTIFIER NOT NULL,
	[WorkTaskStatusId] UNIQUEIDENTIFIER NOT NULL,
	[Title] NVARCHAR(512) NOT NULL,
	[Text] NVARCHAR(MAX) NOT NULL,
	[AssignedToUserId] VARCHAR(1024) CONSTRAINT [DF_WorkTask_AssignedToUserId] DEFAULT ('') NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_WorkTask_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_WorkTask_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_WorkTask] PRIMARY KEY NONCLUSTERED ([WorkTaskId]), 
    CONSTRAINT [FK_WorkTask_To_WorkTaskType] FOREIGN KEY ([WorkTaskTypeId]) REFERENCES [blwt].[WorkTaskType]([WorkTaskTypeId]), 
    CONSTRAINT [FK_WorkTask_To_WorkTaskStatus] FOREIGN KEY ([WorkTaskStatusId]) REFERENCES [blwt].[WorkTaskStatus]([WorkTaskStatusId]) 
)

GO

CREATE CLUSTERED INDEX [IX_WorkTask_DomainId] ON [blwt].[WorkTask] ([DomainId], [WorkTaskTypeId])

GO

CREATE INDEX [IX_WorkTask_WorkTaskStatusId] ON [blwt].[WorkTask] ([WorkTaskStatusId])
