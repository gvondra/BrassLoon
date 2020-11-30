using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Interface.Account.Models
{
    public class ClientCredentialRequest : Client
    {
        public string Secret { get; set; }
    }
}
