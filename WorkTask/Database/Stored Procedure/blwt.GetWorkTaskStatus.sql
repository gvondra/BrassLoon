CREATE PROCEDURE [blwt].[GetWorkTaskStatus]
	@id UNIQUEIDENTIFIER
AS
SELECT [WorkTaskStatusId], [WorkTaskTypeId], [DomainId], 
	[Code], [Name], [Description], [IsDefaultStatus], [IsClosedStatus], 
	[CreateTimestamp], [UpdateTimestamp]
FROM [blwt].[WorkTaskStatus]
WHERE [WorkTaskStatusId] = @id
;