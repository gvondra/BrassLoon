CREATE PROCEDURE [bll].[GetAllMetricEventCode]
	@domainId UNIQUEIDENTIFIER
AS
SELECT DISTINCT [EventCode] 
FROM [bll].[Metric] WITH(READUNCOMMITTED)
WHERE [DomainId] = @domainId
ORDER BY [EventCode]
;