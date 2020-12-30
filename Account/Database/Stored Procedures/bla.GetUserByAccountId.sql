CREATE PROCEDURE [bla].[GetUserByAccountId]
	@accountId UNIQUEIDENTIFIER
AS
SELECT [UserGuid], [ReferenceId], [Name], [EmailAddressGuid], [Roles], [CreateTimestamp], [UpdateTimestamp]
FROM [bla].[User]
WHERE 0 < (SELECT COUNT(1) FROM [bla].[AccountUser] WHERE [UserGuid] = [bla].[User].[UserGuid] AND [AccountGuid] = @accountId AND [IsActive] <> 0)
;