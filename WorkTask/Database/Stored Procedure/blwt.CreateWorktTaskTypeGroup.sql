CREATE PROCEDURE [blwt].[CreateWorktTaskTypeGroup]
	@workTaskTypeId UNIQUEIDENTIFIER,
	@workGroupId UNIQUEIDENTIFIER
AS
BEGIN
	IF NOT EXISTS (SELECT TOP 1 1 FROM [blwt].[WorkTaskTypeGroup] WHERE [WorkTaskTypeId] = @workTaskTypeId AND [WorkGroupId] = @workGroupId)
	BEGIN
		INSERT INTO [blwt].[WorkTaskTypeGroup] ([WorkTaskTypeId], [WorkGroupId])
		VALUES (@workTaskTypeId, @workGroupId)
		;
	END
END