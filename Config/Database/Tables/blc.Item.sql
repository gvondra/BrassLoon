﻿CREATE TABLE [blc].[Item]
(
	[ItemId] UNIQUEIDENTIFIER CONSTRAINT [DF_Item_Id] DEFAULT NEWID() NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[Code] VARCHAR(256) NOT NULL,
	[Data] NVARCHAR(MAX) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_Item_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_Item_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAiNT [PK_Item] PRIMARY KEY NONCLUSTERED ([ItemId])
)

GO

CREATE UNIQUE CLUSTERED INDEX [IX_Item_DomainId_Code] ON [blc].[Item] ([DomainId], [Code])
