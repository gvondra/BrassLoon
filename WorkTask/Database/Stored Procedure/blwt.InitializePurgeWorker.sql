CREATE PROCEDURE [blwt].[InitializePurgeWorker]
AS
BEGIN 
	DECLARE @minTimestamp DATETIME2(4) = DATEADD(hour, -36, SYSUTCDATETIME());
	BEGIN TRANSACTION
	BEGIN TRY
		-- Delete everything that is old and done or errored
		DELETE FROM [blwt].[PurgeWorker]
		WHERE ([Status] = 255 OR [Status] < 0)
			AND [UpdateTimestamp] < @minTimestamp
		;
		-- Rest everything that doesn't appear to be progressing
		UPDATE [blwt].[PurgeWorker]
		SET [Status] = 0
		WHERE [Status] <> 0
			AND [UpdateTimestamp] < @minTimestamp
		;		
		-- Add everything that is missing
		INSERT INTO [blwt].[PurgeWorker] ([PurgeWorkerId], [DomainId])
		SELECT NEWID(), [DomainId]
		FROM (
			SELECT DISTINCT [DomainId]
			FROM [blwt].[WorkTask] WITH(READUNCOMMITTED)
			) [DOMAIN]
		WHERE NOT EXISTS (SELECT TOP 1 1 FROM [blwt].[PurgeWorker] WHERE [blwt].[PurgeWorker].[DomainId] = [DOMAIN].[DomainId])
		;
		COMMIT TRANSACTION
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;
		THROW;
	END CATCH
END