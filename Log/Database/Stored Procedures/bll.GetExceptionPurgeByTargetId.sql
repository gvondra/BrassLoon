CREATE PROCEDURE [bll].[GetExceptionPurgeByTargetId]
	@targetId BIGINT
AS
SELECT [PurgeId],[DomainId], [Status], [TargetId], [ExpirationTimestamp], [CreateTimestamp], [UpdateTimestamp]
FROM [bll].[ExceptionPurge]
WHERE [TargetId] = @targetId
;