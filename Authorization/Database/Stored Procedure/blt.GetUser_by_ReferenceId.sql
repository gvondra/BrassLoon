CREATE PROCEDURE [blt].[GetUser_by_ReferenceId]
	@domainId UNIQUEIDENTIFIER,
	@referenceId VARCHAR(MAX)
AS
SELECT TOP 1 [UserId], [DomainId], [ReferenceId], [EmailAddressId], [Name], 
	[CreateTimestamp], [UpdateTimestamp]
FROM [blt].[User]
WHERE [DomainId] = @domainId
AND [ReferenceId] = @referenceId
;