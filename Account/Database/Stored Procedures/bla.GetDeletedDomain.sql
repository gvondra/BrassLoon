CREATE PROCEDURE [bla].[GetDeletedDomain]
	@id UNIQUEIDENTIFIER
AS
SELECT [DomainGuid], [AccountGuid], [Name], [Deleted], [CreateTimestamp], [UpdateTimestamp]
FROM [bla].[Domain]
WHERE [DomainGuid] = @id
	AND [Deleted] <> 0
;