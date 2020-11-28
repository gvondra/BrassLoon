CREATE PROCEDURE [bla].[GetDomain]
	@id UNIQUEIDENTIFIER
AS
SELECT [DomainGuid], [AccountGuid], [Name], [CreateTimestamp], [UpdateTimestamp]
FROM [bla].[Domain]
WHERE [DomainGuid] = @id
;