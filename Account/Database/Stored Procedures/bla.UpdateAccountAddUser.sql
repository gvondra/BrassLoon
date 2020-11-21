CREATE PROCEDURE [bla].[UpdateAccountAddUser]
	@accountGuid UNIQUEIDENTIFIER,
	@userGuid UNIQUEIDENTIFIER,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [bla].[AccountUser]
	SET [IsActive] = 1
	WHERE [AccountGuid] = @accountGuid
		and [UserGuid] = @userGuid
	;
	IF (@@ROWCOUNT = 0)
		EXEC [bla].[CreateAccountUser] @accountGuid, @userGuid, @timestamp OUT;
END
