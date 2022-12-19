CREATE PROCEDURE [blt].[UpdateRole]
	@id UNIQUEIDENTIFIER,
	@name VARCHAR(1024),
	@isActive BIT = 1,
	@comment NVARCHAR(1024) = '',
	@timestamp DATETIME2(4) OUT
AS
BEGIN 
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [blt].[Role] 
	SET 
		[Name] = @name,
		[IsActive] = @isActive,
		[Comment] = @comment,
		[UpdateTimestamp] = @timestamp
	WHERE [RoleId] = @id
	;
END