/*
2025 January, Greg: Replaced with v2
*/
CREATE PROCEDURE [bll].[CreateMetric]
	@id BIGINT OUT,
	@domainId UNIQUEIDENTIFIER,
	@eventCode VARCHAR(200),
	@magnitude FLOAT,
	@data NVARCHAR(MAX),
	@timestamp DATETIME2(4),
	@status VARCHAR(500) = '',
	@requestor VARCHAR(200) = '',
	@eventId UNIQUEIDENTIFIER = NULL,
	@category NVARCHAR(512) = '',
	@level NVARCHAR(512) = ''
AS
BEGIN
	INSERT INTO [bll].[Metric] ([DomainId], [EventCode], [Magnitude], [Data], [CreateTimestamp], [Status], [Requestor],
		[EventId], [Category], [Level]) 
	VALUES (@domainId, @eventCode, @magnitude, @data, @timestamp, @status, @requestor,
		@eventId, @category, @level)
	;
	SET @id = SCOPE_IDENTITY();
END