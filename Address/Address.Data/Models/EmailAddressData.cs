using BrassLoon.DataClient;

namespace BrassLoon.Address.Data.Models
{
    public class EmailAddressData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid EmailAddressId { get; set; }
        [ColumnMapping] public Guid DomainId { get; set; }
        [ColumnMapping] public Guid KeyId { get; set; }
        [ColumnMapping] public byte[] InitializationVector { get; set; }
        [ColumnMapping] public byte[] Hash { get; set; }
        [ColumnMapping] public byte[] Address { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
    }
}
