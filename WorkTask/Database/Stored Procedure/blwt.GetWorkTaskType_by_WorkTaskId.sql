CREATE PROCEDURE [blwt].[GetWorkTaskType_by_WorkTaskId]
	@workTaskId UNIQUEIDENTIFIER
AS
BEGIN
SELECT TOP 1 [tp].[WorkTaskTypeId], [tp].[DomainId], [tp].[Code], [tp].[Title], [tp].[Description], [tp].[PurgePeriod],
	[tp].[CreateTimestamp], [tp].[UpdateTimestamp],
	(SELECT COUNT(1) FROM [blwt].[WorkTask] WITH(READUNCOMMITTED) WHERE [DomainId] = [tp].[DomainId] AND [WorkTaskTypeId] = [tp].[WorkTaskTypeId]) [WorkTaskCount]
FROM [blwt].[WorkTaskType] [tp]
INNER JOIN [blwt].[WorkTask] [tsk] on [tp].[WorkTaskTypeId] = [tsk].[WorkTaskTypeId]
WHERE [tsk].[WorkTaskId] = @workTaskId
;
EXEC [blwt].[GetWorkTaskStatus_by_WorkTaskId] @workTaskId;
END