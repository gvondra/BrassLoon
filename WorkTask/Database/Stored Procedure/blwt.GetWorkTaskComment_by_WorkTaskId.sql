CREATE PROCEDURE [blwt].[GetWorkTaskComment_by_WorkTaskId]
	@workTaskId UNIQUEIDENTIFIER
AS
SELECT [cmt].[CommentId], [cmt].[DomainId], [cmt].[Text], [cmt].[CreateTimestamp]
FROM [blwt].[WorkTaskComment] [wtc]
INNER JOIN [blwt].[Comment] [cmt] on [wtc].[CommentId] = [cmt].[CommentId] AND [wtc].[WorkTaskId] = @workTaskId
;