CREATE PROCEDURE [bll].[GetException_v2]
	@exceptionId UNIQUEIDENTIFIER
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
WHERE [ExceptionGuid] = @exceptionId
;