CREATE PROCEDURE [bll].[ClaimPurgeWorker]
	@id UNIQUEIDENTIFIER OUT
AS
BEGIN
	IF 0 < (SELECT COUNT(1) FROM [bll].[PurgeWorker] WHERE [Status] = 0)
	BEGIN
		SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
		BEGIN TRANSACTION 
		BEGIN TRY
			DECLARE @purgeWorkId UNIQUEIDENTIFIER = NULL;
			SELECT TOP 1 @purgeWorkId = [PurgeWorkerId] FROM [bll].[PurgeWorker] WHERE [Status] = 0;
			IF @purgeWorkId IS NOT NULL
			BEGIN
				UPDATE [bll].[PurgeWorker] 
				SET [Status] = 1,
					[UpdateTimestamp] = SYSUTCDATETIME()
				WHERE [PurgeWorkerId] = @purgeWorkId
					AND [Status] = 0
				;
			END
			COMMIT TRANSACTION;
			SET @id = @purgeWorkId;
		END TRY
		BEGIN CATCH
			ROLLBACK TRANSACTION;
			THROW;
		END CATCH
	END
END