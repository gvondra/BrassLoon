CREATE PROCEDURE [blc].[GetItemHistoryByItemId]
	@itemId UNIQUEIDENTIFIER
AS
SELECT [ItemHistoryId], [ItemId], [DomainId], [Code], [Data], [CreateTimestamp]
FROM [blc].[ItemHistory]
WHERE [ItemId] = @itemId
ORDER BY CreateTimestamp DESC
;