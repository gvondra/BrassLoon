CREATE PROCEDURE [blwt].[GetWorkTaskStatus_by_DomainId]
	@domainId UNIQUEIDENTIFIER
AS
SELECT [WorkTaskStatusId], [WorkTaskTypeId], [DomainId], 
	[Code], [Name], [Description], [IsDefaultStatus], [IsClosedStatus], 
	[CreateTimestamp], [UpdateTimestamp],
	(SELECT COUNT(1) FROM [blwt].[WorkTask] WITH(READUNCOMMITTED) WHERE [DomainId] = [blwt].[WorkTaskStatus].[DomainId] AND [WorkTaskStatusId] = [blwt].[WorkTaskStatus].[WorkTaskStatusId]) [WorkTaskCount]
FROM [blwt].[WorkTaskStatus]
WHERE [DomainId] = @domainId
;