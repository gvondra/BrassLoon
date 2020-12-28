CREATE PROCEDURE [bla].[UpdateUserInvitation]
	@id UNIQUEIDENTIFIER,
	@status SMALLINT,
	@expirationTimestamp DATETIME2(4),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [bla].[UserInvitation] 
	SET 
		[Status] = @status, 
		[ExpirationTimestamp] = @expirationTimestamp, 
		[UpdateTimestamp] = @timestamp
	WHERE [UserInvitationId] = @id 
	;
END