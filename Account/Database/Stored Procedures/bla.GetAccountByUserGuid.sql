CREATE PROCEDURE [bla].[GetAccountByUserGuid]
	@userGuid UNIQUEIDENTIFIER
AS
SELECT [AccountGuid], [Name], [CreateTimestamp], [UpdateTimestamp]
FROM [bla].[Account]
WHERE 0 < (SELECT COUNT(1) FROM [bla].[AccountUser] WHERE [AccountGuid] = [bla].[Account].[AccountGuid] AND [UserGuid] = @userGuid AND [IsActive] <> 0)
ORDER BY [Name]
;
