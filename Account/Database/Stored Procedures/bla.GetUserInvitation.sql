CREATE PROCEDURE [bla].[GetUserInvitation]
	@id UNIQUEIDENTIFIER
AS
SELECT [UserInvitationId], [AccountGuid], [EmailAddressGuid], [Status], [ExpirationTimestamp], [CreateTimestamp], [UpdateTimestamp] 
FROM [bla].[UserInvitation] 
WHERE [UserInvitationId] = @id 
;