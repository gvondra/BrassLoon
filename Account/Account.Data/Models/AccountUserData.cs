using BrassLoon.DataClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BrassLoon.Account.Data.Models
{
    public class AccountUserData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid AccountGuid { get; set; }

        [ColumnMapping]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid UserGuid { get; set; }

        [ColumnMapping]
        public bool IsActive { get; set; }

        [ColumnMapping("CreateTimestamp", IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }

        [ColumnMapping("UpdateTimestamp", IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime UpdateTimestamp { get; set; }
    }
}
