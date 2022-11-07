CREATE PROCEDURE [bll].[GetTopMetricBeforeTimestamp]
	@domainId UNIQUEIDENTIFIER,
	@eventCode VARCHAR(200),
	@maxTimestamp DATETIME2(4)
AS
SELECT TOP 10000
	[MetricId],
	[DomainId],
	[EventCode],
	[Magnitude],
	[Data],
	[CreateTimestamp],
	[Status],
	[Requestor]
FROM [bll].[Metric] WITH(READUNCOMMITTED)
WHERE [DomainId] = @domainId
	AND [EventCode] = @eventCode
	AND [CreateTimestamp] < @maxTimestamp
	AND [CreateTimestamp] in (
		SELECT DISTINCT TOP 50 [CreateTimestamp]
		FROM [bll].[Metric] WITH(READUNCOMMITTED)
		WHERE [DomainId] = @domainId
			AND [EventCode] = @eventCode
			AND [CreateTimestamp] < @maxTimestamp
		ORDER BY [CreateTimestamp] DESC
		)
ORDER BY [CreateTimestamp] DESC
;