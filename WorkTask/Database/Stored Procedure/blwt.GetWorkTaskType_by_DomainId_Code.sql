CREATE PROCEDURE [blwt].[GetWorkTaskType_by_DomainId_Code]
	@domainId UNIQUEIDENTIFIER,
	@code NVARCHAR(128)
AS
BEGIN
SELECT [WorkTaskTypeId], [DomainId], [Code], [Title], [Description], [PurgePeriod],
	[CreateTimestamp], [UpdateTimestamp],
	(SELECT COUNT(1) FROM [blwt].[WorkTask] WITH(READUNCOMMITTED) WHERE [DomainId] = [blwt].[WorkTaskType].[DomainId] AND [WorkTaskTypeId] = [blwt].[WorkTaskType].[WorkTaskTypeId]) [WorkTaskCount]
FROM [blwt].[WorkTaskType] 
WHERE [DomainId] = @domainId
AND [Code] = @code
ORDER BY [Title], [CreateTimestamp]
;
EXEC [blwt].[GetWorkTaskStatus_by_DomainId_TypeCode] @domainId, @code;
END