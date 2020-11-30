CREATE PROCEDURE [bla].[GetClientCredential]
	@id UNIQUEIDENTIFIER
AS
SELECT [ClientCredentialId], [ClientId], [Secret], [IsActive], [CreateTimestamp], [UpdateTimestamp]
FROM [bla].[ClientCredential]
WHERE [ClientCredentialId] = @id
;