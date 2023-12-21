CREATE PROCEDURE [blad].[GetPhone]
	@id UNIQUEIDENTIFIER
AS
SELECT [PhoneId], [DomainId], [KeyId], [Hash], [InitializationVector], 
	[Number], [CountryCode], [CreateTimestamp]
FROM [blad].[Phone]
WHERE [PhoneId] = @id;