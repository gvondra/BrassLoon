CREATE PROCEDURE [bll].[CreateMetric_v2]
	@id BIGINT OUT,
	@guid UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@eventCode VARCHAR(200),
	@magnitude FLOAT,
	@data NVARCHAR(MAX),
	@timestamp DATETIME2(4),
	@status VARCHAR(500),
	@requestor VARCHAR(200),
	@eventId UNIQUEIDENTIFIER,
	@category NVARCHAR(512),
	@level NVARCHAR(512)
AS
BEGIN
	SET @guid = NEWID();
	INSERT INTO [bll].[Metric] ([DomainId], [EventCode], [Magnitude], [Data], [CreateTimestamp], [Status], [Requestor],
		[EventId], [Category], [Level], [MetricGuid]) 
	VALUES (@domainId, @eventCode, @magnitude, @data, @timestamp, @status, @requestor,
		@eventId, @category, @level, @guid)
	;
	SET @id = SCOPE_IDENTITY();
END