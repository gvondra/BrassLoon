﻿CREATE TABLE [blt].[SigningKey]
(
	[SigningKeyId] UNIQUEIDENTIFIER NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[TknCsp] VARBINARY(MAX) NOT NULL,
	[IsActive] BIT CONSTRAINT [DF_SigningKey_IsActive] DEFAULT 1 NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_SigningKey_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	[UpdateTimestamp] DATETIME2(4) CONSTRAINT [DF_SigningKey_UpdateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_SigningKey] PRIMARY KEY CLUSTERED ([SigningKeyId])
)

GO

CREATE INDEX [IX_SigningKey_DomainId] ON [blt].[SigningKey] ([DomainId])
