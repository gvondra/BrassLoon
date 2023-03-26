CREATE PROCEDURE [blwt].[DeleteWorkTaskStatus]
	@id UNIQUEIDENTIFIER
AS
DELETE FROM [blwt].[WorkTaskStatus]
WHERE [WorkTaskStatusId] = @id 
AND NOT EXISTS (
SELECT TOP 1 1 
FROM [blwt].[WorkTask] [wt]
WHERE [wt].[DomainId] = [blwt].[WorkTaskStatus].[DomainId]
AND [wt].[WorkTaskStatusId] = [blwt].[WorkTaskStatus].[WorkTaskStatusId]
)