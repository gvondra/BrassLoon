CREATE PROCEDURE [blt].[GetSigningKey]
	@id UNIQUEIDENTIFIER
AS
SELECT TOP 1 [SigningKeyId], [DomainId], [KeyVaultKey], [IsActive],
	[CreateTimestamp], [UpdateTimestamp]
FROM [blt].[SigningKey] 
WHERE [SigningKeyId] = @id
;