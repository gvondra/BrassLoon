CREATE PROCEDURE [blwt].[CreateWorkTaskContext]
	@id UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@workTaskId UNIQUEIDENTIFIER,
	@status SMALLINT,
	@referenceType SMALLINT,
	@referenceValue NVARCHAR(2048),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blwt].[WorkTaskContext] ([WorkTaskContextId], [DomainId], [WorkTaskId], [Status], [ReferenceType], [ReferenceValue], [CreateTimestamp]) 
	VALUES (@id, @domainId, @workTaskId, @status, @referenceType, @referenceValue, @timestamp) 
	;
END