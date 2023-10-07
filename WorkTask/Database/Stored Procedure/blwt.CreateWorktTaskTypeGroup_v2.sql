CREATE PROCEDURE [blwt].[CreateWorktTaskTypeGroup_v2]
	@domainId UNIQUEIDENTIFIER,
	@workTaskTypeId UNIQUEIDENTIFIER,
	@workGroupId UNIQUEIDENTIFIER
AS
BEGIN
	IF EXISTS (SELECT TOP 1 1 FROM [blwt].[WorkTaskType] WHERE [DomainId] = @domainId AND [WorkTaskTypeId] = @workTaskTypeId)
	AND EXISTS (SELECT TOP 1 1 FROM [blwt].[WorkGroup] WHERE [DomainId] = @domainId AND [WorkGroupId] = @workGroupId)
	AND NOT EXISTS (SELECT TOP 1 1 FROM [blwt].[WorkTaskTypeGroup] WHERE [WorkTaskTypeId] = @workTaskTypeId AND [WorkGroupId] = @workGroupId)
	BEGIN
		INSERT INTO [blwt].[WorkTaskTypeGroup] ([WorkTaskTypeId], [WorkGroupId])
		VALUES (@workTaskTypeId, @workGroupId)
		;
	END
END