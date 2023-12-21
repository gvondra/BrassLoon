using BrassLoon.DataClient;

namespace BrassLoon.Address.Data.Models
{
    public class AddressData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid AddressId { get; set; }
        [ColumnMapping] public Guid DomainId { get; set; }
        [ColumnMapping] public Guid KeyId { get; set; }
        [ColumnMapping] public byte[] InitializationVector { get; set; }
        [ColumnMapping] public byte[] Hash { get; set; }
        [ColumnMapping] public byte[] Attention { get; set; }
        [ColumnMapping] public byte[] Addressee { get; set; }
        [ColumnMapping] public byte[] Delivery { get; set; }
        [ColumnMapping] public byte[] City { get; set; }
        [ColumnMapping] public byte[] Territory { get; set; }
        [ColumnMapping] public byte[] PostalCode { get; set; }
        [ColumnMapping] public byte[] Country { get; set; }
        [ColumnMapping] public byte[] County { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
    }
}
