CREATE PROCEDURE [bll].[GetPurgeWorker]
	@purgeWorkerId UNIQUEIDENTIFIER
AS
SELECT [PurgeWorkerId], [DomainId], [Status], [CreateTimestamp], [UpdateTimestamp]
FROM [bll].[PurgeWorker] 
WHERE [PurgeWorkerId] = @purgeWorkerId
;