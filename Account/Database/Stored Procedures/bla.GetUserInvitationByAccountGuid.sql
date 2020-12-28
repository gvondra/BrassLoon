CREATE PROCEDURE [bla].[GetUserInvitationByAccountGuid]
	@accountGuid UNIQUEIDENTIFIER
AS
SELECT [UserInvitationId], [AccountGuid], [EmailAddressGuid], [Status], [ExpirationTimestamp], [CreateTimestamp], [UpdateTimestamp] 
FROM [bla].[UserInvitation] 
WHERE [AccountGuid] = @accountGuid
;