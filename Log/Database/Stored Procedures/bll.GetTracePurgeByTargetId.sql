CREATE PROCEDURE [bll].[GetTracePurgeByTargetId]
	@targetId BIGINT
AS
SELECT [PurgeId],[DomainId], [Status], [TargetId], [ExpirationTimestamp], [CreateTimestamp], [UpdateTimestamp]
FROM [bll].[TracePurge]
WHERE [TargetId] = @targetId
;