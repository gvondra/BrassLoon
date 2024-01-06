CREATE PROCEDURE [bll].[InitializeExceptionPurge]
    @domainId UNIQUEIDENTIFIER,
	@expirationTimestamp DATETIME2(4),
	@maxCreateTimestamp DATETIME2(4)
AS
BEGIN
    DECLARE @timestamp DATETIME2(4) = SYSUTCDATETIME();
    MERGE INTO [bll].[ExceptionPurge] AS [Target]
    USING (SELECT [DomainId], [ExceptionId] [TargetId]
        FROM [bll].[Exception]
        WHERE [DomainId] = @domainId
            AND [CreateTimestamp] < @maxCreateTimestamp)
        AS [Source]
    ON [Target].[TargetId] = [Source].[TargetId]
    WHEN MATCHED AND [Target].[Status] <> 0 AND [Target].[Status] <> 255
        THEN UPDATE SET 
            [Status] = 0,
            [UpdateTimestamp] = @timestamp
    WHEN NOT MATCHED BY TARGET
        THEN INSERT ([DomainId], [Status], [TargetId], [ExpirationTimestamp], [CreateTimestamp], [UpdateTimestamp])
        VALUES ([Source].[DomainId], 0, [Source].[TargetId], @expirationTimestamp, @timestamp, @timestamp)
    ;    
END