CREATE PROCEDURE [bll].[CreateMetric]
	@id BIGINT OUT,
	@domainId UNIQUEIDENTIFIER,
	@eventCode VARCHAR(200),
	@magnitude FLOAT,
	@data NVARCHAR(MAX),
	@timestamp DATETIME2(4),
	@status VARCHAR(500) = '',
	@requestor VARCHAR(200) = ''
AS
BEGIN
	INSERT INTO [bll].[Metric] ([DomainId], [EventCode], [Magnitude], [Data], [CreateTimestamp], [Status], [Requestor]) 
	VALUES (@domainId, @eventCode, @magnitude, @data, @timestamp, @status, @requestor)
	;
	SET @id = SCOPE_IDENTITY();
END