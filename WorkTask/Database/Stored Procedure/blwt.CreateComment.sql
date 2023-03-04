CREATE PROCEDURE [blwt].[CreateComment]
	@id UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@text NVARCHAR(MAX),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @id = NEWID();
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blwt].[Comment] ([CommentId], [DomainId], [Text], [CreateTimestamp])
	VALUES (@id, @domainId, @text, @timestamp)
	;
END