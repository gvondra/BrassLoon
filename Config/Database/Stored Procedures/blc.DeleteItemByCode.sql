CREATE PROCEDURE [blc].[DeleteItemByCode]
	@domainId UNIQUEIDENTIFIER,
	@code varchar(256)
AS
DELETE FROM [blc].[ItemHistory]
WHERE [DomainId] = @domainId 
	AND [Code] = @code
;
DELETE FROM [blc].[Item]
WHERE [DomainId] = @domainId 
	AND [Code] = @code
;