CREATE PROCEDURE [bll].[UpdateExceptionPurge]
	@id BIGINT,
	@status SMALLINT,
	@expirationTimestamp DATETIME2(4),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [bll].[ExceptionPurge] 
	SET [Status] = @status,
		[ExpirationTimestamp] = @expirationTimestamp,
		[UpdateTimestamp] = @timestamp
	WHERE [PurgeId] = @id
	;
END