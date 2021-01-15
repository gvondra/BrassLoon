using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigAPI
{
    public class Settings
    {
        public string ConnectionString { get; set; }
        public string ConnectionStringUser { get; set; }
        public bool EnableDatabaseAccessToken { get; set; } = false;
        public string KeyVaultAddress { get; set; }
        public string AccountApiBaseAddress { get; set; }
        public string LogApiBaseAddress { get; set; }
        public string ExceptionLoggingDomainId { get; set; }
    }
}
