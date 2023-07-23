CREATE PROCEDURE [blt].[UpdateClient]
	@id UNIQUEIDENTIFIER,
	@name NVARCHAR(1024),
	@isActive BIT,
	@userEmailAddressId UNIQUEIDENTIFIER = NULL,
	@userName NVARCHAR(1024) = '',
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [blt].[Client]
	SET [Name] = @name,
	[IsActive] = @isActive,
	[UserEmailAddressId] = @userEmailAddressId,
	[UserName] = @userName,
	[UpdateTimestamp] = @timestamp
	WHERE [ClientId] = @id
	;
END