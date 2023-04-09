CREATE PROCEDURE [blwt].[GetWorkTask_by_WorkGroupId]
	@workGroupId UNIQUEIDENTIFIER,
	@includeClosed BIT = 0
AS
BEGIN
	SELECT [tsk].[WorkTaskId], [tsk].[DomainId], [tsk].[WorkTaskTypeId], [tsk].[WorkTaskStatusId], [tsk].[Title], 
	[tsk].[Text], [tsk].[AssignedToUserId],
	[tsk].[CreateTimestamp], [tsk].[UpdateTimestamp]
	FROM [blwt].[WorkTask] [tsk]
	WHERE EXISTS (
		SELECT TOP 1 1 
		FROM [blwt].[WorkTaskTypeGroup] [typgrp] 
		WHERE [typgrp].[WorkTaskTypeId] = [tsk].[WorkTaskTypeId]
		AND [typgrp].[WorkGroupId] = @workGroupId
	)
	-- and include closed or the task's status is not a closed status
	AND (@includeClosed = 1
		OR EXISTS (
		SELECT TOP 1 1 
		FROM [blwt].[WorkTaskStatus] [sts] 
		WHERE [sts].[WorkTaskStatusId] = [tsk].[WorkTaskStatusId]
		AND [sts].[IsClosedStatus] = 0
	))
	ORDER BY [tsk].[CreateTimestamp] DESC
	;
	EXEC [blwt].[GetWorkTaskType_by_WorkGroupId] @workGroupId;
	EXEC [blwt].[GetWorkTaskStatus_by_WorkGroupId] @workGroupId;
	EXEC [blwt].[GetWorkTaskContext_by_WorkGroupId] @workGroupId;
END