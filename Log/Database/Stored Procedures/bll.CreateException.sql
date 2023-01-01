CREATE PROCEDURE [bll].[CreateException]
	@id BIGINT OUT,
	@parentExceptionId BIGINT,
	@domainId UNIQUEIDENTIFIER,
	@message NVARCHAR(2000),
	@typeName NVARCHAR(2000),
	@source NVARCHAR(2000),
	@appDomain NVARCHAR(2000),
	@targetSite NVARCHAR(2000),
	@stackTrace NVARCHAR(MAX),
	@data NVARCHAR(MAX),
	@eventId UNIQUEIDENTIFIER = NULL,
	@category NVARCHAR(512) = '',
	@level NVARCHAR(512) = '',
	@timestamp DATETIME2(4)
AS
BEGIN
	INSERT INTO [bll].[Exception] ([ParentExceptionId], [DomainId], [Message], [TypeName], [Source], [AppDomain], [TargetSite], [StackTrace], [Data], [CreateTimestamp],
		[EventId], [Category], [Level]) 
	VALUES (@parentExceptionId, @domainId, @message, @typeName, @source, @appDomain, @targetSite, @stackTrace, @data, @timestamp,
		@eventId, @category, @level)
	;
	SET @id = SCOPE_IDENTITY();
END
