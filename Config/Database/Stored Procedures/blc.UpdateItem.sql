CREATE PROCEDURE [blc].[UpdateItem]
	@id UNIQUEIDENTIFIER,
	@code VARCHAR(256),
	@data VARCHAR(MAX),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	INSERT INTO [blc].[ItemHistory] ([ItemId], [DomainId], [Code], [Data], [CreateTimestamp]) 
	SELECT [ItemId], [DomainId], [Code], [Data], @timestamp
	FROM [blc].[Item]
	WHERE [ItemId] = @id
	;
	UPDATE [blc].[Item] 
	SET 
		[Code] = @code, 
		[Data] = @data, 
		[UpdateTimestamp] = @timestamp
	WHERE [ItemId] = @id
	;
END