CREATE TABLE [blc].[ItemHistory]
(
	[ItemHistoryId] UNIQUEIDENTIFIER CONSTRAINT [DF_ItemHistory_Id] DEFAULT NEWID() NOT NULL,
	[ItemId] UNIQUEIDENTIFIER NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[Code] VARCHAR(256) NOT NULL,
	[Data] NVARCHAR(MAX) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_ItemHistory_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_ItemHistory] PRIMARY KEY NONCLUSTERED ([ItemHistoryId]), 
    CONSTRAINT [FK_ItemHistory_To_Item] FOREIGN KEY ([ItemId]) REFERENCES [blc].[Item]([ItemId])
)
WITH (DATA_COMPRESSION = PAGE)
GO

CREATE INDEX [IX_ItemHistory_ItemId] ON [blc].[ItemHistory] ([ItemId])

GO

CREATE CLUSTERED INDEX [IX_ItemHistory_DomainId] ON [blc].[ItemHistory] ([DomainId])
