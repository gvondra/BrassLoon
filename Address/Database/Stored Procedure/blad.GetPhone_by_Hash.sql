CREATE PROCEDURE [blad].[GetPhone_by_Hash]
	@domainId UNIQUEIDENTIFIER,
	@hash BINARY(64)
AS
SELECT [PhoneId], [DomainId], [KeyId], [Hash], [InitializationVector], 
	[Number], [CountryCode], [CreateTimestamp]
FROM [blad].[Phone]
WHERE [DomainId] = @domainId
AND [Hash] = @hash;