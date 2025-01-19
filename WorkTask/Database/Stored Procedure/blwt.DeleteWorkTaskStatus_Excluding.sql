CREATE PROCEDURE [blwt].[DeleteWorkTaskStatus_Excluding]
	@ids VARCHAR(MAX)
AS
DELETE FROM [blwt].[WorkTaskStatus]
WHERE NOT EXISTS (SELECT TOP 1 1 FROM string_split(@ids, ',') WHERE CONVERT(UNIQUEIDENTIFIER, TRIM([value])) = [blwt].[WorkTaskStatus].[WorkTaskStatusId])
AND NOT EXISTS (
SELECT TOP 1 1 
FROM [blwt].[WorkTask] [wt]
WHERE [wt].[DomainId] = [blwt].[WorkTaskStatus].[DomainId]
AND [wt].[WorkTaskStatusId] = [blwt].[WorkTaskStatus].[WorkTaskStatusId]
)