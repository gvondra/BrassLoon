CREATE PROCEDURE [blt].[GetUser_by_DomainId]
	@domainId UNIQUEIDENTIFIER
AS
SELECT [UserId], [DomainId], [ReferenceId], [EmailAddressId], [Name], 
	[CreateTimestamp], [UpdateTimestamp]
FROM [blt].[User]
WHERE [DomainId] = @domainId
ORDER BY [Name], [UpdateTimestamp] DESC
;