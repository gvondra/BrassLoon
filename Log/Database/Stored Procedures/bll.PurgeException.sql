CREATE PROCEDURE [bll].[PurgeException]
    @domainId UNIQUEIDENTIFIER,
	@maxExpirationTimestamp DATETIME2(4)
AS
BEGIN
	SET NOCOUNT ON;  
    DECLARE metaDataCursor CURSOR FOR 
    SELECT [PurgeId], [TargetId] 
    FROM [bll].[ExceptionPurge]
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
            DELETE FROM [bll].[Exception] WHERE [ExceptionId] = @targetId;
            UPDATE [bll].[ExceptionPurge] 
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