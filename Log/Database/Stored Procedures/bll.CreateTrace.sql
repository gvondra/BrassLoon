CREATE PROCEDURE [bll].[CreateTrace]
	@id BIGINT OUT,
	@domainId UNIQUEIDENTIFIER,
	@eventCode VARCHAR(200),
	@message NVARCHAR(2000),
	@data NVARCHAR(MAX),
	@timestamp DATETIME2(4)
AS
BEGIN
	INSERT INTO [bll].[Trace] ([DomainId], [EventCode], [Message], [Data], [CreateTimestamp])
	VALUES (@domainId, @eventCode, @message, @data, @timestamp)
	;
	SET @id = SCOPE_IDENTITY();
END