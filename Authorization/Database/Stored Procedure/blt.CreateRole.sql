CREATE PROCEDURE [blt].[CreateRole]
	@id UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@name VARCHAR(1024),
	@policyName VARCHAR(256),
	@isActive BIT = 1,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blt].[Role] ([RoleId], [DomainId], [Name], [PolicyName], [IsActive],
		[CreateTimestamp], [UpdateTimestamp])
	VALUES (@id, @domainId, @name, @policyName, @isActive,
		@timestamp, @timestamp)
	;
END