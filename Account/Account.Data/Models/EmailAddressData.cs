﻿using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Account.Data.Models
{
    public class EmailAddressData : DataManagedStateBase
    {
        [ColumnMapping("EmailAddressGuid", IsPrimaryKey = true)] public Guid EmailAddressGuid { get; set; }
        [ColumnMapping("Address")] public string Address { get; set; }
        [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
    }
}
