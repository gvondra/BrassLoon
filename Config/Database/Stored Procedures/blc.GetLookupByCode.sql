CREATE PROCEDURE [blc].[GetLookupByCode]
	@domainId UNIQUEIDENTIFIER,
	@code varchar(256)
AS
SELECT [LookupId], [DomainId], [Code], [Data], [CreateTimestamp], [UpdateTimestamp]
FROM [blc].[Lookup]
WHERE [DomainId] = @domainId 
	AND [Code] = @code
;