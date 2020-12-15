CREATE PROCEDURE [bll].[GetTopTraceBeforeTimestamp]
	@domainId UNIQUEIDENTIFIER,
	@eventCode VARCHAR(200),
	@maxTimestamp DATETIME2(4)
AS
SELECT TOP 10000
	[TraceId],
	[DomainId],
	[EventCode],
	[Message],
	[Data],
	[CreateTimestamp]
FROM [bll].[Trace] WITH(READUNCOMMITTED)
WHERE [DomainId] = @domainId
	AND [EventCode] = @eventCode
	AND [CreateTimestamp] < @maxTimestamp
	AND [CreateTimestamp] in (
		SELECT DISTINCT TOP 50 [CreateTimestamp]
		FROM [bll].[Trace] WITH(READUNCOMMITTED)
		WHERE [DomainId] = @domainId
			AND [EventCode] = @eventCode
			AND [CreateTimestamp] < @maxTimestamp
		ORDER BY [CreateTimestamp] DESC
		)
ORDER BY [CreateTimestamp] DESC
;