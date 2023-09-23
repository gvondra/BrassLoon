#pragma warning disable IDE0130 // Namespace does not match folder structure
using System;

namespace BrassLoon.WorkTask.Purger
{
    public class AppSettings
    {
        public string ConnectionString { get; set; }
        public bool EnableDatabaseAccessToken { get; set; }
        public string BrassLoonLogRpcBaseAddress { get; set; }
        public Guid? LoggingDomainId { get; set; }
        public Guid? LoggingClientId { get; set; }
        public string LoggingClientSecret { get; set; }
        // in months
        public short DefaultPurgePeriod { get; set; } = 18;
    }
}
#pragma warning restore IDE0130 // Namespace does not match folder structure