using System;

namespace BrassLoon.Interface.Account.Models
{
    public class ClientCredential
    {
        public Guid? ClientId { get; set; }
        public string Secret { get; set; }
    }
}
