CREATE PROCEDURE [bla].[CreateUser]
	@guid UNIQUEIDENTIFIER OUT, 
	@referenceId VARCHAR(512),
	@emailAddressGuid UNIQUEIDENTIFIER,
	@name VARCHAR(512),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	SET @guid = NEWID();
	INSERT INTO [bla].[User] ([UserGuid], [ReferenceId], [Name], [EmailAddressGuid], [CreateTimestamp], [UpdateTimestamp])
	VALUES (@guid, @referenceId, @name, @emailAddressGuid, @timestamp, @timestamp)
	;
END