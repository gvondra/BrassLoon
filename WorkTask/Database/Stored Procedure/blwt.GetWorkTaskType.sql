CREATE PROCEDURE [blwt].[GetWorkTaskType]
	@id UNIQUEIDENTIFIER
AS
BEGIN
SELECT TOP 1 [WorkTaskTypeId], [DomainId], [Code], [Title], [Description], [PurgePeriod],
	[CreateTimestamp], [UpdateTimestamp],
	(SELECT COUNT(1) FROM [blwt].[WorkTask] WITH(READUNCOMMITTED) WHERE [DomainId] = [blwt].[WorkTaskType].[DomainId] AND [WorkTaskTypeId] = [blwt].[WorkTaskType].[WorkTaskTypeId]) [WorkTaskCount]
FROM [blwt].[WorkTaskType] 
WHERE [WorkTaskTypeId] = @id
;
EXEC [blwt].[GetWorkTaskStatus_by_WorkTaskTypeId] @id;
END