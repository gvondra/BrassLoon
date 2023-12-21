CREATE PROCEDURE [blad].[GetEmailAddress]
	@id UNIQUEIDENTIFIER
AS
SELECT [EmailAddressId], [DomainId], [KeyId], [Hash], [InitializationVector], 
	[Address], [CreateTimestamp]
FROM [blad].[EmailAddress]
WHERE [EmailAddressId] = @id;