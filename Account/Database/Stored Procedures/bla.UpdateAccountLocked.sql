CREATE PROCEDURE [bla].[UpdateAccountLocked]
	@guid UNIQUEIDENTIFIER, 
	@locked BIT,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [bla].[Account]
	SET 
		[Locked] = @locked, 
		[UpdateTimestamp] = @timestamp
	WHERE [AccountGuid] = @guid
	;
END