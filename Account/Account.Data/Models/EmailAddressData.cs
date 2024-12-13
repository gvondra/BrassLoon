using BrassLoon.DataClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BrassLoon.Account.Data.Models
{
    public class EmailAddressData : DataManagedStateBase
    {
        [ColumnMapping("EmailAddressGuid", IsPrimaryKey = true)]
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid EmailAddressGuid { get; set; }

        [ColumnMapping("Address")]
        [BsonRequired]
        public string Address { get; set; }

        [ColumnMapping("CreateTimestamp", IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }
    }
}
