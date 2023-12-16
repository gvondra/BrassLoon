CREATE PROCEDURE [blad].[GetAddress]
	@id UNIQUEIDENTIFIER
AS
SELECT [AddressId], [DomainId], [KeyId], [Hash], [CreateTimestamp]
FROM [blad].[Address]
WHERE [AddressId] = @id
;