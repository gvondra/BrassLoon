CREATE PROCEDURE [blc].[CreateLookup]
	@id UNIQUEIDENTIFIER OUT,
	@domainId UNIQUEIDENTIFIER,
	@code VARCHAR(256),
	@data VARCHAR(MAX),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	SET @id = NEWID();
	INSERT INTO [blc].[Lookup] ([LookupId], [DomainId], [Code], [Data], [CreateTimestamp], [UpdateTimestamp]) 
	VALUES (@id, @domainId, @code, @data, @timestamp, @timestamp) 
	;
END