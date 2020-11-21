using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Account.Core
{
    public class DataSettings : ISettings
    {
        public string ConnectionString { get; set; }
    }
}
