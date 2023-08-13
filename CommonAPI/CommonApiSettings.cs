namespace BrassLoon.CommonAPI
{
    public abstract class CommonApiSettings
    {
        public string ConnectionString { get; set; }
        public string ConnectionStringUser { get; set; }
        public bool EnableDatabaseAccessToken { get; set; } = false;
        public string KeyVaultAddress { get; set; }
        public string AccountApiBaseAddress { get; set; }
        public string LogApiBaseAddress { get; set; }
        [Obsolete("Switch to LoggingDomainId, LoggingClientId, and LoggingClientSecret")]
        public string ExceptionLoggingDomainId { get; set; }
        public Guid? LoggingDomainId { get; set; }
        public Guid? LoggingClientId { get; set; }
        public string LoggingClientSecret { get; set; }
    }
}
