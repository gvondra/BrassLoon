CREATE TABLE [bll].[Exception]
(
	[ExceptionId] BIGINT IDENTITY(1,1)  NOT NULL,
	[ParentExceptionId] BIGINT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[Message] NVARCHAR(2000) NOT NULL,
	[TypeName] NVARCHAR(2000) NOT NULL,
	[Source] NVARCHAR(2000) NOT NULL,
	[AppDomain] NVARCHAR(2000) NOT NULL,
	[TargetSite] NVARCHAR(2000) NOT NULL,
	[StackTrace] NVARCHAR(MAX) NOT NULL,
	[Data] NVARCHAR(MAX) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_Exception_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_Exception] PRIMARY KEY NONCLUSTERED ([ExceptionId]), 
    CONSTRAINT [FK_Exception_To_Exception] FOREIGN KEY ([ParentExceptionId]) REFERENCES [bll].[Exception]([ExceptionId])
)
WITH (DATA_COMPRESSION = PAGE)

GO

CREATE UNIQUE INDEX [IX_Exception_ParentExceptionId] ON [bll].[Exception] ([ParentExceptionId]) WHERE [ParentExceptionId] IS NOT NULL

GO

CREATE CLUSTERED INDEX [IX_Exception_DomainId] ON [bll].[Exception] ([DomainId])

