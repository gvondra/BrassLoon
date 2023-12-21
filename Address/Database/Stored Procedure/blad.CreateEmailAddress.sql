CREATE PROCEDURE [blad].[CreateEmailAddress]
	@id UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@keyId UNIQUEIDENTIFIER,
	@hash BINARY(64),
	@initializationVector BINARY(16),
	@address VARBINARY(8000),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blad].[EmailAddress] ([EmailAddressId], [DomainId], [KeyId], [Hash], [InitializationVector], 
	[Address], [CreateTimestamp])
	VALUES (@id, @domainId, @keyId, @hash, @initializationVector,
	@address, @timestamp);
END