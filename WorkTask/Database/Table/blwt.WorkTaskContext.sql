CREATE TABLE [blwt].[WorkTaskContext]
(
	[WorkTaskContextId] UNIQUEIDENTIFIER NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[WorkTaskId] UNIQUEIDENTIFIER NOT NULL,
	[Status] SMALLINT NOT NULL,
	[ReferenceType] SMALLINT NOT NULL,
	[ReferenceValue] NVARCHAR(2048) NOT NULL,
	[ReferenceValueHash] BINARY(64) NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_WorkTaskContext_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_WorkTaskContext] PRIMARY KEY NONCLUSTERED ([WorkTaskContextId]), 
    CONSTRAINT [FK_WorkTaskContext_To_WorkTask] FOREIGN KEY ([WorkTaskId]) REFERENCES [blwt].[WorkTask]([WorkTaskId])
)

GO

CREATE INDEX [IX_WorkTaskContext_DomainId] ON [blwt].[WorkTaskContext] ([DomainId])

GO

CREATE INDEX [IX_WorkTaskContext_WorkTaskId] ON [blwt].[WorkTaskContext] ([WorkTaskId])

GO

CREATE NONCLUSTERED INDEX [IX_WorkTaskContext_DomainId_ReferenceType_ReferenceValueHash] ON [blwt].[WorkTaskContext] ([DomainId], [ReferenceType], [ReferenceValueHash])
	WHERE ([ReferenceValueHash] IS NOT NULL)
