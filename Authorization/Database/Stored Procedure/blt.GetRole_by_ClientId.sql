CREATE PROCEDURE [blt].[GetRole_by_ClientId]
	@clientId UNIQUEIDENTIFIER
AS
SELECT [rl].[RoleId], [rl].[DomainId], [rl].[Name], [rl].[PolicyName], [rl].[IsActive], [rl].[Comment], 
	[rl].[CreateTimestamp], [rl].[UpdateTimestamp]
FROM [blt].[Role] [rl]
INNER JOIN [blt].[ClientRole] [cr] on [rl].[RoleId] = [cr].[RoleId] AND [cr].[ClientId] = @clientId
WHERE [cr].[IsActive] = 1
ORDER BY [rl].[Name]
;