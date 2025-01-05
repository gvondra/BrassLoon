using BrassLoon.DataClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BrassLoon.Address.Data.Models
{
    public class AddressData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)]
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid AddressId { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid DomainId { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid KeyId { get; set; }

        [ColumnMapping]
        public byte[] InitializationVector { get; set; }

        [ColumnMapping]
        public byte[] Hash { get; set; }

        [ColumnMapping]
        public byte[] Attention { get; set; }

        [ColumnMapping]
        public byte[] Addressee { get; set; }

        [ColumnMapping]
        public byte[] Delivery { get; set; }

        [ColumnMapping]
        public byte[] Secondary { get; set; }

        [ColumnMapping]
        public byte[] City { get; set; }

        [ColumnMapping]
        public byte[] Territory { get; set; }

        [ColumnMapping]
        public byte[] PostalCode { get; set; }

        [ColumnMapping]
        public byte[] Country { get; set; }

        [ColumnMapping]
        public byte[] County { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }
    }
}
