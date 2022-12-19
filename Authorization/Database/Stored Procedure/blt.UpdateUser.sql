CREATE PROCEDURE [blt].[UpdateUser]
	@id UNIQUEIDENTIFIER,
	@emailAddressId UNIQUEIDENTIFIER,
	@name NVARCHAR(MAX),
	@timestamp DATETIME2(4) OUT
AS
BEGIN 
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [blt].[User]
	SET 
		[EmailAddressId] = @emailAddressId,
		[Name] = @name,
		[UpdateTimestamp] = @timestamp
	WHERE [UserId] = @id
	;
END