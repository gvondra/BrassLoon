DECLARE @accountGuid UNIQUEIDENTIFIER;
DECLARE accountCursor CURSOR FOR 
SELECT [AccountGuid] 
FROM [bla].[Account]
WHERE [Name] like 'test-generated-%';

OPEN accountCursor;

BEGIN TRANSACTION;
BEGIN TRY

FETCH NEXT FROM accountCursor
INTO @accountGuid;

WHILE @@FETCH_STATUS = 0
BEGIN
PRINT 'Deleting account ' + CONVERT(VARCHAR(128), @accountGuid);

DELETE FROM [bla].[Client] WHERE [AccountGuid] = @accountGuid;
DELETE FROM [bla].[Domain] WHERE [AccountGuid] = @accountGuid;
DELETE FROM [bla].[AccountUser] WHERE [AccountGuid] = @accountGuid;
DELETE FROM [bla].[Account] WHERE [AccountGuid] = @accountGuid;

FETCH NEXT FROM accountCursor
INTO @accountGuid;
END

COMMIT TRANSACTION;
END TRY
BEGIN CATCH
ROLLBACK TRANSACTION;
PRINT 'Error (' + CONVERT(VARCHAR(128), ERROR_NUMBER()) + ') ' + ERROR_MESSAGE();
PRINT 'Severity: ' + CONVERT(VARCHAR(128), ERROR_SEVERITY());
PRINT 'State: ' + CONVERT(VARCHAR(128), ERROR_STATE());
PRINT 'Procedure: ' + CONVERT(VARCHAR(128), ISNULL(ERROR_PROCEDURE(), ''));
PRINT 'Line: ' + CONVERT(VARCHAR(128), ERROR_LINE());
END CATCH

CLOSE accountCursor;
DEALLOCATE accountCursor;