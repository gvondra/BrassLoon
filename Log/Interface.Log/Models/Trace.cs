using System;

namespace BrassLoon.Interface.Log.Models
{
    public class Trace
    {
        public long? TraceId { get; set; }
        public Guid? DomainId { get; set; }
        public string EventCode { get; set; }
        public string Message { get; set; }
        public dynamic Data { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public EventId? EventId { get; set; }
        public string Category { get; set; }
        public string Level { get; set; }
    }
}
