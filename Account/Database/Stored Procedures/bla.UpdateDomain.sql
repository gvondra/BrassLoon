CREATE PROCEDURE [bla].[UpdateDomain]
	@id UNIQUEIDENTIFIER,
	@name NVARCHAR(2000),
	@deleted BIT,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [bla].[Domain]
	SET [Name] = @name,
		[Deleted] = @deleted,
		[UpdateTimestamp] = @timestamp
	WHERE [DomainGuid] = @id
	;
END