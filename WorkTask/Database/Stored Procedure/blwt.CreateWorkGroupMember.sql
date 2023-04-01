CREATE PROCEDURE [blwt].[CreateWorkGroupMember]
	@id UNIQUEIDENTIFIER OUT,
	@workGroupId UNIQUEIDENTIFIER,
	@domainId UNIQUEIDENTIFIER,
	@userId VARCHAR(MAX),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blwt].[WorkGroupMember] ([WorkGroupMemberId], [WorkGroupId], [DomainId], [UserId], [CreateTimestamp]) 
	VALUES (@id, @workGroupId, @domainId, @userId, @timestamp)
	;
END