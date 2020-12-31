CREATE PROCEDURE [bla].[GetAccountGuidByUserGuid]
	@userGuid UNIQUEIDENTIFIER
AS
SELECT [AccountGuid] 
FROM [bla].[AccountUser] 
WHERE [UserGuid] = @userGuid
	AND [IsActive] <> 0
;