CREATE PROCEDURE [blt].[GetClient_by_DomainId]
	@domainId UNIQUEIDENTIFIER
AS
SELECT [ClientId], [DomainId], [Name], [SecretKey], [SecretSalt], [IsActive],
	[UserEmailAddressId], [UserName],
	[CreateTimestamp], [UpdateTimestamp]
FROM [blt].[Client]
WHERE [DomainId] = @domainId
ORDER BY [IsActive] DESC, [Name]
;