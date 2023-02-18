CREATE PROCEDURE [blwt].[CreateWorkTaskStatus]
	@id UNIQUEIDENTIFIER OUT,
	@workTaskTypeId UNIQUEIDENTIFIER,
	@domainId UNIQUEIDENTIFIER,
	@code NVARCHAR(128),
	@name NVARCHAR(128),
	@description NVARCHAR(MAX),
	@isDefaultStatus BIT,
	@isClosedStatus BIT,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blwt].[WorkTaskStatus] ([WorkTaskStatusId], [WorkTaskTypeId], [DomainId], 
	[Code], [Name], [Description], [IsDefaultStatus], [IsClosedStatus], 
	[CreateTimestamp], [UpdateTimestamp]) 
	VALUES (@id, @workTaskTypeId, @domainId,
	@code, @name, @description, @isDefaultStatus, @isClosedStatus,
	@timestamp, @timestamp)
	;
END