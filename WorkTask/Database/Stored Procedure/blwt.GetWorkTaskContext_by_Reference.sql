CREATE PROCEDURE [blwt].[GetWorkTaskContext_by_Reference]
	@referenceType SMALLINT,
	@referenceValueHash VARBINARY(64)
AS
SELECT [WorkTaskContextId], [DomainId], [WorkTaskId], [Status], [ReferenceType], [ReferenceValue], [ReferenceValueHash], [CreateTimestamp]
FROM [blwt].[WorkTaskContext]
WHERE [ReferenceType] = @referenceType
AND [ReferenceValueHash] = @referenceValueHash
ORDER BY [CreateTimestamp]
;