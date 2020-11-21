﻿CREATE TABLE [bla].[Domain]
(
	[DomainGuid] UNIQUEIDENTIFIER DEFAULT(NEWID()) NOT NULL,
	[AccountGuid] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(2000) NOT NULL,
	[CreateTimestamp] DATETIME2(4) DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_Domain] PRIMARY KEY CLUSTERED ([DomainGuid]), 
    CONSTRAINT [FK_Domain_To_Account] FOREIGN KEY ([AccountGuid]) REFERENCES [bla].[Account]([AccountGuid])
)

GO

CREATE INDEX [IX_Domain_AccountGuid] ON [bla].[Domain] ([AccountGuid])
