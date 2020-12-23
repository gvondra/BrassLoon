CREATE PROCEDURE [bla].[GetDomainByAccountId]
	@accountId UNIQUEIDENTIFIER
AS
SELECT [DomainGuid], [AccountGuid], [Name], [Deleted], [CreateTimestamp], [UpdateTimestamp]
FROM [bla].[Domain]
WHERE [AccountGuid] = @accountId
;