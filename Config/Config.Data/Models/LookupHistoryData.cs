﻿using BrassLoon.DataClient;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace BrassLoon.Config.Data.Models
{
    public class LookupHistoryData
    {
        [ColumnMapping("LookupHistoryId", IsPrimaryKey = true)]
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid LookupHistoryId { get; set; }

        [ColumnMapping("LookupId")]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid LookupId { get; set; }

        [ColumnMapping("DomainId")]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
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
