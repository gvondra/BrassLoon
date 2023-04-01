CREATE TABLE [blwt].[WorkTaskTypeGroup]
(
	[WorkTaskTypeId] UNIQUEIDENTIFIER NOT NULL,
	[WorkGroupId] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT [PK_WorkTaskTypeGroup] PRIMARY KEY NONCLUSTERED ([WorkTaskTypeId], [WorkGroupId]), 
    CONSTRAINT [FK_WorkTaskTypeGroup_To_WorkTaskType] FOREIGN KEY ([WorkTaskTypeId]) REFERENCES [blwt].[WorkTaskType]([WorkTaskTypeId]), 
    CONSTRAINT [FK_WorkTaskTypeGroup_To_WorkGroup] FOREIGN KEY ([WorkGroupId]) REFERENCES [blwt].[WorkGroup]([WorkGroupId])
)

GO

CREATE INDEX [IX_WorkTaskTypeGroup_WorkTaskTypeId] ON [blwt].[WorkTaskTypeGroup] ([WorkTaskTypeId])

GO

CREATE INDEX [IX_WorkTaskTypeGroup_WorkGroupId] ON [blwt].[WorkTaskTypeGroup] ([WorkGroupId])
