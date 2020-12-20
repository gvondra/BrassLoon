using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Interface.Log.Models
{
    public class PurgeWorker
    {
        public Guid? PurgeWorkerId { get; set; }
        public Guid? DomainId { get; set; }
        public short? Status { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
    }
}
