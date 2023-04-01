CREATE PROCEDURE [blwt].[GetWorkGroup_by_DomainId]
	@domainId UNIQUEIDENTIFIER
AS
BEGIN
	SELECT [WorkGroupId], [DomainId], [Title], [Description], [CreateTimestamp], [UpdateTimestamp]
	FROM [blwt].[WorkGroup]
	WHERE [DomainId] = @domainId
	ORDER BY [Title], [CreateTimestamp]
	;

	EXEC [blwt].[GetWorkGroupMember_by_DomainId] @domainId;
END