﻿CREATE TABLE [blwt].[Comment]
(
	[CommentId] UNIQUEIDENTIFIER NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[Text] NVARCHAR(MAX) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_Comment_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_Comment] PRIMARY KEY NONCLUSTERED ([CommentId])
)

GO

CREATE CLUSTERED INDEX [IX_Comment_DomainId] ON [blwt].[Comment] ([DomainId])
