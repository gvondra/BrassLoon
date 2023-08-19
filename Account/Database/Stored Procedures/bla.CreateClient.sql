CREATE PROCEDURE [bla].[CreateClient]
	@id UNIQUEIDENTIFIER OUT, 
	@accountId UNIQUEIDENTIFIER,
	@name NVARCHAR(1024),
	@secretType SMALLINT = 1,
	@secretKey UNIQUEIDENTIFIER = NULL,
	@secretSalt BINARY(16) = NULL,
	@isActive BIT = 1,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	SET @id = NEWID();
	INSERT INTO [bla].[Client] ([ClientId], [AccountGuid], [Name], [CreateTimestamp], [UpdateTimestamp], [SecretType], [SecretKey], [SecretSalt], [IsActive])
	VALUES (@id, @accountId, @name, @timestamp, @timestamp, @secretType, @secretKey, @secretSalt, @isActive)
	;
END