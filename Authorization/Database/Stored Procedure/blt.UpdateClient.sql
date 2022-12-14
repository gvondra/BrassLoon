CREATE PROCEDURE [blt].[UpdateClient]
	@id UNIQUEIDENTIFIER,
	@name NVARCHAR(1024),
	@isActive BIT,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [blt].[Client]
	SET [Name] = @name,
	[IsActive] = @isActive,
	[UpdateTimestamp] = @timestamp
	WHERE [ClientId] = @id
	;
END