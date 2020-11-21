CREATE PROCEDURE [bla].[UpdateAccount]
	@guid UNIQUEIDENTIFIER, 
	@name NVARCHAR(2000),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [bla].[Account]
	SET 
		[Name] = @name, 
		[UpdateTimestamp] = @timestamp
	WHERE [AccountGuid] = @guid
	;
END