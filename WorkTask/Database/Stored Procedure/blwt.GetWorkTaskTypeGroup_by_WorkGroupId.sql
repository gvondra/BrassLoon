CREATE PROCEDURE [blwt].[GetWorkTaskTypeGroup_by_WorkGroupId]
	@workGroupId UNIQUEIDENTIFIER
AS
SELECT [WorkTaskTypeId], [WorkGroupId]
FROM [blwt].[WorkTaskTypeGroup]
WHERE [WorkGroupId] = @workGroupId
;