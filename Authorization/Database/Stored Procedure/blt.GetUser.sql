CREATE PROCEDURE [blt].[GetUser]
	@id UNIQUEIDENTIFIER
AS
SELECT TOP 1 [UserId], [DomainId], [ReferenceId], [EmailAddressId], [Name], 
	[CreateTimestamp], [UpdateTimestamp]
FROM [blt].[User]
WHERE [UserId] = @id
;