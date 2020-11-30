CREATE PROCEDURE [bla].[CreateClientCredential]
	@id UNIQUEIDENTIFIER OUT, 
	@clientId UNIQUEIDENTIFIER,
	@secret BINARY(64),
	@isActive BIT,
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	SET @id = NEWID();
	IF @isActive = 1
		UPDATE [bla].[ClientCredential]
		SET [IsActive] = 0,
			[UpdateTimestamp] = @timestamp
		WHERE [ClientId] = @clientId
		;
	INSERT INTO [bla].[ClientCredential] ([ClientCredentialId], [ClientId], [Secret], [IsActive], [CreateTimestamp], [UpdateTimestamp])
	VALUES (@id, @clientId, @secret, @isActive, @timestamp, @timestamp)
	;
END