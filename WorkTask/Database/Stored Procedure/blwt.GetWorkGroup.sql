CREATE PROCEDURE [blwt].[GetWorkGroup]
	@id UNIQUEIDENTIFIER
AS
BEGIN
	SELECT TOP 1 [WorkGroupId], [DomainId], [Title], [Description], [CreateTimestamp], [UpdateTimestamp]
	FROM [blwt].[WorkGroup]
	WHERE [WorkGroupId] = @id
	;
END