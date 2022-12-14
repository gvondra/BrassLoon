CREATE PROCEDURE [blt].[CreateClient]
	@id UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@name NVARCHAR(1024),
	@secretKey UNIQUEIDENTIFIER,
	@secretSalt VARBINARY(MAX),
	@isActive BIT,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blt].[Client] ([ClientId], [DomainId], [Name], [SecretKey], [SecretSalt], [IsActive],
		[CreateTimestamp], [UpdateTimestamp])
	VALUES (@id, @domainId, @name, @secretKey, @secretSalt, @isActive,
		@timestamp, @timestamp)
	;
END