using BrassLoon.DataClient;

namespace BrassLoon.Address.Data.Models
{
    public class AddressData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid AddressId { get; set; }
        [ColumnMapping] public Guid DomainId { get; set; }
        [ColumnMapping] public byte[] Hash { get; set; }
        public string Attention { get; set; }
        public string Addressee { get; set; }
        public string Delivery { get; set; }
        public string City { get; set; }
        public string Territory { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string County { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; }
    }
}
