CREATE PROCEDURE [blwt].[GetWorkGroupMember_by_WorkGroupId]
	@workGroupId UNIQUEIDENTIFIER
AS
SELECT [WorkGroupMemberId], [WorkGroupId], [DomainId], [UserId], [CreateTimestamp]
FROM [blwt].[WorkGroupMember]
WHERE [WorkGroupId] = @workGroupId
ORDER BY [CreateTimestamp]
;