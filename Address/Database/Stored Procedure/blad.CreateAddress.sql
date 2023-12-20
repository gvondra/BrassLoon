CREATE PROCEDURE [blad].[CreateAddress]
	@id UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@keyId UNIQUEIDENTIFIER,
	@hash BINARY(64),
	@initializationVector BINARY(16),
	@attention VARBINARY(8000),
	@addressee VARBINARY(8000),
	@delivery VARBINARY(8000),
	@city VARBINARY(8000),
	@territory VARBINARY(4000),
	@postalCode VARBINARY(4000),
	@country VARBINARY(4000),
	@county VARBINARY(8000),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blad].[Address] ([AddressId], [DomainId], [KeyId], [Hash], [InitializationVector], 
	[Attention], [Addressee], [Delivery], [City], [Territory], [PostalCode], [Country], [County],
	[CreateTimestamp])
	VALUES (@id, @domainId, @keyId, @hash, @initializationVector,
	@attention, @addressee, @delivery, @city, @territory, @postalCode, @country, @county,
	@timestamp);
END