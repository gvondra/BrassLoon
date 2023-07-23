CREATE PROCEDURE [blt].[CreateClient]
	@id UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@name NVARCHAR(1024),
	@secretKey UNIQUEIDENTIFIER,
	@secretSalt VARBINARY(MAX),
	@isActive BIT,
	@userEmailAddressId UNIQUEIDENTIFIER = NULL,
	@userName NVARCHAR(1024) = '',
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blt].[Client] ([ClientId], [DomainId], [Name], [SecretKey], [SecretSalt], [IsActive],
		[UserEmailAddressId], [UserName],
		[CreateTimestamp], [UpdateTimestamp])
	VALUES (@id, @domainId, @name, @secretKey, @secretSalt, @isActive,
		@userEmailAddressId, @userName,
		@timestamp, @timestamp)
	;
END