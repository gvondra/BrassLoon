﻿CREATE PROCEDURE [bll].[DeleteTracePurgeByMinTimestamp]
	@minTimestamp DATETIME2(4)
AS
BEGIN
	BEGIN TRANSACTION
	BEGIN TRY
		DELETE FROM [bll].[TracePurge]
		WHERE [UpdateTimestamp] < @minTimestamp
			AND ([Status] = 255 or [Status] < 0) -- done or errored
		;
		COMMIT TRANSACTION;
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION;
		THROW;
	END CATCH
END