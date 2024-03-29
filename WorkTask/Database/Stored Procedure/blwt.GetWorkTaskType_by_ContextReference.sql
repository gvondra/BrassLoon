﻿CREATE PROCEDURE [blwt].[GetWorkTaskType_by_ContextReference]
	@domainId UNIQUEIDENTIFIER,
	@referenceType SMALLINT,
	@referenceValueHash VARBINARY(64)
AS
SELECT [WorkTaskTypeId], [DomainId], [Code], [Title], [Description], [PurgePeriod],
	[CreateTimestamp], [UpdateTimestamp],
	(SELECT COUNT(1) FROM [blwt].[WorkTask] WITH(READUNCOMMITTED) WHERE [DomainId] = [blwt].[WorkTaskType].[DomainId] AND [WorkTaskTypeId] = [blwt].[WorkTaskType].[WorkTaskTypeId]) [WorkTaskCount]
FROM [blwt].[WorkTaskType] 
WHERE [DomainId] = @domainId
	AND EXISTS (
	SELECT TOP 1 1
	FROM [blwt].[WorkTask] [tsk]
	INNER JOIN [blwt].[WorkTaskContext] [wtc] on [tsk].[WorkTaskId] = [wtc].[WorkTaskId]
	WHERE [tsk].[WorkTaskTypeId] = [blwt].[WorkTaskType].[WorkTaskTypeId]
	AND [tsk].[DomainId] = @domainId
	AND [wtc].[DomainId] = @domainId
	AND [wtc].[ReferenceType] = @referenceType
	AND [wtc].[ReferenceValueHash] = @referenceValueHash
);