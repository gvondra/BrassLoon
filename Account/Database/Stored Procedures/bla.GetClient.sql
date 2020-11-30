CREATE PROCEDURE [bla].[GetClient]
	@id UNIQUEIDENTIFIER
AS
SELECT [ClientId], [AccountGuid], [Name], [CreateTimestamp], [UpdateTimestamp]
FROM [bla].[Client]
WHERE [ClientId] = @id
;