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

	-- only want 1 status record to be default
	IF @isDefaultStatus = 1
	BEGIN -- The new status will become the default, so remove default setting from others
		UPDATE [blwt].[WorkTaskStatus]
		SET [IsDefaultStatus] = 0,
		[UpdateTimestamp] = @timestamp
		WHERE [WorkTaskTypeId] = @workTaskTypeId
	END
	ELSE IF 0 = (SELECT COUNT(1) FROM [blwt].[WorkTaskStatus] WHERE [WorkTaskTypeId] = @workTaskTypeId)
	BEGIN -- if new status is first status is must be default
		SET @isDefaultStatus = 1;
	END

	INSERT INTO [blwt].[WorkTaskStatus] ([WorkTaskStatusId], [WorkTaskTypeId], [DomainId], 
	[Code], [Name], [Description], [IsDefaultStatus], [IsClosedStatus], 
	[CreateTimestamp], [UpdateTimestamp]) 
	VALUES (@id, @workTaskTypeId, @domainId,
	@code, @name, @description, @isDefaultStatus, @isClosedStatus,
	@timestamp, @timestamp)
	;
END