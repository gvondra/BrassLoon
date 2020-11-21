CREATE PROCEDURE [bla].[GetEmailAddressByAddress]
	@address NVARCHAR(2000)
AS
SELECT [EmailAddressGuid], [Address], [CreateTimestamp]
FROM [bla].[EmailAddress]
WHERE [Address] = @address 
;