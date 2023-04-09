CREATE PROCEDURE [blwt].[UpdateWorkTask]
	@id UNIQUEIDENTIFIER,
	@workTaskStatusId UNIQUEIDENTIFIER,
	@title NVARCHAR(512),
	@text NVARCHAR(MAX),
	@assignedToUserId VARCHAR(1024) = '',
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [blwt].[WorkTask]
	SET 
		[WorkTaskStatusId] = @workTaskStatusId, 
		[Title] = @title, 
		[Text] = @text, 
		[AssignedToUserId] = @assignedToUserId,
		[UpdateTimestamp] = @timestamp
	WHERE [WorkTaskId] = @id
	;
END