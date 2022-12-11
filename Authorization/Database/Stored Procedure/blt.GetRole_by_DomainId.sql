CREATE PROCEDURE [blt].[GetRole_by_DomainId]
	@domainId UNIQUEIDENTIFIER
AS
SELECT [RoleId], [DomainId], [Name], [PolicyName], [IsActive], [Comment], 
	[CreateTimestamp], [UpdateTimestamp]
FROM [blt].[Role]
WHERE [DomainId] = @domainId
ORDER BY [IsActive] DESC,[Name]
;