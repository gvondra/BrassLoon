CREATE PROCEDURE [blwt].[GetWorkTaskStatus_by_WorkGroupId]
	@workGroupId UNIQUEIDENTIFIER
AS
SELECT [sts].[WorkTaskStatusId], [sts].[WorkTaskTypeId], [sts].[DomainId], 
	[sts].[Code], [sts].[Name], [sts].[Description], [sts].[IsDefaultStatus], [sts].[IsClosedStatus], 
	[sts].[CreateTimestamp], [sts].[UpdateTimestamp],
	(SELECT COUNT(1) FROM [blwt].[WorkTask] WITH(READUNCOMMITTED) WHERE [DomainId] = [sts].[DomainId] AND [WorkTaskStatusId] = [sts].[WorkTaskStatusId]) [WorkTaskCount]
FROM [blwt].[WorkTaskStatus] [sts]
WHERE EXISTS (
	SELECT TOP 1 1
	FROM [blwt].[WorkTask] [tsk]
	INNER JOIN [blwt].[WorkTaskTypeGroup] [typgrp] on [tsk].[WorkTaskTypeId] = [typgrp].[WorkTaskTypeId]
	WHERE [tsk].[WorkTaskStatusId] = [sts].[WorkTaskStatusId]
	AND [typgrp].[WorkGroupId] = @workGroupId
)
ORDER BY [sts].[Name], [sts].[CreateTimestamp]
;