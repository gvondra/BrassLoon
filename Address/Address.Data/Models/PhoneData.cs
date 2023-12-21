using BrassLoon.DataClient;

namespace BrassLoon.Address.Data.Models
{
    public class PhoneData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid PhoneId { get; set; }
        [ColumnMapping] public Guid DomainId { get; set; }
        [ColumnMapping] public Guid KeyId { get; set; }
        [ColumnMapping] public byte[] InitializationVector { get; set; }
        [ColumnMapping] public byte[] Hash { get; set; }
        [ColumnMapping] public byte[] Number { get; set; }
        [ColumnMapping] public string CountryCode { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
    }
}
