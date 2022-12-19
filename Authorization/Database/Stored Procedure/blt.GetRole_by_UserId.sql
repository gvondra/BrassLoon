CREATE PROCEDURE [blt].[GetRole_by_UserId]
	@userId UNIQUEIDENTIFIER
AS
SELECT [rl].[RoleId], [rl].[DomainId], [rl].[Name], [rl].[PolicyName], [rl].[IsActive], [rl].[Comment], 
	[rl].[CreateTimestamp], [rl].[UpdateTimestamp]
FROM [blt].[Role] [rl]
INNER JOIN [blt].[UserRole] [ur] on [rl].[RoleId] = [ur].[RoleId] AND [ur].[UserId] = @userId
WHERE [ur].[IsActive] = 1
ORDER BY [rl].[Name]
;