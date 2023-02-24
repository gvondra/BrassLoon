CREATE PROCEDURE [blwt].[UpdateWorkGroup]
	@id UNIQUEIDENTIFIER,
	@title NVARCHAR(512),
	@description NVARCHAR(MAX),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [blwt].[WorkGroup]
	SET [Title] = @title, 
	[Description] = @description, 
	[UpdateTimestamp] = @timestamp
	WHERE [WorkGroupId] = @id
	;
END