using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogAPI
{
    public class CoreSettings : ISettings
    {
        public string ConnectionString { get; set; }
    }
}
