CREATE PROCEDURE [blad].[CreateAddress]
	@id UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@keyId UNIQUEIDENTIFIER,
	@hash BINARY(64),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blad].[Address] ([AddressId], [DomainId], [KeyId], [Hash], [CreateTimestamp])
	VALUES (@id, @domainId, @keyId, @hash, @timestamp);
END