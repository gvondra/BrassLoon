/*
2025 January, Greg: Replaced with v2
*/
CREATE PROCEDURE [bll].[CreateTrace]
	@id BIGINT OUT,
	@domainId UNIQUEIDENTIFIER,
	@eventCode VARCHAR(200),
	@message NVARCHAR(2000),
	@data NVARCHAR(MAX),
	@eventId UNIQUEIDENTIFIER = NULL,
	@category NVARCHAR(512) = '',
	@level NVARCHAR(512) = '',
	@timestamp DATETIME2(4)
AS
BEGIN
	INSERT INTO [bll].[Trace] ([DomainId], [EventCode], [Message], [Data], [CreateTimestamp],
		[EventId], [Category], [Level])
	VALUES (@domainId, @eventCode, @message, @data, @timestamp,
		@eventId, @category, @level)
	;
	SET @id = SCOPE_IDENTITY();
END
