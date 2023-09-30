using System;

namespace BrassLoon.Interface.Account.Models
{
    public class Account
    {
        public Guid AccountId { get; set; }
        public string Name { get; set; }
        public bool Locked { get; set; }
        public DateTime CreateTimestamp { get; set; }
        public DateTime UpdateTimestamp { get; set; }
    }
}
