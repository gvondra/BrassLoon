﻿CREATE PROCEDURE [bla].[GetAccount]
	@guid UNIQUEIDENTIFIER
AS
SELECT [AccountGuid], [Name], [Locked], [CreateTimestamp], [UpdateTimestamp]
FROM [bla].[Account]
WHERE [AccountGuid] = @guid
;
