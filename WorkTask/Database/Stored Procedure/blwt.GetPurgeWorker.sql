CREATE PROCEDURE [blwt].[GetPurgeWorker]
	@purgeWorkerId UNIQUEIDENTIFIER
AS
SELECT [PurgeWorkerId], [DomainId], [Status], [CreateTimestamp], [UpdateTimestamp]
FROM [blwt].[PurgeWorker] 
WHERE [PurgeWorkerId] = @purgeWorkerId
;