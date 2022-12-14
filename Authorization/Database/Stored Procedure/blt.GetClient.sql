CREATE PROCEDURE [blt].[GetClient]
	@id UNIQUEIDENTIFIER
AS
SELECT TOP 1 [ClientId], [DomainId], [Name], [SecretKey], [SecretSalt], [IsActive],
	[CreateTimestamp], [UpdateTimestamp]
FROM [blt].[Client]
WHERE [ClientId] = @id
;