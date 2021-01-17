﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Interface.Config.Models
{
    public class LookupHistory
    {
        public Guid? LookupHistoryId { get; set; }
        public Guid? DomainId { get; set; }
        public string Code { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public DateTime? CreateTimestamp { get; set; }
    }
}
