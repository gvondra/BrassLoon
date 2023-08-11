using System;

namespace BrassLoon.Interface.Log.Models
{
    public class Metric
    {
        public long? MetricId { get; set; }
        public Guid? DomainId { get; set; }
        public string EventCode { get; set; }
        public double? Magnitude { get; set; }
        public dynamic Data { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public string Status { get; set; }
        public string Requestor { get; set; }
        public EventId? EventId { get; set; }
        public string Category { get; set; }
        public string Level { get; set; }
    }
}
