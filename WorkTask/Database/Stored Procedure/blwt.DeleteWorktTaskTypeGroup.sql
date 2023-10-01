/*
2023 October, Greg: Replaced by v2
*/
CREATE PROCEDURE [blwt].[DeleteWorktTaskTypeGroup]
	@workTaskTypeId UNIQUEIDENTIFIER,
	@workGroupId UNIQUEIDENTIFIER
AS
DELETE FROM [blwt].[WorkTaskTypeGroup]
WHERE [WorkTaskTypeId] = @workTaskTypeId 
AND [WorkGroupId] =  @workGroupId
;