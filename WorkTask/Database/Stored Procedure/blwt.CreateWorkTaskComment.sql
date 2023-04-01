CREATE PROCEDURE [blwt].[CreateWorkTaskComment]
	@commentId UNIQUEIDENTIFIER OUT,
	@workTaskId UNIQUEIDENTIFIER,
	@domainId UNIQUEIDENTIFIER,
	@text NVARCHAR(MAX),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	EXEC [blwt].[CreateComment] @commentId OUT, @domainId, @text, @timestamp OUT;
	INSERT INTO [blwt].[WorkTaskComment] ([WorkTaskId], [CommentId])
	VALUES (@workTaskId, @commentId)
	;
END