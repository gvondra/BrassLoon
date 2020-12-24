CREATE PROCEDURE [bla].[GetUserByEmailAddress]
	@address NVARCHAR(2000)
AS
SELECT [UserGuid], [ReferenceId], [Name], [EmailAddressGuid], [Roles], [CreateTimestamp], [UpdateTimestamp]
FROM [bla].[User]
WHERE 0 < (SELECT COUNT(1) FROM [bla].[EmailAddress] WHERE [Address] = @address AND [bla].[EmailAddress].[EmailAddressGuid] = [bla].[User].[EmailAddressGuid])
;