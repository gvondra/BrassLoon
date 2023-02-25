CREATE PROCEDURE [blwt].[DeleteWorkGroupMember]
	@id UNIQUEIDENTIFIER
AS
DELETE FROM [blwt].[WorkGroupMember]
WHERE [WorkGroupMemberId] = @id
;