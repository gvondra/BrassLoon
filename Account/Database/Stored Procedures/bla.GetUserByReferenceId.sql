﻿CREATE PROCEDURE [bla].[GetUserByReferenceId]
	@referenceId VARCHAR(512)
AS
SELECT [UserGuid], [ReferenceId], [Name], [EmailAddressGuid], [Roles], [CreateTimestamp], [UpdateTimestamp]
FROM [bla].[User]
WHERE [ReferenceId] = @referenceId
;