CREATE PROCEDURE [bla].[CreateClient]
	@id UNIQUEIDENTIFIER OUT, 
	@accountId UNIQUEIDENTIFIER,
	@name NVARCHAR(1024),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	SET @id = NEWID();
	INSERT INTO [bla].[Client] ([ClientId], [AccountGuid], [Name], [CreateTimestamp], [UpdateTimestamp])
	VALUES (@id, @accountId, @name, @timestamp, @timestamp)
	;
END