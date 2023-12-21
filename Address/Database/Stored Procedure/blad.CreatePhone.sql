CREATE PROCEDURE [blad].[CreatePhone]
	@id UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@keyId UNIQUEIDENTIFIER,
	@hash BINARY(64),
	@initializationVector BINARY(16),
	@number VARBINARY(4000),
	@countryCode VARCHAR(8),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blad].[Phone] ([PhoneId], [DomainId], [KeyId], [Hash], [InitializationVector], 
	[Number], [CountryCode], [CreateTimestamp])
	VALUES (@id, @domainId, @keyId, @hash, @initializationVector,
	@number, @countryCode, @timestamp);
END