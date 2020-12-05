using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountAPI
{
    public class Settings
    {
        public string TknCsp { get; set; }
        public string ConnectionString { get; set; }
        public string ConnectionStringUser { get; set; }
        public string KeyVaultAddress { get; set; }
        public string IdIssuer { get; set; }
        public string Issuer { get; set; }
    }
}
