CREATE PROCEDURE [bla].[UpdateDomain]
	@id UNIQUEIDENTIFIER,
	@name NVARCHAR(2000),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [bla].[Domain]
	SET [Name] = @name,
		[UpdateTimestamp] = @timestamp
	WHERE [DomainGuid] = @id
	;
END