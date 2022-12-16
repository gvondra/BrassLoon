CREATE PROCEDURE [blt].[GetUser_by_EmailAddressHash]
	@domainId UNIQUEIDENTIFIER,
	@addressHash VARBINARY(MAX)
AS
SELECT TOP 1 [usr].[UserId], [usr].[DomainId], [usr].[ReferenceId], [usr].[EmailAddressId], [usr].[Name], 
	[usr].[CreateTimestamp], [usr].[UpdateTimestamp]
FROM [blt].[User] [usr]
INNER JOIN [blt].[EmailAddress] [ea] on [usr].[EmailAddressId] = [ea].[EmailAddressId] AND [ea].[AddressHash] = @addressHash
WHERE [usr].[DomainId] = @domainId
;