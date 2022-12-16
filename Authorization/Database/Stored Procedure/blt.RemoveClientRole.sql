CREATE PROCEDURE [blt].[RemoveClientRole]
	@clientId UNIQUEIDENTIFIER,
	@roleId UNIQUEIDENTIFIER
AS
UPDATE [blt].[ClientRole]
SET [IsActive] = 0,
[UpdateTimestamp] = SYSUTCDATETIME()
WHERE [ClientId] = @clientId
AND [RoleId] = @roleId
;