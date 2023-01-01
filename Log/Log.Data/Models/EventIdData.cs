﻿using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data.Models
{
    public class EventIdData : DataManagedStateBase
    {
        [ColumnMapping(IsPrimaryKey = true)] public Guid EventId { get; set; }
        [ColumnMapping()] public Guid DomainId { get; set; }
        [ColumnMapping()] public int Id { get; set; }
        [ColumnMapping()] public string Name { get; set; }
        [ColumnMapping(IsUtc = true)] public DateTime CreateTimestamp { get; set; } 
    }
}
