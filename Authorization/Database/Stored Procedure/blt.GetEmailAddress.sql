CREATE PROCEDURE [blt].[GetEmailAddress]
	@id UNIQUEIDENTIFIER
AS
SELECT TOP 1 [EmailAddressId], [Address], [AddressHash], [CreateTimestamp]
FROM [blt].[EmailAddress]
WHERE [EmailAddressId] = @id
;