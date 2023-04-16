CREATE PROCEDURE [blwt].[UpdateWorkTask]
	@id UNIQUEIDENTIFIER,
	@workTaskStatusId UNIQUEIDENTIFIER,
	@title NVARCHAR(512),
	@text NVARCHAR(MAX),
	@assignedToUserId VARCHAR(1024) = '',
	@assignedDate DATE = NULL,
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
		[AssignedDate] = @assignedDate,
		[UpdateTimestamp] = @timestamp
	WHERE [WorkTaskId] = @id
	;
END