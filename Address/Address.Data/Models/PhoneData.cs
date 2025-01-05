using BrassLoon.DataClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BrassLoon.Address.Data.Models
{
    public class PhoneData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)]
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid PhoneId { get; set; }

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
        public byte[] Number { get; set; }

        [ColumnMapping]
        public string CountryCode { get; set; }

        [ColumnMapping(IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }
    }
}
