CREATE PROCEDURE [blt].[RemoveUserRole]
	@userId UNIQUEIDENTIFIER,
	@roleId UNIQUEIDENTIFIER
AS
UPDATE [blt].[UserRole]
SET [IsActive] = 0,
[UpdateTimestamp] = SYSUTCDATETIME()
WHERE [UserId] = @userId
AND [RoleId] = @roleId
;