CREATE PROCEDURE [bla].[GetClient_by_AccountId]
	@accountId UNIQUEIDENTIFIER
AS
SELECT [ClientId], [AccountGuid], [Name], [CreateTimestamp], [UpdateTimestamp]
FROM [bla].[Client]
WHERE [AccountGuid] = @accountId
;