CREATE PROCEDURE [blad].[GetEmailAddress_by_Hash]
	@domainId UNIQUEIDENTIFIER,
	@hash BINARY(64)
AS
SELECT [EmailAddressId], [DomainId], [KeyId], [Hash], [InitializationVector], 
	[Address], [CreateTimestamp]
FROM [blad].[EmailAddress]
WHERE [DomainId] = @domainId
AND [Hash] = @hash;