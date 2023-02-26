CREATE PROCEDURE [blwt].[GetWorkTaskContext_by_WorkGroupId]
	@workGroupId UNIQUEIDENTIFIER
AS
SELECT [ctx].[WorkTaskContextId], [ctx].[DomainId], [ctx].[WorkTaskId], [ctx].[Status], [ctx].[ReferenceType], [ctx].[ReferenceValue], [ctx].[CreateTimestamp]
FROM [blwt].[WorkTaskContext] [ctx]
WHERE EXISTS (
	SELECT TOP 1 1 
	FROM [blwt].[WorkTask] [tsk]
	INNER JOIN [blwt].[WorkTaskTypeGroup] [typgrp] on [tsk].[WorkTaskTypeId] = [typgrp].[WorkTaskTypeId]
	WHERE [tsk].[WorkTaskId] = [ctx].[WorkTaskId]
	AND [typgrp].[WorkGroupId] = @workGroupId
)
ORDER BY [ctx].[CreateTimestamp]
;