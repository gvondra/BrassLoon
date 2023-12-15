CREATE PROCEDURE [blad].[GetAddress_by_Hash]
	@hash BINARY(64)
AS
SELECT [AddressId], [DomainId], [Hash], [CreateTimestamp]
FROM [blad].[Address]
WHERE [Hash] = @hash
ORDER BY [CreateTimestamp]
;