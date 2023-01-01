CREATE PROCEDURE [bll].[GetInnerException]
	@id BIGINT
AS
SELECT [ExceptionId], [ParentExceptionId], [DomainId], [Message], [TypeName], [Source], [AppDomain], [TargetSite], [StackTrace], [Data], [CreateTimestamp],
	[EventId], [Category], [Level]
FROM [bll].[Exception]
WHERE [ParentExceptionId] = @id
;
