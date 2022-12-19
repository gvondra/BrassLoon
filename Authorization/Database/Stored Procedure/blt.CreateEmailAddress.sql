CREATE PROCEDURE [blt].[CreateEmailAddress]
	@id UNIQUEIDENTIFIER OUT,
	@address NVARCHAR(2048),
	@addressHash VARBINARY(MAX),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blt].[EmailAddress] ([EmailAddressId], [Address], [AddressHash], [CreateTimestamp])
	VALUES (@id, @address, @addressHash, @timestamp)
	;
END