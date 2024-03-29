﻿CREATE PROCEDURE [blwt].[GetWorkTaskContext_by_WorkTaskId]
	@workTaskId UNIQUEIDENTIFIER
AS
SELECT [WorkTaskContextId], [DomainId], [WorkTaskId], [Status], [ReferenceType], [ReferenceValue], [ReferenceValueHash], [CreateTimestamp]
FROM [blwt].[WorkTaskContext]
WHERE [WorkTaskId] = @workTaskId
ORDER BY [CreateTimestamp]
;