﻿CREATE TABLE [blc].[LookupHistory]
(
	[LookupHistoryId] UNIQUEIDENTIFIER CONSTRAINT [DF_LookupHistory_Id] DEFAULT NEWID() NOT NULL,
	[LookupId] UNIQUEIDENTIFIER NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[Code] VARCHAR(256) NOT NULL,
	[Data] NVARCHAR(MAX) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_LookupHistory_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_LookupHistory] PRIMARY KEY CLUSTERED ([LookupHistoryId]), 
    CONSTRAINT [FK_LookupHistory_To_Lookup] FOREIGN KEY ([LookupId]) REFERENCES [blc].[Lookup]([LookupId])
)
WITH (DATA_COMPRESSION = PAGE)
GO

CREATE INDEX [IX_LookupHistory_LookupId] ON [blc].[LookupHistory] ([LookupId])
