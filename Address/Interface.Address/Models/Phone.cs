using Google.Protobuf.WellKnownTypes;
using System;

namespace BrassLoon.Interface.Address.Models
{
    public class Phone
    {
        public Guid? PhoneId { get; set; }
        public Guid? DomainId { get; set; }
        public string Number { get; set; }
        public string CountryCode { get; set; }
        public DateTime? CreateTimestamp { get; set; }

        internal static Phone Create(Protos.Phone emailAddress)
        {
            return new Phone
            {
                PhoneId = !string.IsNullOrEmpty(emailAddress.PhoneId) ? Guid.Parse(emailAddress.PhoneId) : default(Guid?),
                DomainId = !string.IsNullOrEmpty(emailAddress.DomainId) ? Guid.Parse(emailAddress.DomainId) : default(Guid?),
                Number = emailAddress.Number ?? string.Empty,
                CountryCode = emailAddress.CountryCode ?? string.Empty,
                CreateTimestamp = emailAddress.CreateTimestamp?.ToDateTime()
            };
        }

        internal Protos.Phone ToProto()
        {
            return new Protos.Phone
            {
                PhoneId = PhoneId?.ToString("D") ?? string.Empty,
                DomainId = DomainId?.ToString("D") ?? string.Empty,
                Number = Number ?? string.Empty,
                CountryCode = CountryCode ?? string.Empty,
                CreateTimestamp = CreateTimestamp.HasValue ? Timestamp.FromDateTime(CreateTimestamp.Value) : default
            };
        }
    }
}
