CREATE PROCEDURE [blt].[GetRole]
	@id UNIQUEIDENTIFIER
AS
SELECT TOP 1 [RoleId], [DomainId], [Name], [PolicyName], [IsActive],
	[CreateTimestamp], [UpdateTimestamp]
FROM [blt].[Role]
WHERE [RoleId] = @id
;