CREATE PROCEDURE [bla].[GetUser]
	@guid UNIQUEIDENTIFIER
AS
SELECT [UserGuid], [ReferenceId], [Name], [EmailAddressGuid], [Roles], [CreateTimestamp], [UpdateTimestamp]
FROM [bla].[User]
WHERE [UserGuid] = @guid
;