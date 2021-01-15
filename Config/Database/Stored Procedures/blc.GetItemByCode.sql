CREATE PROCEDURE [blc].[GetItemByCode]
	@domainId UNIQUEIDENTIFIER,
	@code varchar(256)
AS
SELECT [ItemId], [DomainId], [Code], [Data], [CreateTimestamp], [UpdateTimestamp]
FROM [blc].[Item]
WHERE [DomainId] = @domainId 
	AND [Code] = @code
;