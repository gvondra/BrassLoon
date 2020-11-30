CREATE PROCEDURE [bla].[UpdateClient]
	@id UNIQUEIDENTIFIER, 
	@name NVARCHAR(1024),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [bla].[Client]
	SET [Name] = @name,
		[UpdateTimestamp] = @timestamp
	WHERE [ClientId] = @id
	;
END