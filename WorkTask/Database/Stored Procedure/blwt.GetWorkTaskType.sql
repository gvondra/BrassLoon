﻿CREATE PROCEDURE [blwt].[GetWorkTaskType]
	@id UNIQUEIDENTIFIER
AS
SELECT TOP 1 [WorkTaskTypeId], [DomainId], [Title], [Description], [CreateTimestamp], [UpdateTimestamp],
	(SELECT COUNT(1) FROM [blwt].[WorkTask] WITH(READUNCOMMITTED) WHERE [DomainId] = [blwt].[WorkTaskType].[DomainId] AND [WorkTaskTypeId] = [blwt].[WorkTaskType].[WorkTaskTypeId]) [WorkTaskCount]
FROM [blwt].[WorkTaskType] 
WHERE [WorkTaskTypeId] = @id
;