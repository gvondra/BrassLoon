﻿CREATE PROCEDURE [blwt].[GetWorkTask_by_ContextReference]
	@referenceType SMALLINT,
	@referenceValueHash VARBINARY(64),
	@includeClosed BIT = 0
AS
BEGIN
	SELECT [tsk].[WorkTaskId], [tsk].[DomainId], [tsk].[WorkTaskTypeId], [tsk].[WorkTaskStatusId], [tsk].[Title], 
	[tsk].[Text], [tsk].[AssignedToUserId], [tsk].[AssignedDate],
	[tsk].[CreateTimestamp], [tsk].[UpdateTimestamp]
	FROM [blwt].[WorkTask] [tsk] 
	WHERE EXISTS (
		SELECT TOP 1 1 
		FROM [blwt].[WorkTaskContext] [wtc]
		WHERE [wtc].[WorkTaskId] = [tsk].[WorkTaskId]
		AND [wtc].[ReferenceType] = @referenceType
		AND [wtc].[ReferenceValueHash] = @referenceValueHash
	)
	-- and include closed or the task's status is not a closed status
	AND (@includeClosed = 1
		OR EXISTS (
		SELECT TOP 1 1 
		FROM [blwt].[WorkTaskStatus] [sts] 
		WHERE [sts].[WorkTaskStatusId] = [tsk].[WorkTaskStatusId]
		AND [sts].[IsClosedStatus] = 0
	))
	ORDER BY [tsk].[AssignedDate], [tsk].[CreateTimestamp]
	;
	EXEC [blwt].[GetWorkTaskType_by_ContextReference] @referenceType, @referenceValueHash;
	EXEC [blwt].[GetWorkTaskStatus_by_ContextReference] @referenceType, @referenceValueHash;
	EXEC [blwt].[GetWorkTaskContext_by_Reference] @referenceType, @referenceValueHash;
END