CREATE PROCEDURE [bll].[CreateTrace_v2]
	@id BIGINT OUT,
	@guid UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@eventCode VARCHAR(200),
	@message NVARCHAR(2000),
	@data NVARCHAR(MAX),
	@eventId UNIQUEIDENTIFIER,
	@category NVARCHAR(512),
	@level NVARCHAR(512),
	@timestamp DATETIME2(4)
AS
BEGIN
	SET @guid = NEWID();
	INSERT INTO [bll].[Trace] ([DomainId], [EventCode], [Message], [Data], [CreateTimestamp],
		[EventId], [Category], [Level], [TraceGuid])
	VALUES (@domainId, @eventCode, @message, @data, @timestamp,
		@eventId, @category, @level, @guid)
	;
	SET @id = SCOPE_IDENTITY();
END