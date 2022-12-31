using System;

namespace BrassLoon.Extensions.Logging
{
    public class LoggerConfiguration
    {
        public string AccountApiBaseAddress { get; set; }
        public string LogApiBaseAddress { get; set; }
        public Guid LogDomainId { get; set; }
        public Guid LogClientId { get; set; }
        public string LogClientSecret { get; set; }
    }
}
