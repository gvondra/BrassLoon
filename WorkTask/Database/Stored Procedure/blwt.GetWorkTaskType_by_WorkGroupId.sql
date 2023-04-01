CREATE PROCEDURE [blwt].[GetWorkTaskType_by_WorkGroupId]
	@workGroupId UNIQUEIDENTIFIER
AS
SELECT [tp].[WorkTaskTypeId], [tp].[DomainId], [tp].[Code], [tp].[Title], [tp].[Description], [tp].[CreateTimestamp], [tp].[UpdateTimestamp],
	(SELECT COUNT(1) FROM [blwt].[WorkTask] WITH(READUNCOMMITTED) WHERE [DomainId] = [tp].[DomainId] AND [WorkTaskTypeId] = [tp].[WorkTaskTypeId]) [WorkTaskCount]
FROM [blwt].[WorkTaskType] [tp]
INNER JOIN [blwt].[WorkTaskTypeGroup] [grp] on [tp].[WorkTaskTypeId] = [grp].[WorkTaskTypeId]
WHERE [grp].[WorkGroupId] = @workGroupId
ORDER BY [tp].[Title], [tp].[CreateTimestamp]
;