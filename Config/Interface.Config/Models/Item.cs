using System;

namespace BrassLoon.Interface.Config.Models
{
    public class Item
    {
        public Guid? ItemId { get; set; }
        public Guid? DomainId { get; set; }
        public string Code { get; set; }
        public dynamic Data { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
    }
}
