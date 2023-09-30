CREATE PROCEDURE [blwt].[PurgeWorkTask]
    @domainId UNIQUEIDENTIFIER,
	@expirationTimestamp DATETIME2(4)
AS
BEGIN
	SET NOCOUNT ON;  
    DECLARE metaDataCursor CURSOR FOR 
    SELECT [PurgeId], [TargetId] 
    FROM [blwt].[WorkTaskPurge]
    WHERE [DomainId] = @domainId
        AND [Status] = 0
        AND [ExpirationTimestamp] < @expirationTimestamp
    ORDER BY [ExpirationTimestamp] DESC
    ;

    DECLARE @purgeId UNIQUEIDENTIFIER;
    DECLARE @targetId UNIQUEIDENTIFIER;
    OPEN metaDataCursor;
    FETCH NEXT FROM metaDataCursor INTO @purgeId, @targetId;
    WHILE @@FETCH_STATUS = 0
    BEGIN 
        BEGIN TRANSACTION 
        BEGIN TRY 
            DELETE FROM [blwt].[WorkTask] WHERE [WorkTaskId] = @targetId;
            UPDATE [blwt].[WorkTaskPurge] 
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