using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountAPI
{
    public class CoreSettings : ISettings
    {
        public string ConnectionString { get; set; }
    }
}
