using System;

namespace BrassLoon.Interface.Account.Models
{
    public class Client
    {
        public Guid? ClientId { get; set; }
        public Guid? AccountId { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
    }
}
