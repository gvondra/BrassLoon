CREATE PROCEDURE [blad].[GetAddress]
	@id UNIQUEIDENTIFIER
AS
SELECT [AddressId], [DomainId], [KeyId], [Hash], [InitializationVector], 
	[Attention], [Addressee], [Delivery], [City], [Territory], [PostalCode], [Country], [County],
	[CreateTimestamp]
FROM [blad].[Address]
WHERE [AddressId] = @id
;