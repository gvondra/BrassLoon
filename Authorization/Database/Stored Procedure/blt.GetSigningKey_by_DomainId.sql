CREATE PROCEDURE [blt].[GetSigningKey_by_DomainId]
	@domainId UNIQUEIDENTIFIER
AS
SELECT [SigningKeyId], [DomainId], [KeyVaultKey], [IsActive],
	[CreateTimestamp], [UpdateTimestamp]
FROM [blt].[SigningKey] 
WHERE [DomainId] = @domainId
;