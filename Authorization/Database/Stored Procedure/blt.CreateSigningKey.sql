CREATE PROCEDURE [blt].[CreateSigningKey]
	@id UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@keyVaultKey UNIQUEIDENTIFIER,
	@isActive BIT = 1,
	@timestamp DATETIME2(4) OUT
AS
BEGIN 
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blt].[SigningKey] ([SigningKeyId], [DomainId], [KeyVaultKey], [IsActive],
		[CreateTimestamp], [UpdateTimestamp])
	VALUES (@id, @domainId, @keyVaultKey, @isActive,
		@timestamp, @timestamp)
	;
END