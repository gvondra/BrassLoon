CREATE PROCEDURE [bla].[CreateEmailAddress]
	@guid UNIQUEIDENTIFIER OUT,
	@address NVARCHAR(2000),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	SET @guid = NEWID();
	INSERT INTO [bla].[EmailAddress] ([EmailAddressGuid], [Address], [CreateTimestamp])
	VALUES (@guid, @address, @timestamp)
	;
END