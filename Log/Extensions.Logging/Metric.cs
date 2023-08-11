using System;
using System.Collections.Generic;

namespace BrassLoon.Extensions.Logging
{
    public class Metric
    {
        private DateTime _createTimestamp = DateTime.UtcNow;
        private DateTime _startTime = DateTime.Now;

        public Guid? DomainId { get; set; }
        public string EventCode { get; set; }
        public double? Magnitude { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public DateTime CreateTimestamp
        { 
            get => _createTimestamp;
            set => _createTimestamp = value.ToUniversalTime();
        }
        public string Status { get; set; }
        public string Requestor { get; set; }

        public void SetStartTime(DateTime? start)
        {
            if (!start.HasValue)
                start = DateTime.Now;
            _startTime = start.Value.ToLocalTime();
        }

        public void SetMagnitudeSeconds(DateTime? endTime)
        {
            if (!endTime.HasValue)
                endTime = DateTime.Now;
            endTime = endTime.Value.ToLocalTime();
            Magnitude = endTime.Value.Subtract(_startTime).TotalSeconds;
        }
    }
}
