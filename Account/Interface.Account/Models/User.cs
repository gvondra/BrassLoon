using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Interface.Account.Models
{
    public class User
    {
        public Guid? UserId { get; set; }
        public string Name { get; set; }        
    }
}
