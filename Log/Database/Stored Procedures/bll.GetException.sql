CREATE PROCEDURE [bll].[GetException]
	@exceptionId BIGINT
AS
SELECT
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
FROM [bll].[Exception]
WHERE [ExceptionId] = @exceptionId
;