CREATE PROCEDURE [blt].[UpdateSigningKey]
	@id UNIQUEIDENTIFIER,
	@isActive BIT = 1,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [blt].[SigningKey] 
	SET [IsActive] = @isActive,
	[UpdateTimestamp] = @timestamp
	WHERE [SigningKeyId] = @id
	;
END