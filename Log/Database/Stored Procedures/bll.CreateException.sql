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
	@timestamp DATETIME2(4)
AS
BEGIN
	INSERT INTO [bll].[Exception] ([ParentExceptionId], [DomainId], [Message], [TypeName], [Source], [AppDomain], [TargetSite], [StackTrace], [Data], [CreateTimestamp]) 
	VALUES (@parentExceptionId, @domainId, @message, @typeName, @source, @appDomain, @targetSite, @stackTrace, @data, @timestamp)
	;
	SET @id = SCOPE_IDENTITY();
END
