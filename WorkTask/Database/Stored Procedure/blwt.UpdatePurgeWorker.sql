CREATE PROCEDURE [blwt].[UpdatePurgeWorker]
	@purgeWorkerId UNIQUEIDENTIFIER,
	@status SMALLINT,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [blwt].[PurgeWorker]
	SET [Status] = @status,
		[UpdateTimestamp] = @timestamp
	WHERE [PurgeWorkerId] = @purgeWorkerId
	;
END