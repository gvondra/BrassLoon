CREATE PROCEDURE [bla].[UpdateAccountRemoveUser]
	@accountGuid UNIQUEIDENTIFIER,
	@userGuid UNIQUEIDENTIFIER,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	IF ((SELECT COUNT(1) FROM [bla].[AccountUser] WHERE [AccountGuid] = @accountGuid AND [IsActive] <> 0) > 1)
		UPDATE [bla].[AccountUser]
		SET [IsActive] = 0
		WHERE [AccountGuid] = @accountGuid 
			and [UserGuid] = @userGuid
		;
	ELSE
		THROW 50000, 'Last user cannot be removed from account', 0;
END
