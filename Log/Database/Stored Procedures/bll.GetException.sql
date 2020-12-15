CREATE PROCEDURE [bll].[GetException]
	@exceptionId BIGINT
AS
SELECT
	[ExceptionId],
	[ParentExceptionId],
	[DomainId],
	[Message],
	[TypeName],
	[Source],
	[AppDomain],
	[TargetSite],
	[StackTrace],
	[Data],
	[CreateTimestamp]
FROM [bll].[Exception]
WHERE [ExceptionId] = @exceptionId
;