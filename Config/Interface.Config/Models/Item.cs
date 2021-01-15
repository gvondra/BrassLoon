using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Interface.Config.Models
{
    public class Item
    {
        public Guid? ItemId { get; set; }
        public Guid? DomainId { get; set; }
        public string Code { get; set; }
        public string Data { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
    }
}
