CREATE PROCEDURE [blwt].[GetWorkTaskStatus_by_WorkTaskTypeId]
	@workTaskTypeId UNIQUEIDENTIFIER
AS
SELECT [WorkTaskStatusId], [WorkTaskTypeId], [DomainId], 
	[Code], [Name], [Description], [IsDefaultStatus], [IsClosedStatus], 
	[CreateTimestamp], [UpdateTimestamp]
FROM [blwt].[WorkTaskStatus]
WHERE [WorkTaskTypeId] = @workTaskTypeId
ORDER BY [Name], [CreateTimestamp]
;