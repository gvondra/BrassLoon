CREATE PROCEDURE [blwt].[GetWorkGroupMember_by_DomainId]
	@domainId UNIQUEIDENTIFIER
AS
SELECT [WorkGroupMemberId], [WorkGroupId], [DomainId], [UserId], [CreateTimestamp]
FROM [blwt].[WorkGroupMember]
WHERE [DomainId] = @domainId
ORDER BY [CreateTimestamp]
;