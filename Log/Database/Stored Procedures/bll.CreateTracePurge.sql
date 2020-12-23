CREATE PROCEDURE [bll].[CreateTracePurge]
	@id BIGINT OUT,
	@domainId UNIQUEIDENTIFIER,
	@status SMALLINT,
	@targetId BIGINT,
	@expirationTimestamp DATETIME2(4),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [bll].[TracePurge] ([DomainId], [Status], [TargetId], [ExpirationTimestamp], [CreateTimestamp], [UpdateTimestamp]) 
	VALUES (@domainId, @status, @targetId, @expirationTimestamp, @timestamp, @timestamp) 
	;
	SET @id = SCOPE_IDENTITY();
END