UPDATE [bll].[Exception]
SET [ParentExceptionGuid] = (
	SELECT TOP 1 [iex].[ExceptionGuid] FROM [bll].[Exception] [iex]
	WHERE [iex].[ExceptionId] = [bll].[Exception].[ParentExceptionId]
)
WHERE [ParentExceptionId] IS NOT NULL
AND [ParentExceptionGuid] IS NULL
;