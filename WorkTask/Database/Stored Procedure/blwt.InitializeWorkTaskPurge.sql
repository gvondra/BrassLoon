CREATE PROCEDURE [blwt].[InitializeWorkTaskPurge]
    @domainId UNIQUEIDENTIFIER,
	@expirationTimestamp DATETIME2(4),
	@defaultPurgePeriod SMALLINT
AS
BEGIN
    DECLARE @timestamp DATETIME2(4) = SYSUTCDATETIME();
    MERGE INTO [blwt].[WorkTaskPurge] AS [Target]
    USING (SELECT [wt].[DomainId], [wt].[WorkTaskId] [TargetId]
        FROM [blwt].[WorkTask] [wt]
        INNER JOIN [blwt].[WorkTaskType] [wtt] on [wt].[WorkTaskTypeId] = [wtt].[WorkTaskTypeId]
        INNER JOIN [blwt].[WorkTaskStatus] [wts] on [wt].[WorkTaskStatusId] = [wts].[WorkTaskStatusId] AND [wts].[IsClosedStatus] <> 0
        WHERE [wt].[DomainId] = @domainId
            AND [wt].[UpdateTimestamp] < DATEADD(mm, -1 * ISNULL([wtt].[PurgePeriod], @defaultPurgePeriod), SYSUTCDATETIME()))
        AS [Source]
    ON [Target].[TargetId] = [Source].[TargetId]
    WHEN MATCHED AND [Target].[Status] <> 0
        THEN UPDATE SET 
            [Status] = 0,
            [UpdateTimestamp] = @timestamp
    WHEN NOT MATCHED BY TARGET
        THEN INSERT ([PurgeId], [DomainId], [Status], [TargetId], [ExpirationTimestamp], [CreateTimestamp], [UpdateTimestamp])
        VALUES (NEWID(), [Source].[DomainId], 0, [Source].[TargetId], @expirationTimestamp, @timestamp, @timestamp)
    ;    
END