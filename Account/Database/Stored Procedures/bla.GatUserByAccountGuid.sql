CREATE PROCEDURE [bla].[GatUserByAccountGuid]
	@accountGuid UNIQUEIDENTIFIER
AS
SELECT [UserGuid], [ReferenceId], [Name], [EmailAddressGuid], [CreateTimestamp], [UpdateTimestamp]
FROM [bla].[User]
WHERE 0 < (SELECT COUNT(1) FROM [bla].[AccountUser] WHERE [AccountGuid] = @accountGuid AND [UserGuid] = [bla].[AccountUser].[UserGuid] AND [IsActive] <> 0)
ORDER BY [Name]
;
