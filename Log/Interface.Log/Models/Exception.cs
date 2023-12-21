using System;

namespace BrassLoon.Interface.Log.Models
{
#pragma warning disable S2166 // Classes named like "Exception" should extend "Exception" or a subclass
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
    public class Exception
    {
        public long? ExceptionId { get; set; }
        public Guid? DomainId { get; set; }
        public string Message { get; set; }
        public string TypeName { get; set; }
        public string Source { get; set; }
        public string AppDomain { get; set; }
        public string TargetSite { get; set; }
        public string StackTrace { get; set; }
        public dynamic Data { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public Exception InnerException { get; set; }
        public EventId? EventId { get; set; }
        public string Category { get; set; }
        public string Level { get; set; }
    }
#pragma warning restore S2166 // Classes named like "Exception" should extend "Exception" or a subclass
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
}
