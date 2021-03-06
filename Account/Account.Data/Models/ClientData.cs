﻿using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Account.Data.Models
{
    public class ClientData : DataManagedStateBase
    {
        [ColumnMapping("ClientId")] public Guid ClientId { get; set; }
        [ColumnMapping("AccountGuid")] public Guid AccountId { get; set; }
        [ColumnMapping("Name")] public string Name { get; set; }
        [ColumnMapping("CreateTimestamp", IsUtc = true)] public DateTime CreateTimestamp { get; set; }
        [ColumnMapping("UpdateTimestamp", IsUtc = true)] public DateTime UpdateTimestamp { get; set; }
    }
}
