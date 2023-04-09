CREATE PROCEDURE [blwt].[ClaimWorkTask]
	@workTaskId UNIQUEIDENTIFIER,
	@domainId UNIQUEIDENTIFIER,
	@userId VARCHAR(1024),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [blwt].[WorkTask]
	SET [AssignedToUserId] = @userId,
	[UpdateTimestamp] = @timestamp
	WHERE [WorkTaskId] = @workTaskId
	AND [DomainId] = @domainId
	AND ([AssignedToUserId] = '' OR @userId = '')
	;
END