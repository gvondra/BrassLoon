CREATE PROCEDURE [blt].[CreateUser]
	@id UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@referenceId VARCHAR(MAX),
	@emailAddressId UNIQUEIDENTIFIER,
	@name NVARCHAR(MAX),
	@timestamp DATETIME2(4) OUT
AS
BEGIN 
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blt].[User] ([UserId], [DomainId], [ReferenceId], [EmailAddressId], [Name], 
		[CreateTimestamp], [UpdateTimestamp])
	VALUES (@id, @domainId, @referenceId, @emailAddressId, @name,
		@timestamp, @timestamp)
	;
END