CREATE PROCEDURE [blc].[UpdateLookup]
	@id UNIQUEIDENTIFIER,
	@code VARCHAR(256),
	@data VARCHAR(MAX),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blc].[LookupHistory] ([LookupId], [DomainId], [Code], [Data], [CreateTimestamp]) 
	SELECT [LookupId], [DomainId], [Code], [Data], @timestamp
	FROM [blc].[Lookup]
	WHERE [LookupId] = @id
	;
	UPDATE [blc].[Lookup] 
	SET
		[Code] = @code, 
		[Data] = @DATA, 
		[UpdateTimestamp] = @timestamp
	WHERE [LookupId] = @id
	;
END