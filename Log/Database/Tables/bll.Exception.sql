CREATE TABLE [bll].[Exception]
(
	[ExceptionId] BIGINT IDENTITY(1,1)  NOT NULL,
	[ParentExceptionId] BIGINT NULL,
	[DomainId] VARCHAR(32) NOT NULL,
	[Message] NVARCHAR(2000) NOT NULL,
	[TypeName] NVARCHAR(2000) NOT NULL,
	[Source] NVARCHAR(2000) NOT NULL,
	[AppDomain] NVARCHAR(2000) NOT NULL,
	[TargetSite] NVARCHAR(2000) NOT NULL,
	[StackTrace] NVARCHAR(MAX) NOT NULL,
	[Data] NVARCHAR(MAX) NOT NULL,
	[CreateTimestamp] DATETIME2(4) DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_Exception] PRIMARY KEY CLUSTERED ([ExceptionId]), 
    CONSTRAINT [FK_Exception_To_Exception] FOREIGN KEY ([ParentExceptionId]) REFERENCES [bll].[Exception]([ExceptionId])
)

GO

CREATE INDEX [IX_Exception_ParentExceptionId] ON [bll].[Exception] ([ParentExceptionId])
