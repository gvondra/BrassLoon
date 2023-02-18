CREATE PROCEDURE [blwt].[GetWorkTaskType_by_DomainId]
	@domainId UNIQUEIDENTIFIER
AS
SELECT [WorkTaskTypeId], [DomainId], [Title], [Description], [CreateTimestamp], [UpdateTimestamp],
	(SELECT COUNT(1) FROM [blwt].[WorkTask] WITH(READUNCOMMITTED) WHERE [DomainId] = @domainId AND [WorkTaskTypeId] = [blwt].[WorkTaskType].[WorkTaskTypeId]) [WorkTaskCount]
FROM [blwt].[WorkTaskType] 
WHERE [DomainId] = @domainId
ORDER BY [Title], [CreateTimestamp]
;