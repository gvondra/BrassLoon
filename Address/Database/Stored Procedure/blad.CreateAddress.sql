CREATE PROCEDURE [blad].[CreateAddress]
	@id UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@hash BINARY(64),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blad].[Address] ([AddressId], [DomainId], [Hash], [CreateTimestamp])
	VALUES (@id, @domainId, @hash, @timestamp);
END