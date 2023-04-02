CREATE PROCEDURE [blwt].[GetWorkTaskTypeGroup_by_DomainId]
	@domainId UNIQUEIDENTIFIER
AS
SELECT [WorkTaskTypeId], [WorkGroupId]
FROM [blwt].[WorkTaskTypeGroup]
WHERE EXISTS (
	SELECT TOP 1 1 
	FROM [blwt].[WorkTaskType] [wtt] 
	WHERE [wtt].[WorkTaskTypeId] = [blwt].[WorkTaskTypeGroup].[WorkTaskTypeId]
	AND [wtt].[DomainId] = @domainId
)
AND EXISTS (
	SELECT TOP 1 1 
	FROM [blwt].[WorkGroup] [wg] 
	WHERE [wg].[WorkGroupId] = [blwt].[WorkTaskTypeGroup].[WorkGroupId]
	AND [wg].[DomainId] = @domainId
)
;