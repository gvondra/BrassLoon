CREATE PROCEDURE [blc].[DeleteLookupByCode]
	@domainId UNIQUEIDENTIFIER,
	@code varchar(256)
AS
DELETE FROM [blc].[LookupHistory]
WHERE [DomainId] = @domainId 
	AND [Code] = @code
;
DELETE FROM [blc].[Lookup]
WHERE [DomainId] = @domainId 
	AND [Code] = @code
;