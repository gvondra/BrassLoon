CREATE PROCEDURE [bla].[UpdateClient]
	@id UNIQUEIDENTIFIER, 
	@name NVARCHAR(1024),
	@secretType SMALLINT = 1,
	@secretKey UNIQUEIDENTIFIER = NULL,
	@secretSalt BINARY(16) = NULL,
	@isActive BIT = 1,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [bla].[Client]
	SET [Name] = @name,
		[SecretType] = @secretType,
		[SecretKey] = @secretKey,
		[SecretSalt] = @secretSalt,
		[IsActive] = @isActive,
		[UpdateTimestamp] = @timestamp
	WHERE [ClientId] = @id
	;
END