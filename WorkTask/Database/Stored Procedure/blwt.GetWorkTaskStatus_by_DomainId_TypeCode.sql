CREATE PROCEDURE [blwt].[GetWorkTaskStatus_by_DomainId_TypeCode]
	@domainId UNIQUEIDENTIFIER,
	@code NVARCHAR(128)
AS
SELECT [WorkTaskStatusId], [WorkTaskTypeId], [DomainId], 
	[Code], [Name], [Description], [IsDefaultStatus], [IsClosedStatus], 
	[CreateTimestamp], [UpdateTimestamp],
	(SELECT COUNT(1) FROM [blwt].[WorkTask] WITH(READUNCOMMITTED) WHERE [DomainId] = [blwt].[WorkTaskStatus].[DomainId] AND [WorkTaskStatusId] = [blwt].[WorkTaskStatus].[WorkTaskStatusId]) [WorkTaskCount]
FROM [blwt].[WorkTaskStatus]
WHERE [DomainId] = @domainId
AND EXISTS (SELECT TOP 1 1
	FROM [blwt].[WorkTaskType] [wtt]
	WHERE [wtt].[WorkTaskTypeId] = [blwt].[WorkTaskStatus].[WorkTaskTypeId]
	AND [wtt].[DomainId] = @domainId
	AND [wtt].[Code] = @code)
;