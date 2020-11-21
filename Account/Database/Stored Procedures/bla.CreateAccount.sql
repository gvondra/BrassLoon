CREATE PROCEDURE [bla].[CreateAccount]
	@guid UNIQUEIDENTIFIER OUT, 
	@userGuid UNIQUEIDENTIFIER,
	@name NVARCHAR(2000),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	SET @guid = NEWID();
	INSERT INTO [bla].[Account] ([AccountGuid], [Name], [CreateTimestamp], [UpdateTimestamp])
	VALUES (@guid, @name, @timestamp, @timestamp)
	;
	DECLARE @userTs DATETIME2(4);
	EXEC [bla].[CreateAccountUser] @guid, @userGuid, @userTs OUT
END