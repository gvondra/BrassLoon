CREATE PROCEDURE [bll].[PurgeMetric]
    @domainId UNIQUEIDENTIFIER,
	@maxExpirationTimestamp DATETIME2(4)
AS
BEGIN
	SET NOCOUNT ON;  
    DECLARE metaDataCursor CURSOR FOR 
    SELECT [PurgeId], [TargetId] 
    FROM [bll].[MetricPurge]
    WHERE [DomainId] = @domainId
        AND [Status] = 0
        AND [ExpirationTimestamp] < @maxExpirationTimestamp
    ORDER BY [ExpirationTimestamp] DESC
    ;

    DECLARE @purgeId BIGINT;
    DECLARE @targetId BIGINT;
    OPEN metaDataCursor;
    FETCH NEXT FROM metaDataCursor INTO @purgeId, @targetId;
    WHILE @@FETCH_STATUS = 0
    BEGIN 
        BEGIN TRANSACTION 
        BEGIN TRY 
            DELETE FROM [bll].[Metric] WHERE [MetricId] = @targetId;
            UPDATE [bll].[MetricPurge] 
            SET [Status] = 255,
                [UpdateTimestamp] = SYSUTCDATETIME()
            WHERE [PurgeId] = @purgeId;
            COMMIT TRANSACTION; 
        END TRY 
        BEGIN CATCH 
            ROLLBACK TRANSACTION;
            THROW;
        END CATCH    
        FETCH NEXT FROM metaDataCursor INTO @purgeId, @targetId;
    END 
    CLOSE metaDataCursor;
    DEALLOCATE metaDataCursor;
END