CREATE PROCEDURE [blwt].[GetWorkGroup_by_MemberUserId]
	@domainId UNIQUEIDENTIFIER,
	@userId VARCHAR(1024)
AS
BEGIN
	SELECT [wg].[WorkGroupId], [wg].[DomainId], [wg].[Title], [wg].[Description], [wg].[CreateTimestamp], [wg].[UpdateTimestamp]
	FROM [blwt].[WorkGroup] [wg]
	WHERE [wg].[DomainId] = @domainId
	AND EXISTS (
		SELECT TOP 1 1 
		FROM [blwt].[WorkGroupMember] [wgm]
		WHERE [wgm].[WorkGroupId] = [wg].[WorkGroupId]
		AND [wgm].[UserId] = @userId
	)
	ORDER BY [wg].[Title], [wg].[CreateTimestamp]
	;

	SELECT [wgm].[WorkGroupMemberId], [wgm].[WorkGroupId], [wgm].[DomainId], [wgm].[UserId], [wgm].[CreateTimestamp]
	FROM [blwt].[WorkGroupMember] [wgm]
	WHERE [DomainId] = @domainId
	AND EXISTS (
		SELECT TOP 1 1 
		FROM [blwt].[WorkGroupMember] [iwgm]
		WHERE [iwgm].[WorkGroupId] = [wgm].[WorkGroupId]
		AND [iwgm].[UserId] = @userId
	)
	ORDER BY [CreateTimestamp]
	;

	EXEC [blwt].[GetWorkTaskTypeGroup_by_DomainId] @domainId;
END