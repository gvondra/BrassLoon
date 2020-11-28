CREATE PROCEDURE [bla].[CreateDomain]
	@id UNIQUEIDENTIFIER OUT,
	@accountId UNIQUEIDENTIFIER,
	@name NVARCHAR(2000),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	SET @id = NEWID();
	INSERT INTO [bla].[Domain] ([DomainGuid], [AccountGuid], [Name], [CreateTimestamp], [UpdateTimestamp])
	VALUES (@id, @accountId, @name, @timestamp, @timestamp)
	;
END