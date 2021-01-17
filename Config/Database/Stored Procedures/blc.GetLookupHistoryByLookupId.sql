CREATE PROCEDURE [blc].[GetLookupHistoryByLookupId]
	@lookupId UNIQUEIDENTIFIER
AS
SELECT [LookupHistoryId], [LookupId], [DomainId], [Code], [Data], [CreateTimestamp]
FROM [blc].[LookupHistory]
WHERE [LookupId] = @lookupId
ORDER BY CreateTimestamp DESC
;