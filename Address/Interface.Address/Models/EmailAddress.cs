using Google.Protobuf.WellKnownTypes;
using System;

namespace BrassLoon.Interface.Address.Models
{
    public class EmailAddress
    {
        public Guid? EmailAddressId { get; set; }
        public Guid? DomainId { get; set; }
        public string Address { get; set; }
        public DateTime? CreateTimestamp { get; set; }

        internal static EmailAddress Create(Protos.EmailAddress emailAddress)
        {
            return new EmailAddress
            {
                EmailAddressId = !string.IsNullOrEmpty(emailAddress.EmailAddressId) ? Guid.Parse(emailAddress.EmailAddressId) : default(Guid?),
                DomainId = !string.IsNullOrEmpty(emailAddress.DomainId) ? Guid.Parse(emailAddress.DomainId) : default(Guid?),
                Address = emailAddress.Aaddress ?? string.Empty,
                CreateTimestamp = emailAddress.CreateTimestamp?.ToDateTime()
            };
        }

        internal Protos.EmailAddress ToProto()
        {
            return new Protos.EmailAddress
            {
                EmailAddressId = EmailAddressId?.ToString("D") ?? string.Empty,
                DomainId = DomainId?.ToString("D") ?? string.Empty,
                Aaddress = Address ?? string.Empty,
                CreateTimestamp = CreateTimestamp.HasValue ? Timestamp.FromDateTime(CreateTimestamp.Value) : default
            };
        }
    }
}
