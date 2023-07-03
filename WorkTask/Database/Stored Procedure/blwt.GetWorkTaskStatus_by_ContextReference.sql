CREATE PROCEDURE [blwt].[GetWorkTaskStatus_by_ContextReference]
	@domainId UNIQUEIDENTIFIER,
	@referenceType SMALLINT,
	@referenceValueHash VARBINARY(64)
AS
SELECT [WorkTaskStatusId], [WorkTaskTypeId], [DomainId], 
	[Code], [Name], [Description], [IsDefaultStatus], [IsClosedStatus], 
	[CreateTimestamp], [UpdateTimestamp],
	(SELECT COUNT(1) FROM [blwt].[WorkTask] WITH(READUNCOMMITTED) WHERE [DomainId] = [blwt].[WorkTaskStatus].[DomainId] AND [WorkTaskStatusId] = [blwt].[WorkTaskStatus].[WorkTaskStatusId]) [WorkTaskCount]
FROM [blwt].[WorkTaskStatus]
WHERE [DomainId] = @domainId
	AND EXISTS (SELECT TOP 1 1 
	FROM [blwt].[WorkTask] [tsk]
	INNER JOIN [blwt].[WorkTaskContext] [wtc] ON [tsk].[WorkTaskId] = [wtc].[WorkTaskId]
	WHERE [tsk].[WorkTaskStatusId] = [blwt].[WorkTaskStatus].[WorkTaskStatusId]
	AND [tsk].[DomainId] = @domainId
	AND [wtc].[DomainId] = @domainId
	AND [wtc].[ReferenceType] = @referenceType
	AND [wtc].[ReferenceValueHash] = @referenceValueHash
);