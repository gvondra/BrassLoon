CREATE PROCEDURE [blwt].[UpdateWorkTaskType]
	@id UNIQUEIDENTIFIER,
	@title NVARCHAR(512),
	@description NVARCHAR(MAX),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [blwt].[WorkTaskType] 
	SET 
		[Title] = @title, 
		[Description] = @description, 
		[UpdateTimestamp] = @timestamp
	WHERE [WorkTaskTypeId] = @id
	;
END
