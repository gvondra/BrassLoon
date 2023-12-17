using BrassLoon.DataClient;

namespace BrassLoon.Address.Data.Models
{
    public class AddressData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid AddressId { get; set; }
        [ColumnMapping] public Guid DomainId { get; set; }
        [ColumnMapping] public Guid KeyId { get; set; }
        public byte[] InitializationVector { get; set; }
        [ColumnMapping] public byte[] Hash { get; set; }
        public byte[] Attention { get; set; }
        public byte[] Addressee { get; set; }
        public byte[] Delivery { get; set; }
        public byte[] City { get; set; }
        public byte[] Territory { get; set; }
        public byte[] PostalCode { get; set; }
        public byte[] Country { get; set; }
        public byte[] County { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
    }
}
