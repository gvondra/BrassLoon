CREATE PROCEDURE [bll].[GetTopExceptionBeforeTimestamp]
	@domainId UNIQUEIDENTIFIER,
	@maxTimestamp DATETIME2(4)
AS
SELECT TOP 10000
	[ExceptionId],
	[ExceptionGuid],
	[ParentExceptionId]
	[ParentExceptionGuid],
	[DomainId],
	[Message],
	[TypeName],
	[Source],
	[AppDomain],
	[TargetSite],
	[StackTrace],
	[Data],
	[CreateTimestamp],
	[EventId], 
	[Category], 
	[Level]
FROM [bll].[Exception] WITH(READUNCOMMITTED)
WHERE [DomainId] = @domainId
	AND [ParentExceptionId] IS NULL
	AND [CreateTimestamp] < @maxTimestamp
	AND [CreateTimestamp] in (
		SELECT DISTINCT TOP 50 [CreateTimestamp]
		FROM [bll].[Exception] WITH(READUNCOMMITTED)
		WHERE [DomainId] = @domainId
			AND [CreateTimestamp] < @maxTimestamp
			AND [ParentExceptionId] IS NULL
		ORDER BY [CreateTimestamp] DESC
		)
ORDER BY [CreateTimestamp] DESC
;