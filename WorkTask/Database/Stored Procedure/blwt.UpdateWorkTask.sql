CREATE PROCEDURE [blwt].[UpdateWorkTask]
	@id UNIQUEIDENTIFIER,
	@workTaskStatusId UNIQUEIDENTIFIER,
	@title NVARCHAR(512),
	@text NVARCHAR(MAX),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [blwt].[WorkTask]
	SET 
		[WorkTaskStatusId] = @workTaskStatusId, 
		[Title] = @title, 
		[Text] = @text, 
		[UpdateTimestamp] = @timestamp
	WHERE [WorkTaskId] = @id
	;
END