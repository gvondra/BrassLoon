using System;

namespace BrassLoon.Interface.Config.Models
{
    public class ItemHistory
    {
        public Guid? ItemHistoryId { get; set; }
        public Guid? DomainId { get; set; }
        public string Code { get; set; }
        public dynamic Data { get; set; }
        public DateTime? CreateTimestamp { get; set; }
    }
}
