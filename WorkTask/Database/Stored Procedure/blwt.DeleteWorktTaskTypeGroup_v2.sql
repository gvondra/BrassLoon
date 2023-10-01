CREATE PROCEDURE [blwt].[DeleteWorktTaskTypeGroup_v2]
	@domainId UNIQUEIDENTIFIER,
	@workTaskTypeId UNIQUEIDENTIFIER,
	@workGroupId UNIQUEIDENTIFIER
AS
DELETE FROM [blwt].[WorkTaskTypeGroup]
WHERE [WorkTaskTypeId] = @workTaskTypeId 
AND [WorkGroupId] =  @workGroupId
AND (EXISTS (SELECT TOP 1 1 FROM [blwt].[WorkTaskType] WHERE [DomainId] = @domainId AND [WorkTaskTypeId] = @workTaskTypeId)
OR EXISTS (SELECT TOP 1 1 FROM [blwt].[WorkGroup] WHERE [DomainId] = @domainId AND [WorkGroupId] = @workGroupId))
;