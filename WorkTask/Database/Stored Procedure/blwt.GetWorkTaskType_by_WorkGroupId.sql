CREATE PROCEDURE [blwt].[GetWorkTaskType_by_WorkGroupId]
	@workGroupId UNIQUEIDENTIFIER
AS
BEGIN
SELECT [tp].[WorkTaskTypeId], [tp].[DomainId], [tp].[Code], [tp].[Title], [tp].[Description], [tp].[PurgePeriod],
	[tp].[CreateTimestamp], [tp].[UpdateTimestamp],
	(SELECT COUNT(1) FROM [blwt].[WorkTask] WITH(READUNCOMMITTED) WHERE [DomainId] = [tp].[DomainId] AND [WorkTaskTypeId] = [tp].[WorkTaskTypeId]) [WorkTaskCount]
FROM [blwt].[WorkTaskType] [tp]
INNER JOIN [blwt].[WorkTaskTypeGroup] [grp] on [tp].[WorkTaskTypeId] = [grp].[WorkTaskTypeId]
WHERE [grp].[WorkGroupId] = @workGroupId
ORDER BY [tp].[Title], [tp].[CreateTimestamp]
;

SELECT [sts].[WorkTaskStatusId], [sts].[WorkTaskTypeId], [sts].[DomainId], 
	[sts].[Code], [sts].[Name], [sts].[Description], [sts].[IsDefaultStatus], [sts].[IsClosedStatus], 
	[sts].[CreateTimestamp], [sts].[UpdateTimestamp],
	(SELECT COUNT(1) FROM [blwt].[WorkTask] WITH(READUNCOMMITTED) WHERE [DomainId] = [sts].[DomainId] AND [WorkTaskStatusId] = [sts].[WorkTaskStatusId]) [WorkTaskCount]
FROM [blwt].[WorkTaskStatus] [sts]
INNER JOIN [blwt].[WorkTaskTypeGroup] [grp] on [sts].[WorkTaskTypeId] = [grp].[WorkTaskTypeId]
WHERE [grp].[WorkGroupId] = @workGroupId
;
END