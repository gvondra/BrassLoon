CREATE TABLE [blwt].[WorkGroupMember]
(
	[WorkGroupMemberId] UNIQUEIDENTIFIER NOT NULL,
	[WorkGroupId] UNIQUEIDENTIFIER NOT NULL,
	[DomainId] UNIQUEIDENTIFIER NOT NULL,
	[UserId] VARCHAR(1024) NOT NULL,
	[CreateTimestamp] DATETIME2(4) CONSTRAINT [DF_WorkGroupMember_CreateTimestamp] DEFAULT(SYSUTCDATETIME()) NOT NULL,
	CONSTRAINT [PK_WorkGroupMember] PRIMARY KEY NONCLUSTERED ([WorkGroupMemberId]), 
    CONSTRAINT [FK_WorkGroupMember_To_WorkGroup] FOREIGN KEY ([WorkGroupId]) REFERENCES [blwt].[WorkGroup]([WorkGroupId])
)

GO

CREATE CLUSTERED INDEX [IX_WorkGroupMember_DomainId] ON [blwt].[WorkGroupMember] ([DomainId])

GO

CREATE INDEX [IX_WorkGroupMember_WorkGroupId] ON [blwt].[WorkGroupMember] ([WorkGroupId])
