using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Authorization.Models
{
    public class User
    {
        public Guid? UserId { get; set; }
        public Guid? DomainId { get; set; }
        public string ReferenceId { get; set; }
        public string EmailAddress { get; set; }
        public string Name { get; set; }
        // Roles as list of tuples (policy name, name)
        public List<ValueTuple<string, string>> Roles { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
    }
}
