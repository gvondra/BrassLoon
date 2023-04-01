CREATE PROCEDURE [blwt].[UpdateWorkTaskStatus]
	@id UNIQUEIDENTIFIER,
	@workTaskTypeId UNIQUEIDENTIFIER,
	@name NVARCHAR(128),
	@description NVARCHAR(MAX),
	@isDefaultStatus BIT,
	@isClosedStatus BIT,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();

	-- only want 1 status record to be default
	IF @isDefaultStatus = 1
	BEGIN -- The target status will become the default, so remove default setting from others
		UPDATE [blwt].[WorkTaskStatus]
		SET [IsDefaultStatus] = 0,
		[UpdateTimestamp] = @timestamp
		WHERE [WorkTaskTypeId] = @workTaskTypeId
		AND [WorkTaskStatusId] <> @id
	END
	ELSE IF 0 < (SELECT COUNT(1) FROM [blwt].[WorkTaskStatus] WHERE [WorkTaskStatusId] = @id AND [IsDefaultStatus] = 1)
	BEGIN -- if target status is default status then it cannot be unset
		SET @isDefaultStatus = 1;
	END

	UPDATE [blwt].[WorkTaskStatus] 
	SET		
		[Name] = @name, 
		[Description] = @description, 
		[IsDefaultStatus] = @isDefaultStatus, 
		[IsClosedStatus] = @isClosedStatus, 
		[UpdateTimestamp] = @timestamp
	WHERE [WorkTaskStatusId] = @id
	;
END