CREATE PROCEDURE [blwt].[DeleteWorktTaskTypeGroup]
	@workTaskTypeId UNIQUEIDENTIFIER,
	@workGroupId UNIQUEIDENTIFIER
AS
DELETE FROM [blwt].[WorkTaskTypeGroup]
WHERE [WorkTaskTypeId] = @workTaskTypeId 
AND [WorkGroupId] =  @workGroupId
;