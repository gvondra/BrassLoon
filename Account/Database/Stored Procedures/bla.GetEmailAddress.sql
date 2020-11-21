CREATE PROCEDURE [bla].[GetEmailAddress]
	@guid UNIQUEIDENTIFIER
AS
SELECT [EmailAddressGuid], [Address], [CreateTimestamp]
FROM [bla].[EmailAddress]
WHERE [EmailAddressGuid] = @guid 
;