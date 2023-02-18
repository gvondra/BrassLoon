CREATE PROCEDURE [blwt].[UpdateWorkTaskStatus]
	@id UNIQUEIDENTIFIER,
	@name NVARCHAR(128),
	@description NVARCHAR(MAX),
	@isDefaultStatus BIT,
	@isClosedStatus BIT,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
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