CREATE PROCEDURE [bla].[GetClientCredential_by_ClientId]
	@clientId UNIQUEIDENTIFIER
AS
SELECT [ClientCredentialId], [ClientId], [Secret], [IsActive], [CreateTimestamp], [UpdateTimestamp]
FROM [bla].[ClientCredential]
WHERE [ClientId] = @clientId
;