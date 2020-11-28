using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Interface.Account.Models
{
    public class Domain
    {
        public Guid? DomainId { get; }
        public Guid? AccountId { get; }
        public string Name { get; set; }
        public DateTime? CreateTimestamp { get; }
        public DateTime? UpdateTimestamp { get; }
    }
}
