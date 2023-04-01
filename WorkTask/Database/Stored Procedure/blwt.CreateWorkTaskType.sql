CREATE PROCEDURE [blwt].[CreateWorkTaskType]
	@id UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@code NVARCHAR(128) = '',
	@title NVARCHAR(512),
	@description NVARCHAR(MAX),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blwt].[WorkTaskType] ([WorkTaskTypeId], [DomainId], [Code], [Title], [Description], [CreateTimestamp], [UpdateTimestamp]) 
	VALUES (@id, @domainId, @code, @title, @description, @timestamp, @timestamp)
	;
END
