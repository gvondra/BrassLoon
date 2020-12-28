CREATE PROCEDURE [bla].[CreateUserInvitation]
	@id UNIQUEIDENTIFIER OUT,
	@accountGuid UNIQUEIDENTIFIER, 
	@emailAddressGuid UNIQUEIDENTIFIER,
	@status SMALLINT,
	@expirationTimestamp DATETIME2(4),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	SET @id = NEWID();
	INSERT INTO [bla].[UserInvitation] ([UserInvitationId], [AccountGuid], [EmailAddressGuid], [Status], [ExpirationTimestamp], [CreateTimestamp], [UpdateTimestamp]) 
	VALUES (@id, @accountGuid, @emailAddressGuid, @status, @expirationTimestamp, @timestamp, @timestamp) 
	;
END