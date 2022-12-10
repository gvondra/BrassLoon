CREATE PROCEDURE [blt].[GetRole_by_DomainId]
	@domainId UNIQUEIDENTIFIER
AS
SELECT [RoleId], [DomainId], [Name], [PolicyName],
	[CreateTimestamp], [UpdateTimestamp]
FROM [blt].[Role]
WHERE [DomainId] = @domainId
ORDER BY [Name]
;