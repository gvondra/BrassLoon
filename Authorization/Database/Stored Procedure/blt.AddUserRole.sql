CREATE PROCEDURE [blt].[AddUserRole]
	@userId UNIQUEIDENTIFIER,
	@roleId UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @timestamp DATETIME2(4) = SYSUTCDATETIME();
	UPDATE [blt].[UserRole]
	SET [IsActive] = 1,
	[UpdateTimestamp] = @timestamp
	WHERE [UserId] = @userId
	AND [RoleId] = @roleId
	;
	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO [blt].[UserRole] ([UserId], [RoleId], [IsActive], [CreateTimestamp], [UpdateTimestamp])
		VALUES (@userId, @roleId, 1, @timestamp, @timestamp)
		;
	END
END