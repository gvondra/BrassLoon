﻿CREATE TABLE [blc].[Lookup]
(
	[LookupId] UNIQUEIDENTIFIER CONSTRAINT [DF_Lookup_Id] DEFAULT NEWID() NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[Code] VARCHAR(256) NOT NULL,
	[Data] NVARCHAR(MAX) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_Lookup_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_Lookup_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAiNT [PK_Lookup] PRIMARY KEY CLUSTERED ([LookupId])
)
WITH (DATA_COMPRESSION = PAGE)
GO

CREATE UNIQUE INDEX [IX_Lookup_DomainId_Code] ON [blc].[Lookup] ([DomainId], [Code])