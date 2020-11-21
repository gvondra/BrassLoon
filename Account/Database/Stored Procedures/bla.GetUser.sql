CREATE PROCEDURE [bla].[GetUser]
	@guid UNIQUEIDENTIFIER
AS
SELECT [UserGuid], [ReferenceId], [Name], [EmailAddressGuid], [CreateTimestamp], [UpdateTimestamp]
FROM [bla].[User]
WHERE [UserGuid] = @guid
;