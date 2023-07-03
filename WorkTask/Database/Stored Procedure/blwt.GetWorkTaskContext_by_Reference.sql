CREATE PROCEDURE [blwt].[GetWorkTaskContext_by_Reference]
    @domainId UNIQUEIDENTIFIER,
	@referenceType SMALLINT,
	@referenceValueHash VARBINARY(64)
AS
SELECT [WorkTaskContextId], [DomainId], [WorkTaskId], [Status], [ReferenceType], [ReferenceValue], [ReferenceValueHash], [CreateTimestamp]
FROM [blwt].[WorkTaskContext]
WHERE [DomainId] = @domainId
AND [ReferenceType] = @referenceType
AND [ReferenceValueHash] = @referenceValueHash
ORDER BY [CreateTimestamp]
;