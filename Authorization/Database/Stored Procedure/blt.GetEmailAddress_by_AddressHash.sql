CREATE PROCEDURE [blt].[GetEmailAddress_by_AddressHash]
	@addressHash VARBINARY(MAX)
AS
SELECT TOP 1 [EmailAddressId], [Address], [AddressHash], [CreateTimestamp]
FROM [blt].[EmailAddress]
WHERE [AddressHash] = @addressHash
;