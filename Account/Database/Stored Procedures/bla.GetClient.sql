CREATE PROCEDURE [bla].[GetClient]
	@id UNIQUEIDENTIFIER
AS
SELECT [ClientId], [AccountGuid], [Name], [CreateTimestamp], [UpdateTimestamp],
[SecretType], [SecretKey], [SecretSalt], [IsActive]
FROM [bla].[Client]
WHERE [ClientId] = @id
;