CREATE PROCEDURE [bll].[GetEventId]
	@eventId UNIQUEIDENTIFIER
AS
SELECT TOP 1 [EventId], [DomainId], [Id], [Name], [CreateTimestamp]
FROM [bll].[EventId]
WHERE [EventId] = @eventId
;