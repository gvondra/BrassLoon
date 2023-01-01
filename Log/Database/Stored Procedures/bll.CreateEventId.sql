CREATE PROCEDURE [bll].[CreateEventId]
	@eventId UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@id INT,
	@name NVARCHAR(512),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SELECT TOP 1 @eventId = [EventId], @timestamp = [CreateTimestamp] 
	FROM [bll].[EventId]
	WHERE [DomainId] = @domainId
	AND [Id] = @id 
	AND [Name] = @name
	;
	IF @eventId IS NULL 
	BEGIN
		SET @eventId = NEWID();
		SET @timestamp = SYSUTCDATETIME();
		INSERT INTO [bll].[EventId] ([EventId], [DomainId], [Id], [Name], [CreateTimestamp])
		VALUES (@eventId, @domainId, @id, @name, @timestamp)
		;
	END
END