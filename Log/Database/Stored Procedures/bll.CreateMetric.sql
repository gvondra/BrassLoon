CREATE PROCEDURE [bll].[CreateMetric]
	@id BIGINT OUT,
	@domainId UNIQUEIDENTIFIER,
	@eventCode VARCHAR(200),
	@magnitude FLOAT,
	@data NVARCHAR(MAX),
	@timestamp DATETIME2(4)
AS
BEGIN
	INSERT INTO [bll].[Metric] ([DomainId], [EventCode], [Magnitude], [Data], [CreateTimestamp]) 
	VALUES (@domainId, @eventCode, @magnitude, @data, @timestamp)
	;
	SET @id = SCOPE_IDENTITY();
END