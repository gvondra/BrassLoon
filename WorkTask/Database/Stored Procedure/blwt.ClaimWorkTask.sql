CREATE PROCEDURE [blwt].[ClaimWorkTask]
	@workTaskId UNIQUEIDENTIFIER,
	@domainId UNIQUEIDENTIFIER,
	@userId VARCHAR(1024),
	@assignedDate DATE,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [blwt].[WorkTask]
	SET [AssignedToUserId] = @userId,
	[AssignedDate] = @assignedDate,
	[UpdateTimestamp] = @timestamp
	WHERE [WorkTaskId] = @workTaskId
	AND [DomainId] = @domainId
	AND ([AssignedToUserId] = '' OR @userId = '')
	;
END