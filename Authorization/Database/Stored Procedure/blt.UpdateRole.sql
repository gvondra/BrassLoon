CREATE PROCEDURE [blt].[UpdateRole]
	@id UNIQUEIDENTIFIER,
	@name VARCHAR(1024),
	@timestamp DATETIME2(4) OUT
AS
BEGIN 
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [blt].[Role] 
	SET 
		[Name] = @name,
		[UpdateTimestamp] = @timestamp
	WHERE [RoleId] = @id
	;
END