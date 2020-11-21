CREATE PROCEDURE [bla].[CreateAccountUser]
	@accountGuid UNIQUEIDENTIFIER,
	@userGuid UNIQUEIDENTIFIER,	
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [bla].[AccountUser] ([AccountGuid], [UserGuid], [CreateTimestamp], [UpdateTimestamp])
	VALUES (@accountGuid, @userGuid, @timestamp, @timestamp)
	;
END