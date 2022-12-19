CREATE PROCEDURE [blt].[AddClientRole]
	@clientId UNIQUEIDENTIFIER,
	@roleId UNIQUEIDENTIFIER
AS
BEGIN
	DECLARE @timestamp DATETIME2(4) = SYSUTCDATETIME();
	UPDATE [blt].[ClientRole]
	SET [IsActive] = 1,
	[UpdateTimestamp] = @timestamp
	WHERE [ClientId] = @clientId
	AND [RoleId] = @roleId
	;
	IF @@ROWCOUNT = 0
	BEGIN
		INSERT INTO [blt].[ClientRole] ([ClientId], [RoleId], [IsActive], [CreateTimestamp], [UpdateTimestamp])
		VALUES (@clientId, @roleId, 1, @timestamp, @timestamp)
		;
	END
END