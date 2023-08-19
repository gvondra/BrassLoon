using System;

namespace BrassLoon.Interface.Authorization.Models
{
    public class SigningKey
    {
        public Guid? SigningKeyId { get; set; }
        public Guid? DomainId { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
    }
}
