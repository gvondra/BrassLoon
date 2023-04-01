CREATE PROCEDURE [blwt].[CreateWorkGroup]
	@id UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@title NVARCHAR(512),
	@description NVARCHAR(MAX),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blwt].[WorkGroup] ([WorkGroupId], [DomainId], [Title], [Description], [CreateTimestamp], [UpdateTimestamp])
	VALUES (@id, @domainId, @title, @description, @timestamp, @timestamp) 
	;
END