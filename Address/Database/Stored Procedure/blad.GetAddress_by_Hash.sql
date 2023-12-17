CREATE PROCEDURE [blad].[GetAddress_by_Hash]
	@domainId UNIQUEIDENTIFIER,
	@hash BINARY(64)
AS
SELECT [AddressId], [DomainId], [KeyId], [Hash], [CreateTimestamp]
FROM [blad].[Address]
WHERE [DomainId] = @domainId
AND [Hash] = @hash
ORDER BY [CreateTimestamp]
;