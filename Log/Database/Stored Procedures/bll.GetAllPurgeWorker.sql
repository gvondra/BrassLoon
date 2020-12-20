CREATE PROCEDURE [bll].[GetAllPurgeWorker]
AS
SELECT [PurgeWorkerId], [DomainId], [Status], [CreateTimestamp], [UpdateTimestamp]
FROM [bll].[PurgeWorker] 
;