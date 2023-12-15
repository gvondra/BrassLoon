CREATE PROCEDURE [blad].[GetAddress]
	@id UNIQUEIDENTIFIER
AS
SELECT [AddressId], [DomainId], [Hash], [CreateTimestamp]
FROM [blad].[Address]
WHERE [AddressId] = @id
;