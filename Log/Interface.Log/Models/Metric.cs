using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Interface.Log.Models
{
    public class Metric
    {
        public long MetricId { get; set; }
        public Guid? DomainId { get; set; }
        public string EventCode { get; set; }
        public double? Magnitude { get; set; }
        public dynamic Data { get; set; }
        public DateTime? CreateTimestamp { get; set; }
    }
}
