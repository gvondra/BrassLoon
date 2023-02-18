CREATE TABLE [blwt].[WorkTaskComment]
(
	[WorkTaskId] UNIQUEIDENTIFIER NOT NULL,
	[CommentId] UNIQUEIDENTIFIER NOT NULL,
	CONSTRAINT [PK_WorkTaskComment] PRIMARY KEY NONCLUSTERED ([WorkTaskId], [CommentId]), 
    CONSTRAINT [FK_WorkTaskComment_To_WorkTask] FOREIGN KEY ([WorkTaskId]) REFERENCES [blwt].[WorkTask]([WorkTaskId]), 
    CONSTRAINT [FK_WorkTaskComment_To_Comment] FOREIGN KEY ([CommentId]) REFERENCES [blwt].[Comment]([CommentId])
)

GO

CREATE INDEX [IX_WorkTaskComment_WorkTaskId] ON [blwt].[WorkTaskComment] ([WorkTaskId])

GO

CREATE INDEX [IX_WorkTaskComment_CommentId] ON [blwt].[WorkTaskComment] ([CommentId])
