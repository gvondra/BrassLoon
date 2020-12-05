using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogAPI
{
    public class Settings
    {
        public string ConnectionString { get; set; }
        public string ConnectionStringUser { get; set; }
        public string KeyVaultAddress { get; set; }
        public string AccountApiBaseAddress { get; set; }
    }
}
