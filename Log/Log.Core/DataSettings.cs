﻿using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Log.Core
{
    public class DataSettings : ISettings
    {
        public string ConnectionString { get; set; }
    }
}
