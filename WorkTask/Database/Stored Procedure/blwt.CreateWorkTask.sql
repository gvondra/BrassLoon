CREATE PROCEDURE [blwt].[CreateWorkTask]
	@id UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@workTaskTypeId UNIQUEIDENTIFIER,
	@workTaskStatusId UNIQUEIDENTIFIER,
	@title NVARCHAR(512),
	@text NVARCHAR(MAX),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blwt].[WorkTask] ([WorkTaskId], [DomainId], [WorkTaskTypeId], [WorkTaskStatusId], [Title], [Text], [CreateTimestamp], [UpdateTimestamp])
	VALUES (@id, @domainId, @workTaskTypeId, @workTaskStatusId, @title, @text, @timestamp, @timestamp)
	;
END