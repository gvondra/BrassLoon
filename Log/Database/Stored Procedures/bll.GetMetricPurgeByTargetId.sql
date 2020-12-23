CREATE PROCEDURE [bll].[GetMetricPurgeByTargetId]
	@targetId BIGINT
AS
SELECT [PurgeId],[DomainId], [Status], [TargetId], [ExpirationTimestamp], [CreateTimestamp], [UpdateTimestamp]
FROM [bll].[MetricPurge]
WHERE [TargetId] = @targetId
;