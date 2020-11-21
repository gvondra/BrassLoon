CREATE PROCEDURE [bla].[GetAccount]
	@guid UNIQUEIDENTIFIER
AS
SELECT [AccountGuid], [Name], [CreateTimestamp], [UpdateTimestamp]
FROM [bla].[Account]
WHERE [AccountGuid] = @guid
;
