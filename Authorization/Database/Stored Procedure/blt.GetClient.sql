CREATE PROCEDURE [blt].[GetClient]
	@id UNIQUEIDENTIFIER
AS
SELECT TOP 1 [ClientId], [DomainId], [Name], [SecretKey], [SecretSalt], [IsActive],
	[UserEmailAddressId], [UserName],
	[CreateTimestamp], [UpdateTimestamp]
FROM [blt].[Client]
WHERE [ClientId] = @id
;