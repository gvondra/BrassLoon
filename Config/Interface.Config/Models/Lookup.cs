using System;
using System.Collections.Generic;

namespace BrassLoon.Interface.Config.Models
{
    public class Lookup
    {
        public Guid? LookupId { get; set; }
        public Guid? DomainId { get; set; }
        public string Code { get; set; }
        public Dictionary<string, string> Data { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
    }
}
