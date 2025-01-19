CREATE PROCEDURE [blwt].[GetWorkTask]
	@id UNIQUEIDENTIFIER
AS
BEGIN
	SELECT TOP 1 [WorkTaskId], [DomainId], [WorkTaskTypeId], [WorkTaskStatusId], [Title], [Text], [AssignedToUserId], [AssignedDate], [ClosedDate],
	[CreateTimestamp], [UpdateTimestamp]
	FROM [blwt].[WorkTask] 
	WHERE [WorkTaskId] = @id 
	;
	EXEC [blwt].[GetWorkTaskType_by_WorkTaskId] @id;	
	EXEC [blwt].[GetWorkTaskContext_by_WorkTaskId] @id;
END