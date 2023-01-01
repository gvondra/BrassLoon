CREATE PROCEDURE [bll].[GetEventId_by_DomainId]
	@domainId UNIQUEIDENTIFIER
AS
SELECT [EventId], [DomainId], [Id], [Name], [CreateTimestamp]
FROM [bll].[EventId]
WHERE [DomainId] = @domainId
ORDER BY [EventId], [Name], [CreateTimestamp] DESC
;