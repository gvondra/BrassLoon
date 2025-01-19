CREATE PROCEDURE [blwt].[GetWorkTaskStatus_by_WorkTaskId]
	@workTaskId UNIQUEIDENTIFIER
AS
SELECT TOP 1 [sts].[WorkTaskStatusId], [sts].[WorkTaskTypeId], [sts].[DomainId], 
	[sts].[Code], [sts].[Name], [sts].[Description], [sts].[IsDefaultStatus], [sts].[IsClosedStatus], 
	[sts].[CreateTimestamp], [sts].[UpdateTimestamp],
	(SELECT COUNT(1) FROM [blwt].[WorkTask] WITH(READUNCOMMITTED) WHERE [DomainId] = [sts].[DomainId] AND [WorkTaskStatusId] = [sts].[WorkTaskStatusId]) [WorkTaskCount]
FROM [blwt].[WorkTaskStatus] [sts]
INNER JOIN [blwt].[WorkTask] [tsk] on [sts].[WorkTaskTypeId] = [tsk].[WorkTaskTypeId]
WHERE [tsk].[WorkTaskId] = @workTaskId
;