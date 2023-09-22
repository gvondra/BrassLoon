CREATE PROCEDURE [blwt].[CreateWorkTask]
	@id UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@workTaskTypeId UNIQUEIDENTIFIER,
	@workTaskStatusId UNIQUEIDENTIFIER,
	@title NVARCHAR(512),
	@text NVARCHAR(MAX),
	@assignedToUserId VARCHAR(1024) = '',
	@assignedDate DATE = NULL,
	@closedDate DATE = NULL,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blwt].[WorkTask] ([WorkTaskId], [DomainId], [WorkTaskTypeId], [WorkTaskStatusId], [Title], [Text], [AssignedToUserId], [AssignedDate], [ClosedDate],
	[CreateTimestamp], [UpdateTimestamp])
	VALUES (@id, @domainId, @workTaskTypeId, @workTaskStatusId, @title, @text, @assignedToUserId, @assignedDate, @closedDate,
	@timestamp, @timestamp)
	;
END