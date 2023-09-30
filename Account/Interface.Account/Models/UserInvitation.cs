using System;

namespace BrassLoon.Interface.Account.Models
{
    public class UserInvitation
    {
        public Guid? UserInvitationId { get; set; }
        public string EmailAddress { get; set; }
        public short? Status { get; set; }
        public DateTime? ExpirationTimestamp { get; set; }
        public DateTime? CreateTimestamp { get; set; }
        public DateTime? UpdateTimestamp { get; set; }
    }
}
