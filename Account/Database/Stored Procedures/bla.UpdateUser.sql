﻿CREATE PROCEDURE [bla].[UpdateUser]
	@guid UNIQUEIDENTIFIER, 
	@emailAddressGuid UNIQUEIDENTIFIER,
	@roles SMALLINT,
	@name VARCHAR(512),
	@timestamp DATETIME2(4) OUT
AS
BEGIN
	SET @timestamp = SYSUTCDATETIME();
	UPDATE [bla].[User]
	SET 
		[Name] = @name, 
		[EmailAddressGuid] = @emailAddressGuid, 
		[Roles] = @roles,
		[UpdateTimestamp] = @timestamp
	WHERE [UserGuid] = @guid
	;
END