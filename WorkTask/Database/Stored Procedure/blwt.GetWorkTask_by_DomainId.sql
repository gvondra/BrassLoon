CREATE PROCEDURE [blwt].[GetWorkTask_by_DomainId]
	@domainId UNIQUEIDENTIFIER
AS
BEGIN
	SELECT [WorkTaskId], [DomainId], [WorkTaskTypeId], [WorkTaskStatusId], [Title], [Text], [AssignedToUserId], [AssignedDate], [ClosedDate],
	[CreateTimestamp], [UpdateTimestamp]
	FROM [blwt].[WorkTask] 
	WHERE [DomainId] = @domainId
	ORDER BY [UpdateTimestamp] DESC
	;
END