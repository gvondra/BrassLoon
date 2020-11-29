using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Interface.Account.Models
{
    public class Domain
    {
        public Guid? DomainId { get; set; }
        public Guid? AccountId { get; set; }
        public string Name { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
    }
}
