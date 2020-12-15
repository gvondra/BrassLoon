CREATE PROCEDURE [bll].[GetAllTraceEventCode]
	@domainId UNIQUEIDENTIFIER
AS
SELECT [EventCode] 
FROM [bll].[Trace] WITH(READUNCOMMITTED)
WHERE [DomainId] = @domainId
ORDER BY [EventCode]
;