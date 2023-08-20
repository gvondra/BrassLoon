using System;

namespace BrassLoon.Log.Purger
{
    public class AppSettings
    {
        public string ConnectionString { get; set; }
        public string ConnectionStringUser { get; set; }
        public bool EnableDatabaseAccessToken { get; set; } = false;
        public string KeyVaultAddress { get; set; }
        public string AccountApiBaseAddress { get; set; }
        public string LogApiBaseAddress { get; set; }
        public Guid ExceptionLoggingDomainId { get; set; }
        public Guid TraceLoggingDomainId { get; set; }
        public string PurgeMetaDataTimespan { get; set; }
        public string RetensionPeriod { get; set; }
        public Guid ClientId { get; set; }
        public string Secret { get; set; }
    }
}
