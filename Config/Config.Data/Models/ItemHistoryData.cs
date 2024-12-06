using BrassLoon.DataClient;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BrassLoon.Config.Data.Models
{
    public class ItemHistoryData
    {
        [ColumnMapping("ItemHistoryId", IsPrimaryKey = true)]
        [BsonId]
        public Guid ItemHistoryId { get; set; }

        [ColumnMapping("ItemId")]
        public Guid ItemId { get; set; }

        [ColumnMapping("DomainId")]
        public Guid DomainId { get; set; }

        [ColumnMapping("Code")]
        [BsonRequired]
        public string Code { get; set; }

        [ColumnMapping("Data")]
        [BsonRequired]
        public string Data { get; set; }

        [ColumnMapping("CreateTimestamp", IsUtc = true)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime CreateTimestamp { get; set; }
    }
}
