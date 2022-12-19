using System;

namespace BrassLoon.Interface.Authorization.Models
{
    public sealed class ClientCredential
    {
        public Guid? ClientId { get; set; }
        public string Secret { get; set; }
    }
}
